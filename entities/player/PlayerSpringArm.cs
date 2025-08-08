using Godot;
using System;
using ZooBuilder.globals;
using ZooBuilder.util;

public partial class PlayerSpringArm : SpringArm3D
{
	[Export] public int LookUpDownMin = -70;
	[Export] public int LookUpDownMax = -25;
	
	private Vector3 _rotation;
	private Camera3D _camera;
	private Vector2 _mouseRelative;
	private float _mouseSensitivityBaseline = 0.1f;
	private const float MouseSpeedScale = 100f;
	private SettingsResource _settings;
	private InventorySingleton _inventory;
	private GlobalObjectsContainer _globals;
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_camera = GetNode<Camera3D>("PlayerCamera");
		_settings = SettingsResource.Load();
		_inventory = InventorySingleton.Instance;
		SpringLength = _camera.Position.Z;
		_rotation = RotationDegrees;
		Input.MouseMode = Input.MouseModeEnum.Captured;
		_globals = GlobalObjectsContainer.Instance;
		_globals.PlayerSpringArm = this;
		_globals.PlayerCamera = _camera;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_rotation.X += _mouseRelative.Y * (_settings.MouseUpDownFlipped ? -1 : 1);
		_rotation.Y += _mouseRelative.X;
		_rotation.X = Mathf.Clamp(_rotation.X, LookUpDownMin, LookUpDownMax);
		RotationDegrees = _rotation;
		_mouseRelative = new Vector2();
	}

	public override void _Input(InputEvent @event)
	{
		if (!_inventory.MenuOpen && !_settings.SettingsOpen && @event is InputEventMouseMotion eventMouseMotion)
		{
			float speed = (_settings.MouseSensitivity / MouseSpeedScale) * _mouseSensitivityBaseline;
			_mouseRelative = -eventMouseMotion.Relative * speed;
		}
	}
}
