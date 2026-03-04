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
    private int _selectedHotbarIndex;

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

        GetChild<InventoryItemStack>(_selectedHotbarIndex).HighlightSlot();
        OnInventoryUpdated();
        _settings = SettingsResource.Load();
        _globals = GlobalObjectsContainer.Instance;
    }

    
    public override void _Process(double delta)
    {
    }
    
    private void OnItemClicked(InventoryItemStack clickedSlot)
    { 
        _inventorySingleton.ItemClicked(clickedSlot.InventoryIndex);
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
            slot.ItemStackResource = stackAtIndex;
        }
    }

    // Change selected hotbar slot with mouse scrolling
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


            int previousIndex = _selectedHotbarIndex;
            if (wheelDown ^ _settings.HotbarScrollDirectionFlipped)
            {
                _selectedHotbarIndex = (_selectedHotbarIndex + 1) % InventorySingleton.HotBarSize;
            }

            if (wheelUp ^ _settings.HotbarScrollDirectionFlipped)
            {
                _selectedHotbarIndex = ((_selectedHotbarIndex - 1) + InventorySingleton.HotBarSize) % InventorySingleton.HotBarSize;
            }
            
            GetChild<InventoryItemStack>(previousIndex).RemoveHighlight();
            GetChild<InventoryItemStack>(_selectedHotbarIndex).HighlightSlot();
        }

        if (!@event.IsActionPressed("toss_single_item")) return;
        InventoryItemStack currentSelectedStack = GetChild<InventoryItemStack>(_selectedHotbarIndex);
        if (InventorySingleton.Instance.HeldItem != null || currentSelectedStack.ItemStackResource is not { Amount: >= 0 }) return;
        OverworldItem.SpawnItemAndLaunchFromPlayer(new ItemStackResource(currentSelectedStack.ItemStackResource.ItemData, 1));
        currentSelectedStack.DecrementRerenderAndRemoveIfZero();
    }
}