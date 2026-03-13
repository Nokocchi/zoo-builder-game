using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Godot;
using ZooBuilder.globals;

public partial class InventorySingleton : Node, IInventory
{
    public static InventorySingleton Instance { get; private set; }

    // Update the inventory when new stacks are added, stacks are removed or stacks are swapped. Not for increment/decrement/split, as this is handled in the InventoryItemStack itself.
    [Signal]
    public delegate void InventoryUpdatedEventHandler();

    [Signal]
    public delegate void InventoryItemStackHeldEventHandler(HeldItem heldItem);

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
            EmitSignal(SignalName.InventoryItemStackHeld, HeldItem);
        }
        else
        {
            Inventory[index] = null;
            EmitSignal(SignalName.InventoryUpdated);
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
            EmitSignal(SignalName.InventoryUpdated);
            return true;
        }

        // We did not have an existing stack, and there was no empty slot. 
        if (firstEmptySlot < 0)
        {
            return false;
        }

        Inventory[firstEmptySlot] = itemStack;
        EmitSignal(SignalName.InventoryUpdated);
        return true;
    }

    public void ItemClicked(int itemIndex)
    {
        if (HeldItem != null)
        {
            InsertHeldItem(itemIndex);
            EmitSignal(SignalName.InventoryUpdated);
            EmitSignal(SignalName.InventoryItemStackHeld, HeldItem);
        }

        // We are not currently holding anything, but the slot we clicked does have an item. Pick it up
        else if (Inventory[itemIndex] != null)
        {
            HoldItem(itemIndex);
            EmitSignal(SignalName.InventoryUpdated);
            EmitSignal(SignalName.InventoryItemStackHeld, HeldItem);
        }
    }

    public void DropHeldItem()
    {
        if (HeldItem == null) return;
        OverworldItem.SpawnItemAndLaunchFromPlayer(HeldItem.ItemStackResource);
        HeldItem = null;
        EmitSignal(SignalName.InventoryItemStackHeld, HeldItem);
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
        if (itemOnInsertIndex == null)
        {
            Inventory[insertIntoIndex] = heldItemItemStackResource;
            HeldItem = null;
        }
        else
        {
            // We are inserting on a spot that already has an item
            if (Inventory[heldItemOriginatesFromInventoryIndex] == null)
            {
                Inventory[heldItemOriginatesFromInventoryIndex] = Inventory[insertIntoIndex];
                Inventory[insertIntoIndex] = HeldItem.ItemStackResource;
                HeldItem = null;
            }
            else
            {
                Inventory[insertIntoIndex] = HeldItem.ItemStackResource;
                HeldItem = new HeldItem(Inventory[insertIntoIndex], insertIntoIndex);
            }
        }


    }

    private void HoldItem(int index)
    {
        HeldItem = new HeldItem(Inventory[index], index);
        Inventory[index] = null;
    }
}