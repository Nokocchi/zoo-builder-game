using Godot;
using System;

public partial class InventoryItemStack : Panel
{
    [Signal]
    public delegate void ItemStackPressedEventHandler(InventoryItemStack stack);

    private ItemStackResource _itemStackResource;
    //private InventorySingleton _inventorySingleton;

    public ItemStackResource ItemStackResource
    {
        get => _itemStackResource;
        set
        {
            _itemStackResource = value;
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
        _selectedStyle =
            ResourceLoader.Load<StyleBoxFlat>("res://ui/inventory/hotbar/item_stack_panel_theme_selected.tres");
        _unselectedStyle =
            ResourceLoader.Load<StyleBoxFlat>("res://ui/inventory/hotbar/item_stack_panel_theme_unselected.tres");
        InventorySingleton.Instance.SelectedHotbarSlotUpdated += OnSelectedHotbarSlotChanged;
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
        ItemStackResource.Amount -= 1;
        if (ItemStackResource.Amount <= 0)
        {
            InventorySingleton.Instance.RemoveStackFromInventory(InventoryIndex);
        }

        Render();
    }


    // Called by the button child, propagated as new signal that the inventory can listen to
    private void OnPressed()
    {
        EmitSignal(SignalName.ItemStackPressed, this);
    }
    
    private void OnSelectedHotbarSlotChanged(int oldIndex, int newIndex)
    {
        // This stack should now be highlighted
        if (InventoryIndex == newIndex)
        {
            AddThemeStyleboxOverride("panel", _selectedStyle);
        }
        else
        {
            // This stack should no longer be highlighted
            AddThemeStyleboxOverride("panel", _unselectedStyle);
        }
    }
}