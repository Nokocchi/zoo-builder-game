using Godot;
using System;
using ZooBuilder.globals;

public partial class Minimap : CanvasLayer
{
	private SettingsSingleton _settings;
	private GlobalObjectsContainer _globals;
	private Node3D _cameraPivot;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_settings = SettingsSingleton.Load();
		_globals = GlobalObjectsContainer.Instance;
		_cameraPivot = GetNode<Node3D>("%MinimapCameraPivot");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Visible = !_settings.HideMinimap;
		_cameraPivot.Position = _globals.Player.Position;
		// This ensures that "up" on the minimap is always where the main player camera is looking. In other words, "up" on the minimap is always "forwards" for the player. 
		_cameraPivot.Rotation = new Vector3(_cameraPivot.Rotation.X, _globals.PlayerSpringArm.Rotation.Y, _cameraPivot.Rotation.Z);
	}
}
