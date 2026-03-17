using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Godot;
using ZooBuilder.globals;
using ZooBuilder.ui.inventory;

public partial class InventorySingleton : Node, IInventory
{
    public static IInventory Instance { get; private set; }

    // TODO Inv: Set inventory size via inventory resource
    [Export] public int InventorySize = 20;

    public const int HotBarSize = 8;

    public List<ItemStackResource> Inventory;

    // Should be kept in InventorySingleton so it can be saved.. Maybe?
    public HeldItem HeldItem { get; private set; }

    public bool MenuOpen { get; set; }

    public override void _EnterTree()
    {
        if (Instance != null)
        {
            QueueFree();
        }

        Instance = this;
        Inventory = Enumerable.Repeat<ItemStackResource>(null, InventorySize).ToList();
    }

    // Interface methods

    public void RemoveStackFromInventory(int index)
    {
        if (HeldItem?.OriginatesFromInventoryIndex == index)
        {
            HeldItem = null;
            EventBus.Publish(new InventoryItemStackHeldEvent(HeldItem));
        }
        else
        {
            Inventory[index] = null;
            EventBus.Publish(new OnInventoryUpdatedEvent());
        }
    }

    // Returns true if item could be added. False if item could not be added
    public bool TryAddItem(ItemStackResource itemStack)
    {
        ItemStackResource existingStack = null;
        int firstEmptySlot = -1;
        for (int i = 0; i < InventorySize; i++)
        {
            ItemStackResource stackAtIndex = Inventory[i];
            // We already have this kind of item in the inventory. Break early
            if (stackAtIndex != null && stackAtIndex.ItemData.ItemName == itemStack.ItemData.ItemName)
            {
                existingStack = stackAtIndex;
                break;
            }

            // Keep track of the first empty slot in the inventory, in case we don't already have an existing stack of the item being added
            if (firstEmptySlot < 0 && stackAtIndex == null)
            {
                firstEmptySlot = i;
            }
        }

        if (existingStack != null)
        {
            existingStack.IncreaseStackSize(itemStack.Amount);
            EventBus.Publish(new OnInventoryUpdatedEvent());
            return true;
        }

        // We did not have an existing stack, and there was no empty slot. 
        if (firstEmptySlot < 0)
        {
            return false;
        }

        Inventory[firstEmptySlot] = itemStack;
        EventBus.Publish(new OnInventoryUpdatedEvent());
        return true;
    }

    public void ItemClicked(int itemIndex)
    {
        if (HeldItem != null)
        {
            InsertHeldItem(itemIndex);
            EventBus.Publish(new OnInventoryUpdatedEvent());
            EventBus.Publish(new InventoryItemStackHeldEvent(HeldItem));
        }

        // We are not currently holding anything, but the slot we clicked does have an item. Pick it up
        else if (Inventory[itemIndex] != null)
        {
            HoldItem(itemIndex);
            EventBus.Publish(new OnInventoryUpdatedEvent());
            EventBus.Publish(new InventoryItemStackHeldEvent(HeldItem));
        }
    }

    public void ItemRightClicked(int itemIndex)
    {
        ItemStackResource clickedItem = Inventory[itemIndex];
        // Holds item
        if (HeldItem != null)
        {
            // Clicked is empty
            if (clickedItem == null)
            {
                Inventory[itemIndex] = new ItemStackResource(HeldItem.ItemStackResource.ItemData, 1);
                HeldItem.DecrementRerenderAndRemoveIfZero();
                EventBus.Publish(new OnInventoryUpdatedEvent());
                EventBus.Publish(new InventoryItemStackHeldEvent(HeldItem));
                return;
            }

            // Same as clicked
            if (clickedItem.ItemData.ItemName == HeldItem.ItemStackResource.ItemData.ItemName)
            {
                HeldItem.DecrementRerenderAndRemoveIfZero();
                clickedItem.IncreaseStackSize(1);
                EventBus.Publish(new OnInventoryUpdatedEvent());
                EventBus.Publish(new InventoryItemStackHeldEvent(HeldItem));
                return;
            }
            
            // Clicked is different item
            InsertHeldItem(itemIndex);
            EventBus.Publish(new OnInventoryUpdatedEvent());
            EventBus.Publish(new InventoryItemStackHeldEvent(HeldItem));
        }

        // We are not currently holding anything, but the slot we clicked does have an item. Split it, pick up half
        else if (clickedItem != null)
        {
            int removedAmount = clickedItem.SplitInHalf();
            if (removedAmount == 0)
            {
                removedAmount = 1;
                Inventory[itemIndex] = null;
            }

            // TODO: If we pick up an item with right click, then left-clicking on a different item seems to overwrite the clicked item's resource??
            ItemStackResource pickedUpItemStack = new ItemStackResource(clickedItem.ItemData, removedAmount);
            HeldItem = new HeldItem(pickedUpItemStack, itemIndex);
            EventBus.Publish(new OnInventoryUpdatedEvent());
            EventBus.Publish(new InventoryItemStackHeldEvent(HeldItem));
        }
    }

    public void DropHeldItem()
    {
        if (HeldItem == null) return;
        OverworldItem.SpawnItemAndLaunchFromPlayer(HeldItem.ItemStackResource);
        HeldItem = null;
        EventBus.Publish(new InventoryItemStackHeldEvent(HeldItem));
    }

    public List<ItemStackResource> GetInventory()
    {
        return Inventory;
    }

    public bool IsMenuOpen()
    {
        return MenuOpen;
    }

    public void SetMenuOpen(bool menuOpen)
    {
        MenuOpen = menuOpen;
    }

    public HeldItem GetHeldItem()
    {
        return HeldItem;
    }

    // Internals

    // TODO Inv: Clean up
    private void InsertHeldItem(int insertIntoIndex)
    {
        if (HeldItem == null)
        {
            return;
        }

        ItemStackResource heldItemItemStackResource = HeldItem.ItemStackResource;
        int heldItemOriginatesFromInventoryIndex = HeldItem.OriginatesFromInventoryIndex;
        ItemStackResource itemOnInsertIndex = Inventory[insertIntoIndex];

        // Insert into empty slot
        if (itemOnInsertIndex == null)
        {
            Inventory[insertIntoIndex] = heldItemItemStackResource;
            HeldItem = null;
        }
        else 
        // We are inserting on a spot that already has an item
        {
            // Same item, just merge
            if (itemOnInsertIndex.ItemData.ItemName == HeldItem.ItemStackResource.ItemData.ItemName)
            {
                itemOnInsertIndex.Amount += HeldItem.ItemStackResource.Amount;
                HeldItem = null;
                return;
            }

            // Not same item, but the spot where the held item came from is empty. Swap
            if (Inventory[heldItemOriginatesFromInventoryIndex] == null)
            {
                Inventory[heldItemOriginatesFromInventoryIndex] = Inventory[insertIntoIndex];
                Inventory[insertIntoIndex] = HeldItem.ItemStackResource;
                HeldItem = null;
            }
            else
            // Not same item, and the held item's original spot is occupied
            // Swap the clicked item and the held item, so we are now holding the clicked item
            {
                GD.Print("Swap!");
                GD.Print("Clicked on ", itemOnInsertIndex.ItemData.ItemName);
                GD.Print("Inserting ", HeldItem.ItemStackResource.ItemData.ItemName);
                Inventory[insertIntoIndex] = HeldItem.ItemStackResource;
                HeldItem = new HeldItem(itemOnInsertIndex, insertIntoIndex);
            }
        }
    }

    private void HoldItem(int index)
    {
        HeldItem = new HeldItem(Inventory[index], index);
        Inventory[index] = null;
    }
}