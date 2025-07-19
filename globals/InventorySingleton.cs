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

    [Export] public int InventorySize = 20;

    public const int HotBarSize = 8;
    
    public List<ItemStackResource> Inventory;

    public bool MenuOpen { get; set; }

    private int _selectedHotbarSlotIndex;

    private SettingsSingleton _settings;
    
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

    public override void _Ready()
    {
        _settings = SettingsSingleton.Load();
    }
    
    private void SetHotbarSlotIndex(int newIndex)
    {
        GD.Print("New selected index is ", newIndex);
        int oldIndex = _selectedHotbarSlotIndex;
        _selectedHotbarSlotIndex = Math.Abs(newIndex) % HotBarSize;
        EmitSignal(SignalName.SelectedHotbarSlotUpdated, oldIndex, _selectedHotbarSlotIndex);
    }

    public void AddItem(ItemStackResource itemStack)
    {
        ItemStackResource existingStack = null;
        int firstEmptySlot = -1;
        for (int i = 0; i < InventorySize; i++)
        {
            ItemStackResource stackAtIndex = Inventory[i];
            if (stackAtIndex != null && stackAtIndex.ItemData.ItemName == itemStack.ItemData.ItemName)
            {
                existingStack = stackAtIndex;
                break;
            }

            if (firstEmptySlot < 0 && stackAtIndex == null)
            {
                firstEmptySlot = i;
            }
        }

        if (existingStack != null)
        {
            GD.Print("Increase stack");
            existingStack.IncreaseStackSize(itemStack.Amount);
            EmitSignal(SignalName.InventoryUpdated);
            return;
        }

        if (firstEmptySlot < 0)
        {
            GD.Print("Inventory at max capacity");
            return;
        }

        GD.Print("Add new stack");
        Inventory.Insert(firstEmptySlot, itemStack);
        EmitSignal(SignalName.InventoryUpdated);
    }


}