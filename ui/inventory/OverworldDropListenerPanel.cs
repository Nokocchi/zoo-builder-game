using Godot;
using System;
using ZooBuilder.globals;

public partial class OverworldDropListenerPanel : Panel
{
    public void OnGuiInput(InputEvent @event)
    {
        if (@event is not InputEventMouseButton eventMouseButton || !eventMouseButton.IsPressed() || eventMouseButton.ButtonIndex != MouseButton.Left) return;
        InventorySingleton.Instance.DropHeldItem();
    }
}