using Godot;

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
    private bool _initialized;
    
    
    public override void _Ready()
    {
        _itemIcon = GetNode<TextureRect>("%ItemIcon");
        _stackSizeLabel = GetNode<Label>("%StackSize");
        _selectedStyle = ResourceLoader.Load<StyleBoxFlat>("res://src/ui/inventory/hotbar/item_stack_panel_theme_selected.tres");
        _unselectedStyle = ResourceLoader.Load<StyleBoxFlat>("res://src/ui/inventory/hotbar/item_stack_panel_theme_unselected.tres");
        _initialized = true;
    }

    public void SetInventorySlotResource(InventorySlotResource slot)
    {
        InventorySlotResource = slot;
        slot.SlotContentChanged += Render;
        Render();
    }
    
    public override void _Process(double delta)
    {
        // Continuously adjust height if width changes (responsive)
        float width = Size.X;
        if (!Mathf.IsEqualApprox(CustomMinimumSize.Y, width))
        {
            CustomMinimumSize = new Vector2(CustomMinimumSize.X, width);
        }
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
        if (@event is not InputEventMouseButton eventMouseButton) return;
        if (!eventMouseButton.Pressed) return;
        if (eventMouseButton.ButtonIndex == MouseButton.Left)
        {
            EmitSignal(SignalName.SlotClicked, InventorySlotResource.InventoryIndex);
        }
        else if (eventMouseButton.ButtonIndex == MouseButton.Right)
        {
            EmitSignal(SignalName.SlotRightClicked, InventorySlotResource.InventoryIndex);
        }
    }

    public override GodotObject _MakeCustomTooltip(string forText)
    {
        return InventorySlotResource == null ? null : ItemTooltip.CreateWithData(InventorySlotResource.GetItem());
    }
}