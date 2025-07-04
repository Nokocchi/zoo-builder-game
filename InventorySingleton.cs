using System.Collections.Generic;
using Godot;

namespace SquashtheCreeps3D;

public partial class InventorySingleton : Node
{
    public static InventorySingleton Instance { get; private set; }

    [Signal]
    public delegate void InventoryUpdatedEventHandler();

    [Export] public int InventorySize = 8;
    public readonly List<ItemStack> Inventory = [];

    public override void _EnterTree()
    {
        if (Instance != null)
        {
            QueueFree();
        }

        Instance = this;
    }

    public override void _Ready()
    {
    }

    public void AddItem(Item item)
    {
        // Item already is in inventory - increase stack size
        ItemStack existingItemStack = Inventory.Find(stack => stack.Item.ItemName == item.ItemName);
        if (existingItemStack != null)
        {
            GD.Print("Increase stack");
            existingItemStack.IncreaseStackSize(1);
        }
        // Item not in inventory. Check inventory size first. If at max capacity, don't pick up item
        else if (Inventory.Count >= InventorySize)
        {
            GD.Print("Stack too large");
            return;
        }
        // We didn't already have the item, and our inventory is not full. Add new item stack
        else
        {
            GD.Print("Add new stack");
            Inventory.Add(new ItemStack(item, 1));
        }

        //item.QueueFree();
        EmitSignal(SignalName.InventoryUpdated);
    }
}