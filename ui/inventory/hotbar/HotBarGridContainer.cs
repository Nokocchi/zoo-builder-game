using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;
using ZooBuilder.globals;

public partial class HotBarGridContainer : GridContainer
{
    private PackedScene _inventoryItemStackScene;
    private SettingsSingleton _settings;
    private InventorySingleton _inventorySingleton;
    private GlobalObjectsContainer _globals;

    public override void _Ready()
    {
        _inventoryItemStackScene = GD.Load<PackedScene>("res://ui/inventory/inventory_item_stack.tscn");
        _inventorySingleton = InventorySingleton.Instance;
        _inventorySingleton.InventoryUpdated += OnInventoryUpdated;
        _inventorySingleton.HighlightedSlotUpdated += OnHighlightedHotbarSlotUpdated;
        for (int i = 0; i < InventorySingleton.HotBarSize; i++)
        {
            InventoryItemStack hotbarSlot = _inventoryItemStackScene.Instantiate<InventoryItemStack>();
            hotbarSlot.InventoryIndex = i;
            hotbarSlot.Pressed += OnItemClicked(hotbarSlot);
            AddChild(hotbarSlot);
        }

        OnInventoryUpdated();
        SetSlotSelected(InventorySingleton.Instance.SelectedHotbarSlotIndex, true);
        _settings = SettingsSingleton.Load();
        _globals = GlobalObjectsContainer.Instance;
    }


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    // TODO: Refactor the hotbar and inventory so they don't need all this duplicated logic.
    private Action OnItemClicked(InventoryItemStack clickedSlot)
    {
        return () =>
        {
            GD.Print("Slot clicked");
            ItemStackResource currentlyHeldItemStackResource = _globals.MouseWithMarker.ItemStack.ItemStackResource;
            int currentlyHeldItemStackIndex = _globals.MouseWithMarker.ItemStack.InventoryIndex;

            // We are holding an item. We want to either drop it, if the clicked slot is empty, or swap items
            ItemStackResource clickedSlotItemResource = clickedSlot.ItemStackResource;
            if (currentlyHeldItemStackResource != null)
            {
                // Clicked slot is empty, drop item here
                if (clickedSlotItemResource == null)
                {
                    clickedSlot.ItemStackResource = currentlyHeldItemStackResource;
                }
                // Clicked slot has item, swap
                else
                {
                    clickedSlot.ItemStackResource =currentlyHeldItemStackResource;
                    InventoryItemStack slotItemCameFrom = GetChild<InventoryItemStack>(currentlyHeldItemStackIndex);
                    slotItemCameFrom.ItemStackResource = clickedSlotItemResource;
                }

                _globals.MouseWithMarker.ClearItemStack();
            }
            // We are not currently holding anything, but the slot we clicked does have an item. Pick it up
            else if (currentlyHeldItemStackResource == null && clickedSlotItemResource != null)
            {
                GD.Print("Hold item");
                GlobalObjectsContainer.Instance.MouseWithMarker.HoldItemStack(clickedSlot);
                clickedSlot.ItemStackResource = null;
            }

            _inventorySingleton.SetHotbarSlotIndex(clickedSlot.InventoryIndex);
        };
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

        InventorySingleton inventorySingleton = InventorySingleton.Instance;
        List<ItemStackResource> inventory = inventorySingleton.Inventory;

        for (int i = 0; i < InventorySingleton.HotBarSize; i++)
        {
            ItemStackResource stackAtIndex = inventory[i];
            InventoryItemStack slot = GetChild<InventoryItemStack>(i);

            if (stackAtIndex != null)
            {
                slot.ItemStackResource = stackAtIndex;
            }
        }

        SetSlotSelected(inventorySingleton.SelectedHotbarSlotIndex, true);
    }

    private void OnHighlightedHotbarSlotUpdated(int oldIndex, int newIndex)
    {
        SetSlotSelected(oldIndex, false);
        SetSlotSelected(newIndex, true);
    }

    private void SetSlotSelected(int index, bool shouldBeSelected)
    {
        // Not sure why Rider is complaining about this always being true
        if ((index >= 0) || (index <= InventorySingleton.HotBarSize))
        {
            InventoryItemStack slot = GetChild<InventoryItemStack>(index);
            slot.Selected(shouldBeSelected);
        }
    }

    public override void _Input(InputEvent @event)
    {
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
                newIndex = (_inventorySingleton.SelectedHotbarSlotIndex + 1) % InventorySingleton.HotBarSize;
            }

            if (wheelUp ^ _settings.HotbarScrollDirectionFlipped)
            {
                newIndex = ((_inventorySingleton.SelectedHotbarSlotIndex - 1) + InventorySingleton.HotBarSize) %
                           InventorySingleton.HotBarSize;
            }

            _inventorySingleton.SetHotbarSlotIndex(newIndex);
        }
    }
}