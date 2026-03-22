using Godot;
using System;
using ZooBuilder.globals;

public partial class Menu : Control
{
    public override void _Ready()
    {
        Visible = false;
    }


    public override void _Input(InputEvent @event)
    {
        IInventory inventorySingleton = InventorySingleton.Instance;
        if (@event.IsActionPressed("open_inventory"))
        {
            inventorySingleton.SetMenuOpen(!inventorySingleton.IsMenuOpen());
            Visible = !Visible;
            Input.MouseMode = Visible ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
            //InventorySingleton.Instance.TossEntireHeldItemStack();
        }
    }
}
