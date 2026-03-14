using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;
using ZooBuilder.globals;

public partial class Inventory : CanvasLayer
{
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public override void _Input(InputEvent @event)
	{
		IInventory inventorySingleton = InventorySingleton.Instance;
		if (@event.IsActionPressed("open_inventory"))
		{
			inventorySingleton.SetMenuOpen(!inventorySingleton.IsMenuOpen());
			Visible = !Visible;
			Input.MouseMode = Visible ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
			InventorySingleton.Instance.DropHeldItem();
		}
	}
	
	
}
