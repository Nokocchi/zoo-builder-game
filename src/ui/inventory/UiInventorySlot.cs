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

    public InventorySlotResource InventorySlotResource;
    private bool _selected;
    private bool _hovered;
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
        //Render();
    }

    public void SetInventorySlotResource(InventorySlotResource slot)
    {
        InventorySlotResource = slot;
        slot.StackSizeChanged += Render;
    }

    private void Render()
    {
        if (InventorySlotResource.HasItem())
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
    
    // TODO:   EventBus.Publish(new SelectedHotbarSlotChangedItemEvent(InventoryIndex));
    // Maybe when the item resource is switched out?

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
        EventBus.Publish(new InventoryItemStackOnHoverEvent(InventorySlotResource.GetItem()));
    }

    public void OnMouseExited()
    {
        if (!_hovered) return;
        EventBus.Publish(new InventoryItemStackOnHoverEvent(null));
    }
}