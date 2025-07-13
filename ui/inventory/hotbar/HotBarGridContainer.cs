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
        OnInventoryUpdated();
    }


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void OnInventoryUpdated()
    {
        Array<Node> children = GetChildren();
        foreach (Node child in children)
        {
            child.QueueFree();
        }

        InventorySingleton inventorySingleton = InventorySingleton.Instance;
        List<ItemStackResource> inventory = inventorySingleton.Inventory;


        for (int i = 0; i < InventorySingleton.HotBarSize; i++)
        {
            ItemStackResource stackAtIndex = inventory[i];
            InventoryItemStack instance = _inventoryItemStackScene.Instantiate<InventoryItemStack>();

            if (stackAtIndex != null)
            {
                TextureRect itemIcon = instance.GetNode<TextureRect>("ItemIcon");
                Label stackSizeLabel = instance.GetNode<Label>("ItemIcon/StackSize");
                stackSizeLabel.Text = "" + stackAtIndex.Amount;
                itemIcon.Texture = stackAtIndex.ItemData.Texture;
            }

            AddChild(instance);
        }
    }
}