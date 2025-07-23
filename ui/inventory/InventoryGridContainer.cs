using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;
using ZooBuilder.globals;

public partial class InventoryGridContainer : GridContainer
{
    private PackedScene _inventoryItemStackScene;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _inventoryItemStackScene = GD.Load<PackedScene>("res://ui/inventory/inventory_item_stack.tscn");
        InventorySingleton.Instance.InventoryUpdated += OnInventoryUpdated;

        // Of all InventorySingleton.InventorySize slots in the inventory,
        // the first InventorySingleton.HotBarSize are rendered by the hotbar. The remaining are rendered here
        for (int i = 0; i < InventorySingleton.Instance.InventorySize - InventorySingleton.HotBarSize; i++)
        {
            InventoryItemStack slot = _inventoryItemStackScene.Instantiate<InventoryItemStack>();
            slot.InventoryIndex = i + InventorySingleton.HotBarSize;
            slot.ItemStackPressed += OnItemClicked;
            AddChild(slot);
        }

        OnInventoryUpdated();
    }

    private void OnItemClicked(InventoryItemStack slot)
    {
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
            if (child is InventoryItemStack slot)
            {
                slot.ClearStackResource();
            }
        }

        InventorySingleton inventorySingleton = InventorySingleton.Instance;
        List<ItemStackResource> inventory = inventorySingleton.Inventory;

        for (int i = 0; i < inventorySingleton.InventorySize - InventorySingleton.HotBarSize; i++)
        {
            ItemStackResource stackAtIndex = inventory[i + InventorySingleton.HotBarSize];
            InventoryItemStack slot = GetChild<InventoryItemStack>(i);

            if (stackAtIndex != null)
            {
                slot.ItemStackResource = stackAtIndex;
            }
        }
    }
}