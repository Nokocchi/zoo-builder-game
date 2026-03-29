using System.Collections.Generic;
using Godot;
using Godot.Collections;
using ZooBuilder.globals;

namespace ZooBuilder.ui.inventory;

public partial class InventoryHandler : Container
{
    private PackedScene _inventoryItemStackScene;
    protected IInventory _inventorySingleton;
    protected int firstSlot;
    protected int lastSlot;
    
    public override void _Ready()
    {
        _inventoryItemStackScene = GD.Load<PackedScene>("res://src/ui/inventory/ui_inventory_slot.tscn");
        _inventorySingleton = InventorySingleton.Instance;
        for (int i = firstSlot; i <= lastSlot; i++)
        {
            UiInventorySlot uiInventorySlot = _inventoryItemStackScene.Instantiate<UiInventorySlot>();
            uiInventorySlot.SlotClicked += (clickedSlotIndex) => _inventorySingleton.ItemClicked(clickedSlotIndex);
            uiInventorySlot.SlotRightClicked += (clickedSlotIndex) => _inventorySingleton.ItemRightClicked(clickedSlotIndex);
            uiInventorySlot.SetInventorySlotResource(_inventorySingleton.GetInventory()[i]);
            AddChild(uiInventorySlot);
        }
    }
    
}