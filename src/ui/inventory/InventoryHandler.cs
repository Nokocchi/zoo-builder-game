using System.Collections.Generic;
using Godot;
using Godot.Collections;
using ZooBuilder.globals;

namespace ZooBuilder.ui.inventory;

public partial class InventoryHandler : GridContainer
{
    private PackedScene _inventoryItemStackScene;
    protected IInventory _inventorySingleton;
    protected int firstSlot;
    protected int lastSlot;
    
    public override void _Ready()
    {
        _inventoryItemStackScene = GD.Load<PackedScene>("res://src/ui/inventory/inventory_item_stack.tscn");
        _inventorySingleton = InventorySingleton.Instance;
        for (int i = firstSlot; i <= lastSlot; i++)
        {
            // TODO: Wouldn't it be better to have some kind of itemStackResource **always**, and an empty stack is represented by a null ItemDataResource, instead of the itemStackResource itself being null
            // That way, we don't need the InventoryUpdated event which completely rebuilds the inventory every time
            // InventoryItemStacks can then just re-render whenever their resource updates, making swaps very efficient
            UiInventorySlot uiUiInventorySlot = _inventoryItemStackScene.Instantiate<UiInventorySlot>();
            uiUiInventorySlot.SlotClicked += (clickedSlotIndex) => _inventorySingleton.ItemClicked(clickedSlotIndex);
            uiUiInventorySlot.SlotRightClicked += (clickedSlotIndex) => _inventorySingleton.ItemRightClicked(clickedSlotIndex);
            AddChild(uiUiInventorySlot);
        }
    }
    
}