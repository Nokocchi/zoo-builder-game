using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Godot;
using ZooBuilder.globals;
using ZooBuilder.ui.inventory;

// Strategy: All changes to inventory data must be made from IInventory
// Examples are stack count, which items are in which slot, etc.
// All inventory slots contain an InventorySlotResource which is never empty
// The contents of an InventorySlotResource can be updated, in which case a signal is emitted, so that
// Any visual node containing that slot resource can update accordingly
// Only InventorySingleton should call the modifying methods in InventorySlotResource.
// Events are only published from public functions, and public functions do not call each other. This will cause some unnecessary events,
// (Or maybe in the future I would want an event to be published only at the correct time, which risks multiple of the same events being published in a single API call)
public partial class InventorySingleton : Node, IInventory
{
    public static IInventory Instance { get; private set; }

    // TODO Inv: Set inventory size via inventory resource
    [Export] public int InventorySize = 20;

    public const int HotBarSize = 8;

    public List<InventorySlotResource> Inventory = [];

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
        for (int i = 0; i < InventorySize; i++)
        {
            Inventory.Add(new InventorySlotResource(i, null));
        }
    }

    // Interface methods
    // Only these can publish events

    // Returns true if item could be added. False if item could not be added
    public bool TryAddItem(ItemStackResource itemStack)
    {
        InventorySlotResource slotWithStackOfSameType = null;
        int firstEmptySlot = -1;
        for (int i = 0; i < InventorySize; i++)
        {
            InventorySlotResource slot = Inventory[i];
            // We already have this kind of item in the inventory. Break early
            if (slot.HasItem() && slot.GetItem().ItemData.ItemName == itemStack.ItemData.ItemName)
            {
                slotWithStackOfSameType = slot;
                break;
            }

            // Keep track of the first empty slot in the inventory, in case we don't already have an existing stack of the item being added
            if (firstEmptySlot < 0 && !slot.HasItem())
            {
                firstEmptySlot = i;
            }
        }

        if (slotWithStackOfSameType != null)
        {
            slotWithStackOfSameType.IncreaseStackSize(itemStack.Amount);
            EventBus.Publish(new OnInventoryUpdatedEvent());
            return true;
        }

        // We did not have an existing stack, and there was no empty slot. 
        if (firstEmptySlot < 0)
        {
            return false;
        }

        // Let's insert the item into the first empty slot
        Inventory[firstEmptySlot].SetItemStack(itemStack);
        EventBus.Publish(new OnInventoryUpdatedEvent());
        return true;
    }

    public void ItemClicked(int itemIndex)
    {
        InventorySlotResource clickedSlot = Inventory[itemIndex];
        if (HeldItem != null)
        {
            InsertHeldItem(itemIndex);
            EventBus.Publish(new InventoryItemStackHeldEvent(HeldItem));
            EventBus.Publish(new OnInventoryUpdatedEvent());
            return;
        }

        if (clickedSlot.HasItem())
        {
            HoldItem(itemIndex);
            EventBus.Publish(new InventoryItemStackHeldEvent(HeldItem));
            EventBus.Publish(new OnInventoryUpdatedEvent());
        }
    }

    public void ItemRightClicked(int itemIndex)
    {
        InventorySlotResource slot = Inventory[itemIndex];
        if (HeldItem != null)
        {
            HandleRightClickWithHeldItem(itemIndex, slot);
            EventBus.Publish(new InventoryItemStackHeldEvent(HeldItem)); // If ran out of held item, or clicked on item of different type causing a swap
            EventBus.Publish(new OnInventoryUpdatedEvent()); // If clicked on empty slot, or if clicked on item of different type causing a swap
            return;
        }

        if (slot.HasItem())
        {
            SplitStackAndHold(itemIndex, slot);
            EventBus.Publish(new InventoryItemStackHeldEvent(HeldItem));
            EventBus.Publish(new OnInventoryUpdatedEvent()); // In case you pick up the last of that stack
        }
    }

    public void TossEntireHeldItemStack()
    {
        if (HeldItem == null) return;
        OverworldItem.SpawnItemAndLaunchFromPlayer(HeldItem.GetItemStack());
        HeldItem = null;
        EventBus.Publish(new InventoryItemStackHeldEvent(HeldItem));
    }

    public List<InventorySlotResource> GetInventory()
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

    public void TossOneOfHeldItem()
    {
        if (HeldItem == null) return;
        OverworldItem.SpawnItemAndLaunchFromPlayer(new ItemStackResource(HeldItem.GetItemStack().ItemData, 1));
        DecrementHeldItem();
        EventBus.Publish(new InventoryItemStackHeldEvent(HeldItem));
    }

    public void TossOneOfItem(int index)
    {
        OverworldItem.SpawnItemAndLaunchFromPlayer(new ItemStackResource(Inventory[index].GetItem().ItemData, 1));
        DecrementItem(index);
        EventBus.Publish(new OnInventoryUpdatedEvent()); 
    }

    // Internals
    
    private void DecrementItem(int inventoryIndex)
    {
        InventorySlotResource slot = Inventory[inventoryIndex];
        if (slot.IsEmpty()) return;
        
        // Item at index exists
        int newSize = slot.DecrementStackSize();
        if (newSize != 0) return;
        
        // Item stack amount is 0
        slot.ClearStack();
    }
    
    private void DecrementHeldItem()
    {
        if (HeldItem == null) return;
        
        // Holds item
        int newSize = HeldItem.DecrementStackSize();
        if (newSize != 0) return;
        
        // Dropped last of held item
        HeldItem = null;
    }
    
    private void HandleRightClickWithHeldItem(int index, InventorySlotResource clickedSlot)
    {
        if (clickedSlot.IsEmpty())
        {
            InsertOneOfHeldItemInEmptySlot(index);
            return;
        }

        ItemStackResource clickedItemStack = clickedSlot.GetItem();
        if (IsSameItem(clickedItemStack, HeldItem.GetItemStack()))
        {
            AddOneOfHeldItemToExistingStack(clickedSlot);
            return;
        }

        // Behaves as a normal click -> Swap with clicked item
        InsertHeldItem(index);
    }

    private void InsertOneOfHeldItemInEmptySlot(int index)
    {
        Inventory[index].SetItemStack(new ItemStackResource(HeldItem.GetItemStack().ItemData, 1));
        DecrementHeldItem();
    }

    private void AddOneOfHeldItemToExistingStack(InventorySlotResource targetSlot)
    {
        targetSlot.IncreaseStackSize(1);
        DecrementHeldItem();
    }

    private void SplitStackAndHold(int index, InventorySlotResource clickedSlot)
    {
        ItemDataResource itemData = clickedSlot.GetItem().ItemData;
        int removedAmount = clickedSlot.SplitInHalf();

        if (removedAmount == 0)
        {
            // TODO: Should the clicked slot just handle this internally? Or does it go against keeping logic in this singleton?
            removedAmount = 1;
            Inventory[index].ClearStack();
        }

        HeldItem = new HeldItem(new ItemStackResource(itemData, removedAmount), clickedSlot.InventoryIndex);
    }

    private bool IsSameItem(ItemStackResource a, ItemStackResource b)
    {
        return a.ItemData.ItemName == b.ItemData.ItemName;
    }
    
    private void InsertHeldItem(int insertIntoIndex)
    {
        if (HeldItem == null) return;

        ItemStackResource heldStack = HeldItem.SlotResource.GetItem();
        InventorySlotResource targetSlot = Inventory[insertIntoIndex];

        // Empty slot → insert and stop holding item
        if (targetSlot.IsEmpty())
        {
            targetSlot.SetItemStack(heldStack);
            HeldItem = null;
            return;
        }

        // Same item → merge
        if (IsSameItem(targetSlot.GetItem(), heldStack))
        {
            targetSlot.IncreaseStackSize(heldStack.Amount);
            HeldItem = null;
            return;
        }

        // Occupied slot with different item - swap with held item
        SwapInventoryItemWithHeldItem(insertIntoIndex);
    }

    private void SwapInventoryItemWithHeldItem(int targetIndex)
    {
        // If the held item's original slot is still empty, move the clicked item there. Drop held item on clicked slot
        InventorySlotResource heldItemOriginSlot = Inventory[HeldItem.GetOriginatesFromIndex()];
        InventorySlotResource clickedSlot = Inventory[targetIndex];
        ItemStackResource heldItemStack = HeldItem.GetItemStack();
        ItemStackResource clickedSlotStack = clickedSlot.GetItem();
        if (heldItemOriginSlot.IsEmpty())
        {
            heldItemOriginSlot.SetItemStack(clickedSlotStack);
            clickedSlot.SetItemStack(heldItemStack);
            HeldItem = null;
            return;
        }

        // The held item's original slot is occupied. Insert held item into clicked slot, and hold that item instead
        clickedSlot.SetItemStack(heldItemStack);
        HeldItem = new HeldItem(clickedSlotStack, targetIndex);
    }

    private void HoldItem(int index)
    {
        InventorySlotResource slotToHold = Inventory[index];
        HeldItem = new HeldItem(slotToHold.GetItem(), index);
        slotToHold.ClearStack();
    }
}