using System.Collections.Generic;
using Godot;
using Godot.Collections;
using SquashtheCreeps3D;

public partial class InventoryView : Control
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InventorySingleton.Instance.InventoryUpdated += OnInventoryUpdated;
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
            
        List<ItemStack> inventory = InventorySingleton.Instance.Inventory;
        int index = 0;
        foreach (ItemStack stack in inventory)
        {
            Label label = new Label();
            label.SetPosition(new Vector2(0, 20f * index));
            label.Text = $"{stack.Amount} {stack.Item.ItemName}";
            AddChild(label);
            index++;
        }
    }
}