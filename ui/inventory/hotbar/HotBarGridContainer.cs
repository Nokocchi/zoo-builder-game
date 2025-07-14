using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

public partial class HotBarGridContainer : GridContainer
{
    private PackedScene _inventoryItemStackScene;

    public override void _Ready()
    {
        _inventoryItemStackScene = GD.Load<PackedScene>("res://ui/inventory/inventory_item_stack.tscn");
        InventorySingleton.Instance.InventoryUpdated += OnInventoryUpdated;
        InventorySingleton.Instance.HighlightedSlotUpdated += OnHighlightedHotbarSlotUpdated;
        for (int i = 0; i < InventorySingleton.HotBarSize; i++)
        {
            InventoryItemStack hotbarSlot = _inventoryItemStackScene.Instantiate<InventoryItemStack>();
            AddChild(hotbarSlot);
        }

        OnInventoryUpdated();
        SetSlotFocus(InventorySingleton.Instance.SelectedHotbarSlotIndex, true);
    }


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
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
        
        SetSlotFocus(inventorySingleton.SelectedHotbarSlotIndex, true);
    }

    private void OnHighlightedHotbarSlotUpdated(int oldIndex, int newIndex)
    {
        SetSlotFocus(oldIndex, false);
        SetSlotFocus(newIndex, true);
    }

    // TODO: Don't use Button focus. Figure out a way to make a proper border
    private void SetSlotFocus(int index, bool shouldHaveFocus)
    {
        GD.Print("New focus: ", index, shouldHaveFocus);
        // Not sure why Rider is complaining about this always being true
        if ((index >= 0) || (index <= InventorySingleton.HotBarSize))
        {
            InventoryItemStack slot = GetChild<InventoryItemStack>(index);
            if (shouldHaveFocus)
            {
                slot.GrabFocus();
            }
            else
            {
                slot.ReleaseFocus();
            }
        }
    }


}