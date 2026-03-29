using Godot;
using System;
using ZooBuilder.globals;

public partial class OverworldDropListenerPanel : Panel
{
    public void OnGuiInput(InputEvent @event)
    {
        if (@event is not InputEventMouseButton eventMouseButton || !eventMouseButton.IsPressed()) return;
        if (eventMouseButton.ButtonIndex == MouseButton.Left)
        {
            InventorySingleton.Instance.TossEntireHeldItemStack();
        }
        if (eventMouseButton.ButtonIndex == MouseButton.Right)
        {
            InventorySingleton.Instance.TossOneOfHeldItem();
        }
    }
}