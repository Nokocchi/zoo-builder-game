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
        EventBus.Subscribe<OnInventoryUpdatedEvent>(OnInventoryUpdated);
        for (int i = firstSlot; i <= lastSlot; i++)
        {
            GD.Print(i);
            // TODO: Wouldn't it be better to have some kind of itemStackResource **always**, and an empty stack is represented by a null ItemDataResource, instead of the itemStackResource itself being null
            // That way, we don't need the InventoryUpdated event which completely rebuilds the inventory every time
            // InventoryItemStacks can then just re-render whenever their resource updates, making swaps very efficient
            // TODO: Fix the bug where inventory updates take effect in both hotbar and main inventory. It's probably OnInventoryUpdated()'s fault.
            InventoryItemStack stack = _inventoryItemStackScene.Instantiate<InventoryItemStack>();
            stack.InventoryIndex = i;
            stack.ItemStackPressed += (clickedSlot) => _inventorySingleton.ItemClicked(clickedSlot.InventoryIndex);
            stack.ItemStackRightClicked += (clickedSlot) => _inventorySingleton.ItemRightClicked(clickedSlot.InventoryIndex);
            AddChild(stack);
        }
        
        OnInventoryUpdated(null);
    }
    
    
    public void OnInventoryUpdated(OnInventoryUpdatedEvent e)
    {
        // First clear all data from hotbar
        Array<Node> children = GetChildren();
        foreach (Node child in children)
        {
            if (child is InventoryItemStack itemStack)
            {
                itemStack.ClearStackResource();
            }
        }
        
        GD.Print("Size of children: ", children.Count);
        
        List<ItemStackResource> inventory = _inventorySingleton.GetInventory();

        for (int i = firstSlot; i <= lastSlot; i++)
        {
            ItemStackResource stackAtIndex = inventory[i];
            InventoryItemStack slot = GetChild<InventoryItemStack>(i-firstSlot); // So that we always start counting children from 0
            GD.Print("Trying to set content of index ", i);
            slot.ItemStackResource = stackAtIndex;
        }
    }
    
}