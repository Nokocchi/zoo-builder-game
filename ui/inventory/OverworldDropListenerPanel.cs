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
            InventorySingleton.Instance.RemoveStackFromInventory(InventorySingleton.Instance.HeldItemIndex);
            OverworldItem.SpawnItemAndLaunchFromPlayer(itemStack);
            InventorySingleton.Instance.ClearHeldItem();
        }
    }
}