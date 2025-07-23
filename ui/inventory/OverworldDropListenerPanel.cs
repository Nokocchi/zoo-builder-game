using Godot;
using System;
using ZooBuilder.globals;

public partial class OverworldDropListenerPanel : Panel
{
    public void OnGuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.IsPressed() &&
            eventMouseButton.ButtonIndex == MouseButton.Left && GlobalObjectsContainer.Instance.MouseWithMarker.HoldsItem)
        {
            InventoryItemStack itemStack = GlobalObjectsContainer.Instance.MouseWithMarker.ItemStackInstance;
            OverworldItem.SpawnItemAndLaunchFromPlayer(itemStack.ItemStackResource);
            InventorySingleton.Instance.RemoveStackFromInventory(itemStack.InventoryIndex);
            GlobalObjectsContainer.Instance.MouseWithMarker.ClearItemStack();
            itemStack.QueueFree();
        }
    }
}