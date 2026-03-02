using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Godot;

public partial class InventorySingleton : Node
{
    public static InventorySingleton Instance { get; private set; }

    [Signal]
    public delegate void InventoryUpdatedEventHandler();

    [Signal]
    public delegate void SelectedHotbarSlotUpdatedEventHandler(int oldIndex, int newIndex);

    [Signal]
    public delegate void InventoryItemStackHeldEventHandler(int heldItemIndex);

    [Signal]
    public delegate void InventoryItemStackNoLongerHeldEventHandler(int heldItemIndex);

    // TODO: Set inventory size via inventory resource
    [Export] public int InventorySize = 20;

    public const int HotBarSize = 8;

    public List<ItemStackResource> Inventory;

    public bool HoldsItem => Inventory.Exists(stack => stack is { BeingHeld: true });
    public ItemStackResource HeldItem => Inventory.Find(stack => stack is { BeingHeld: true });
    public int HeldItemIndex => Inventory.FindIndex(stack => stack is { BeingHeld: true });
    public int HotBarSelectedItemIndex => Inventory.FindIndex(stack => stack is { HotbarItemSelected: true });

    public bool MenuOpen { get; set; }

    private int _selectedHotbarSlotIndex;

    public int SelectedHotbarSlotIndex
    {
        get => _selectedHotbarSlotIndex;
        set => SetHotbarSlotIndex(value);
    }

    public override void _EnterTree()
    {
        if (Instance != null)
        {
            QueueFree();
        }

        Instance = this;
        Inventory = Enumerable.Repeat<ItemStackResource>(null, InventorySize).ToList();
    }

    private void SetHotbarSlotIndex(int newIndex)
    {
        // Keep track of the index outside of the stackresource because we are not guaranteed to have a stackresource.
        // Could we keep track of "currently held item" the same way? 
        int oldIndex = _selectedHotbarSlotIndex;
        if (Inventory[newIndex] != null)
        {
            // TODO: Don't know if this works - remnant from old commit last year
            Inventory[newIndex].HotbarItemSelected = true;
        }

        if (Inventory[oldIndex] != null)
        {
            // TODO: Don't know if this works - remnant from old commit last year
            Inventory[oldIndex].HotbarItemSelected = false;
        }


        _selectedHotbarSlotIndex = Math.Abs(newIndex) % HotBarSize;
        EmitSignal(SignalName.SelectedHotbarSlotUpdated, oldIndex, _selectedHotbarSlotIndex);
    }

    public void RemoveStackFromInventory(int index)
    {
        if (Inventory[index].BeingHeld)
        {
            ClearHeldItem(index);
        }

        if (index == HotBarSelectedItemIndex)
        {
            // Small hack. Force the held item to rerender
            EmitSignal(SignalName.SelectedHotbarSlotUpdated, index, index);
        }

        Inventory[index] = null;
        EmitSignal(SignalName.InventoryUpdated);
    }

    // Returns true if item could be added. False if item could not be added
    public bool AddItem(ItemStackResource itemStack)
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

        // We picked up a new item. If it's the spot in the hotbar that's already selected, force a rerender of the held item
        if (firstEmptySlot < HotBarSize && firstEmptySlot == SelectedHotbarSlotIndex)
        {
            EmitSignal(SignalName.SelectedHotbarSlotUpdated, firstEmptySlot, firstEmptySlot);
        }

        Inventory[firstEmptySlot] = itemStack;
        EmitSignal(SignalName.InventoryUpdated);
        return true;
    }


    public void SetHeldItem(int heldItemIndex)
    {
        Inventory[heldItemIndex].BeingHeld = true;
        EmitSignal(SignalName.InventoryItemStackHeld, heldItemIndex);
        EmitSignal(SignalName.InventoryUpdated);
    }

    public void ClearHeldItem(int heldItemIndex)
    {
        if (heldItemIndex < 0 || heldItemIndex > Inventory.Count)
        {
            return;
        }

        Inventory[heldItemIndex].BeingHeld = false;
        EmitSignal(SignalName.InventoryItemStackNoLongerHeld, heldItemIndex);
        EmitSignal(SignalName.InventoryUpdated);
    }

    public void ClearHeldItem()
    {
        ClearHeldItem(HeldItemIndex);
    }

    public void SwapItems(int index1, int index2)
    {
        if (index1 == index2)
        {
            return;
        }

        (Inventory[index1], Inventory[index2]) = (Inventory[index2], Inventory[index1]);
        EmitSignal(SignalName.InventoryUpdated);
    }

    public void MoveItem(int fromIndex, int toIndex)
    {
        // TODO: Should I check for item being held?
        if (fromIndex == toIndex)
        {
            return;
        }

        Inventory[toIndex] = Inventory[fromIndex];
        Inventory[fromIndex] = null;
        EmitSignal(SignalName.InventoryUpdated);
    }
}