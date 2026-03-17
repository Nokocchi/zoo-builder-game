using System.Collections.Generic;
using Godot;
using Godot.Collections;
using ZooBuilder.globals;

namespace ZooBuilder.ui.inventory;

public partial class InventoryHandler : GridContainer
{
    private PackedScene _inventoryItemStackScene;
    protected IInventory _inventorySingleton;
    protected int slotsCount;
    
    public override void _Ready()
    {
        _inventoryItemStackScene = GD.Load<PackedScene>("res://src/ui/inventory/inventory_item_stack.tscn");
        _inventorySingleton = InventorySingleton.Instance;
        EventBus.Subscribe<OnInventoryUpdatedEvent>(OnInventoryUpdated);
        GD.Print("SlotsCount: ", slotsCount);
        for (int i = 0; i < slotsCount; i++)
        {
            // TODO: How are the InventorySingleton's resources mapped to this container's children?
            // Maybe it is the OnInventoryUpdated listener that copies it in?
            // Wouldn't it be better to have resources always, and have them automatically update, instead of setting and clearing resources in the case of no item stack? 
            // Maybe the ItemStackResource should send a signal when it's changed, and only the containing inventory item stack will listen and react?
            // Then we can add the incremenr/decrement etc. as API methods in IInventory, and make the UI stacks be simple "dumb views/listeners"
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
            if (child is InventoryItemStack hotbarSlot)
            {
                hotbarSlot.ClearStackResource();
            }
        }
        
        List<ItemStackResource> inventory = _inventorySingleton.GetInventory();

        for (int i = 0; i < InventorySingleton.HotBarSize; i++)
        {
            ItemStackResource stackAtIndex = inventory[i];
            InventoryItemStack slot = GetChild<InventoryItemStack>(i);
            slot.ItemStackResource = stackAtIndex;
        }
    }
    
}