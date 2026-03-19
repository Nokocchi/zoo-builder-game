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
    // Only these can publish events

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
        var clickedItem = Inventory[itemIndex];
        if (HeldItem != null)
        {
            InsertHeldItem(itemIndex);
            EventBus.Publish(new InventoryItemStackHeldEvent(HeldItem));
            EventBus.Publish(new OnInventoryUpdatedEvent());
            return;
        }

        if (clickedItem != null)
        {
            HoldItem(itemIndex);
            EventBus.Publish(new InventoryItemStackHeldEvent(HeldItem));
            EventBus.Publish(new OnInventoryUpdatedEvent());
        }
    }

    public void ItemRightClicked(int itemIndex)
    {
        var clickedItem = Inventory[itemIndex];
        if (HeldItem != null)
        {
            HandleRightClickWithHeldItem(itemIndex, clickedItem);
            EventBus.Publish(new InventoryItemStackHeldEvent(HeldItem)); // If ran out of held item, or clicked on item of different type causing a swap
            EventBus.Publish(new OnInventoryUpdatedEvent()); // If clicked on empty slot, or if clicked on item of different type causing a swap
            return;
        }

        if (clickedItem != null)
        {
            SplitStackAndHold(itemIndex, clickedItem);
            EventBus.Publish(new InventoryItemStackHeldEvent(HeldItem));
            EventBus.Publish(new OnInventoryUpdatedEvent()); // In case you pick up the last of that stack
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

    public void DecrementItem(int inventoryIndex)
    {
        ItemStackResource item = Inventory[inventoryIndex];
        if (item == null) return;
        
        // Item at index exists
        int newSize = item.DecrementStackSize();
        if (newSize != 0) return;
        
        // Item stack amount is 0
        Inventory[inventoryIndex] = null;
        EventBus.Publish(new OnInventoryUpdatedEvent());
    }

    public void DecrementHeldItem()
    {
        if (HeldItem == null) return;
        
        // Holds item
        int newSize = HeldItem.DecrementStackSize();
        if (newSize != 0) return;
        
        // Dropped last of held item
        HeldItem = null;
        EventBus.Publish(new InventoryItemStackHeldEvent(HeldItem));
    }

    // Internals
    
    private void HandleRightClickWithHeldItem(int index, ItemStackResource clickedItem)
    {
        if (clickedItem == null)
        {
            InsertOneOfHeldItem(index);
            return;
        }

        if (IsSameItem(clickedItem, HeldItem.ItemStackResource))
        {
            AddOneOfHeldItemToExistingStack(clickedItem);
            return;
        }

        // Behaves as a normal click -> Swap with clicked item
        InsertHeldItem(index);
    }

    private void InsertOneOfHeldItem(int index)
    {
        Inventory[index] = new ItemStackResource(HeldItem.ItemStackResource.ItemData, 1);
        DecrementHeldItem();
    }

    private void AddOneOfHeldItemToExistingStack(ItemStackResource targetStack)
    {
        targetStack.IncreaseStackSize(1);
        DecrementHeldItem();
    }

    private void SplitStackAndHold(int index, ItemStackResource stack)
    {
        int removedAmount = stack.SplitInHalf();

        if (removedAmount == 0)
        {
            removedAmount = 1;
            Inventory[index] = null;
        }

        HeldItem = new HeldItem(new ItemStackResource(stack.ItemData, removedAmount), index
        );
    }

    private bool IsSameItem(ItemStackResource a, ItemStackResource b)
    {
        return a.ItemData.ItemName == b.ItemData.ItemName;
    }
    
    private void InsertHeldItem(int index)
    {
        if (HeldItem == null) return;

        var heldStack = HeldItem.ItemStackResource;
        var originIndex = HeldItem.OriginatesFromInventoryIndex;
        var targetStack = Inventory[index];

        // Empty slot → move
        if (targetStack == null)
        {
            Inventory[index] = heldStack;
            HeldItem = null;
            return;
        }

        // Same item → merge
        if (IsSameItem(targetStack, heldStack))
        {
            targetStack.Amount += heldStack.Amount;
            HeldItem = null;
            return;
        }

        // Swap cases
        SwapItems(index, originIndex, targetStack, heldStack);
    }

    private void SwapItems(int targetIndex, int originIndex, ItemStackResource targetStack, ItemStackResource heldStack)
    {
        // Original slot empty → simple swap back
        if (Inventory[originIndex] == null)
        {
            Inventory[originIndex] = targetStack;
            Inventory[targetIndex] = heldStack;
            HeldItem = null;
            return;
        }

        // Full swap → now holding clicked item
        Inventory[targetIndex] = heldStack;
        HeldItem = new HeldItem(targetStack, targetIndex);
    }

    private void HoldItem(int index)
    {
        HeldItem = new HeldItem(Inventory[index], index);
        Inventory[index] = null;
    }
}