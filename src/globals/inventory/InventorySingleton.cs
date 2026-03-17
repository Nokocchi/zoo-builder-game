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
        var clickedItem = Inventory[itemIndex];
        if (HeldItem != null)
        {
            InsertHeldItem(itemIndex);
            PublishInventoryAndHeldItem();
            return;
        }

        if (clickedItem != null)
        {
            HoldItem(itemIndex);
            PublishInventoryAndHeldItem();
        }
    }

    public void ItemRightClicked(int itemIndex)
    {
        var clickedItem = Inventory[itemIndex];
        if (HeldItem != null)
        {
            HandleRightClickWithHeldItem(itemIndex, clickedItem);
            PublishInventoryAndHeldItem();
            return;
        }

        if (clickedItem != null)
        {
            SplitStackAndHold(itemIndex, clickedItem);
            PublishInventoryAndHeldItem();
        }
    }
    
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
        HeldItem.DecrementRerenderAndRemoveIfZero();
    }
    
    private void AddOneOfHeldItemToExistingStack(ItemStackResource targetStack)
    {
        targetStack.IncreaseStackSize(1);
        HeldItem.DecrementRerenderAndRemoveIfZero();
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

    public void DropHeldItem()
    {
        if (HeldItem == null) return;
        OverworldItem.SpawnItemAndLaunchFromPlayer(HeldItem.ItemStackResource);
        HeldItem = null;
        EventBus.Publish(new InventoryItemStackHeldEvent(HeldItem));
    }
    
    private bool IsSameItem(ItemStackResource a, ItemStackResource b)
    {
        return a.ItemData.ItemName == b.ItemData.ItemName;
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
    
    private void PublishInventoryAndHeldItem()
    {
        EventBus.Publish(new OnInventoryUpdatedEvent());
        EventBus.Publish(new InventoryItemStackHeldEvent(HeldItem));
    }
}