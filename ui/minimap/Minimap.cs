using Godot;
using System;

public partial class Minimap : CanvasLayer
{
	private SettingsSingleton _settings;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_settings = SettingsSingleton.Load();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Visible = !_settings.HideMinimap;
	}
}
