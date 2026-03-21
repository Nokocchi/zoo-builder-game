using Godot;
using System;
using ZooBuilder.entities.player;
using ZooBuilder.ui.inventory;

public partial class UiInventorySlot : Panel
{
    [Signal]
    public delegate void SlotClickedEventHandler(int slotIndex);

    [Signal]
    public delegate void SlotRightClickedEventHandler(int slotIndex);
    
    [Export]
    public PackedScene TooltipScene { get; set; }

    public InventorySlotResource InventorySlotResource;
    private bool _selected;
    private bool _hovered;
    private TextureRect _itemIcon;
    private Label _stackSizeLabel;
    private StyleBoxFlat _selectedStyle;
    private StyleBoxFlat _unselectedStyle;
    private bool _initialized;
    //private ItemTooltip _itemTooltip;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _itemIcon = GetNode<TextureRect>("%ItemIcon");
        _stackSizeLabel = GetNode<Label>("%StackSize");
        _selectedStyle = ResourceLoader.Load<StyleBoxFlat>("res://src/ui/inventory/hotbar/item_stack_panel_theme_selected.tres");
        _unselectedStyle = ResourceLoader.Load<StyleBoxFlat>("res://src/ui/inventory/hotbar/item_stack_panel_theme_unselected.tres");
        //_itemTooltip = TooltipScene.Instantiate<ItemTooltip>();
        _initialized = true;
    }

    public void SetInventorySlotResource(InventorySlotResource slot)
    {
        //_itemTooltip?.SetItemStack(slot.GetItem()); // Not sure why item tooltip would be null
        InventorySlotResource = slot;
        slot.SlotContentChanged += Render;
        Render();
    }

    private void Render()
    {
        if (!_initialized) return; // When inventory is set in InventoryHandler's _Ready()
        if (InventorySlotResource != null && InventorySlotResource.HasItem())
        {
            ItemStackResource itemStack = InventorySlotResource.GetItem();
            SetTexture(itemStack.ItemData.Texture);
            SetStackSize(itemStack.Amount + "");
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
        base._GuiInput(@event); // Otherwise tooltip doesn't appear
        if (@event is not InputEventMouseButton eventMouseMotion) return;
        if (!eventMouseMotion.Pressed) return;
        if (eventMouseMotion.ButtonIndex == MouseButton.Left)
        {
            EmitSignal(SignalName.SlotClicked, InventorySlotResource.InventoryIndex);
        }
        else if (eventMouseMotion.ButtonIndex == MouseButton.Right)
        {
            EmitSignal(SignalName.SlotRightClicked, InventorySlotResource.InventoryIndex);
        }
    }

    public void OnMouseEntered()
    {
        if (InventorySlotResource == null || InventorySlotResource.IsEmpty()) return;
        _hovered = true;
        //EventBus.Publish(new InventoryItemStackOnHoverEvent(InventorySlotResource.GetItem()));
    }

    public void OnMouseExited()
    {
        if (!_hovered) return;
        //EventBus.Publish(new InventoryItemStackOnHoverEvent(null));
    }

    public override GodotObject _MakeCustomTooltip(string forText)
    {
        ItemTooltip t = TooltipScene.Instantiate<ItemTooltip>();
        t.SetItemStack(InventorySlotResource.GetItem());
        return t;
    }
}