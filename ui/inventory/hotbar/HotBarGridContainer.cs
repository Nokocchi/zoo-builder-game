using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;
using ZooBuilder.globals;

public partial class HotBarGridContainer : GridContainer
{
    private PackedScene _inventoryItemStackScene;
    private SettingsResource _settings;
    private InventorySingleton _inventorySingleton;
    private GlobalObjectsContainer _globals;

    public override void _Ready()
    {
        _inventoryItemStackScene = GD.Load<PackedScene>("res://ui/inventory/inventory_item_stack.tscn");
        _inventorySingleton = InventorySingleton.Instance;
        _inventorySingleton.InventoryUpdated += OnInventoryUpdated;
        for (int i = 0; i < InventorySingleton.HotBarSize; i++)
        {
            InventoryItemStack hotbarSlot = _inventoryItemStackScene.Instantiate<InventoryItemStack>();
            hotbarSlot.InventoryIndex = i;
            hotbarSlot.ItemStackPressed += OnItemClicked;
            AddChild(hotbarSlot);
        }

        OnInventoryUpdated();
        _inventorySingleton.SelectedHotbarSlotIndex = 0;
        _settings = SettingsResource.Load();
        _globals = GlobalObjectsContainer.Instance;
    }


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
    
    private void OnItemClicked(InventoryItemStack clickedSlot)
    {
        // We are holding an item. We want to either drop it, if the clicked slot is empty, or swap items
        ItemStackResource clickedSlotItemResource = clickedSlot.ItemStackResource;
        if (_inventorySingleton.HoldsItem)
        {
            int heldItemIndex = _inventorySingleton.HeldItemIndex;
            
            // Clicked slot is empty. 
            if (clickedSlotItemResource == null)
            {
                // We want to move our held item here if it's not the slot we are currently holding
                // TODO: Maybe just update MoveItem to not do anything if the indexes are the same?
                if (clickedSlot.InventoryIndex != heldItemIndex)
                {
                    _inventorySingleton.MoveItem(heldItemIndex, clickedSlot.InventoryIndex);
                }
            }
            // Clicked slot has item. Swap
            else
            {
                _inventorySingleton.SwapItems(clickedSlot.InventoryIndex, heldItemIndex);
            }
            
            // In any case, we are no longer holding an item. The item might have been moved or swapped, so we can no longer rely on the previously fetched heldItemIndex
            _inventorySingleton.ClearHeldItem();
        }
        // We are not currently holding anything, but the slot we clicked does have an item. Pick it up
        else if (clickedSlotItemResource != null)
        {
            GD.Print("Picked up ", clickedSlotItemResource.ItemData.ItemName, " ",  clickedSlot.InventoryIndex);
            _inventorySingleton.SetHeldItem(clickedSlot.InventoryIndex);
        }
    }

    public void OnInventoryUpdated()
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
        
        List<ItemStackResource> inventory = _inventorySingleton.Inventory;

        for (int i = 0; i < InventorySingleton.HotBarSize; i++)
        {
            ItemStackResource stackAtIndex = inventory[i];
            InventoryItemStack slot = GetChild<InventoryItemStack>(i);

            if (stackAtIndex is { BeingHeld: false })
            {
                slot.ItemStackResource = stackAtIndex;
            }
        }
    }

    // Change selected hotbar slot with mouse scrolling
    public override void _Input(InputEvent @event)
    {
        int currentHotBarIndex = _inventorySingleton.SelectedHotbarSlotIndex;
        if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.IsPressed())
        {
            bool wheelDown = eventMouseButton.ButtonIndex == MouseButton.WheelDown;
            bool wheelUp = eventMouseButton.ButtonIndex == MouseButton.WheelUp;

            // Player is not scrolling - end early
            if (!wheelDown && !wheelUp)
            {
                return;
            }


            int newIndex = 0;
            if (wheelDown ^ _settings.HotbarScrollDirectionFlipped)
            {
                newIndex = (currentHotBarIndex + 1) % InventorySingleton.HotBarSize;
            }

            if (wheelUp ^ _settings.HotbarScrollDirectionFlipped)
            {
                newIndex = ((currentHotBarIndex - 1) + InventorySingleton.HotBarSize) %
                           InventorySingleton.HotBarSize;
            }

            _inventorySingleton.SelectedHotbarSlotIndex = newIndex;
        }
        
        if (@event.IsActionPressed("toss_single_item"))
        {
            InventoryItemStack currentSelectedStack = GetChild<InventoryItemStack>(currentHotBarIndex);
            if (!InventorySingleton.Instance.HoldsItem && currentSelectedStack.ItemStackResource is { Amount: >= 0 })
            {
                // TODO: This needs a cleanup maybe? Some places we are calling methods on the itemstack node, some places on the ItemStackResource itself, and other times in the InventorySingleton. 
                OverworldItem.SpawnItemAndLaunchFromPlayer(new ItemStackResource(currentSelectedStack.ItemStackResource.ItemData, 1));
                GD.Print("Q from HotBar");
                currentSelectedStack.DecrementRerenderAndRemoveIfZero();
            }
        }
    }
}