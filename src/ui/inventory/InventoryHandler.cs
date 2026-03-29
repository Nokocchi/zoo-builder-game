using Godot;
using ZooBuilder.globals;

namespace ZooBuilder.ui.inventory;

public partial class InventoryHandler : Control
{
    private PackedScene _inventoryItemStackScene;
    protected Container UiSlotContainer;
    protected IInventory InventorySingleton;
    protected int FirstSlot;
    protected int LastSlot;
    
    public override void _Ready()
    {
        _inventoryItemStackScene = GD.Load<PackedScene>("res://src/ui/inventory/ui_inventory_slot.tscn");
        UiSlotContainer = GetNode<Container>("%UISlotContainer");
        InventorySingleton = global::InventorySingleton.Instance;
        for (int i = FirstSlot; i <= LastSlot; i++)
        {
            UiInventorySlot uiInventorySlot = _inventoryItemStackScene.Instantiate<UiInventorySlot>();
            uiInventorySlot.SlotClicked += (clickedSlotIndex) => InventorySingleton.ItemClicked(clickedSlotIndex);
            uiInventorySlot.SlotRightClicked += (clickedSlotIndex) => InventorySingleton.ItemRightClicked(clickedSlotIndex);
            uiInventorySlot.SetInventorySlotResource(InventorySingleton.GetInventory()[i]);
            UiSlotContainer.AddChild(uiInventorySlot);
        }
    }
    
}