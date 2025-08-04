using Godot;
using System;
using ZooBuilder.globals;

public partial class OverworldDropListenerPanel : Panel
{
    public void OnGuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.IsPressed() &&
            eventMouseButton.ButtonIndex == MouseButton.Left && InventorySingleton.Instance.HoldsItem)
        {
            
            ItemStackResource itemStack = InventorySingleton.Instance.HeldItem;
            int heldItemIndex = InventorySingleton.Instance.HeldItemIndex;
            InventorySingleton.Instance.RemoveStackFromInventory(heldItemIndex);
            OverworldItem.SpawnItemAndLaunchFromPlayer(itemStack);
        }
    }
}