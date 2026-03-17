using Godot;
using System;
using ZooBuilder.entities.player;

public partial class InventoryItemStack : Panel
{
    [Signal]
    public delegate void ItemStackPressedEventHandler(InventoryItemStack stack);

    [Signal]
    public delegate void ItemStackRightClickedEventHandler(InventoryItemStack stack);

    private ItemStackResource _itemStackResource;
    private bool _selected;

    public ItemStackResource ItemStackResource
    {
        get => _itemStackResource;
        set
        {
            _itemStackResource = value;
            if (_selected)
            {
                // TODO Inv: It's not quite clear that this method is called when an InventoryItemStack has been Q'ed enough that the item resource is removed
                //  Also, do I actually want this setter to emit the event below, and ALSO the HotbarGridContainer to emit the same event when scrolling? 
                EventBus.Publish(new SelectedHotbarSlotChangedItemEvent(InventoryIndex));
            }

            Render();
        }
    }

    public int InventoryIndex { get; set; }
    private TextureRect _itemIcon;
    private Label _stackSizeLabel;
    private StyleBoxFlat _selectedStyle;
    private StyleBoxFlat _unselectedStyle;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _itemIcon = GetNode<TextureRect>("%ItemIcon");
        _stackSizeLabel = GetNode<Label>("%StackSize");
        _selectedStyle = ResourceLoader.Load<StyleBoxFlat>("res://src/ui/inventory/hotbar/item_stack_panel_theme_selected.tres");
        _unselectedStyle = ResourceLoader.Load<StyleBoxFlat>("res://src/ui/inventory/hotbar/item_stack_panel_theme_unselected.tres");
        Render();
    }

    public void ClearStackResource()
    {
        ItemStackResource = null;
        Render();
    }

    private void Render()
    {
        if (ItemStackResource != null)
        {
            SetTexture(ItemStackResource.ItemData.Texture);
            SetStackSize(ItemStackResource.Amount + "");
        }
        else
        {
            SetTexture(null);
            SetStackSize(null);
        }
    }

    private void SetTexture(Texture2D texture)
    {
        _itemIcon.Texture = texture;
    }

    private void SetStackSize(string stackSize)
    {
        _stackSizeLabel.Text = "" + stackSize;
    }

    public void DecrementRerenderAndRemoveIfZero()
    {
        ItemStackResource.Amount--;
        if (ItemStackResource.Amount <= 0)
        {
            InventorySingleton.Instance.RemoveStackFromInventory(InventoryIndex);
        }

        Render();
    }

    public void HighlightSlot()
    {
        AddThemeStyleboxOverride("panel", _selectedStyle);
        _selected = true;
    }

    public void RemoveHighlight()
    {
        AddThemeStyleboxOverride("panel", _unselectedStyle);
        _selected = false;
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is not InputEventMouseButton eventMouseMotion) return;
        if (!eventMouseMotion.Pressed) return;
        if (eventMouseMotion.ButtonIndex == MouseButton.Left)
        {
            GD.Print("Clicked left on index ", InventoryIndex);
            EmitSignal(SignalName.ItemStackPressed, this);
        }
        else if (eventMouseMotion.ButtonIndex == MouseButton.Right)
        {
            EmitSignal(SignalName.ItemStackRightClicked, this);
        }
    }
}