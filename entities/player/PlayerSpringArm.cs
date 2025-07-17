using Godot;
using System;
using ZooBuilder.globals;

public partial class PlayerSpringArm : SpringArm3D
{
	private Vector3 _rotation;
	private Camera3D _camera;
	private Vector2 _mouseRelative;
	private float _mouseSensitivityBaseline = 0.1f;
	private const float MouseSpeedScale = 100f;
	private SettingsSingleton _settings;
	private InventorySingleton _inventory;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_camera = GetNode<Camera3D>("PlayerCamera");
		_settings = SettingsSingleton.Load();
		_inventory = InventorySingleton.Instance;
		SpringLength = _camera.Position.Z;
		_rotation = RotationDegrees;
		Input.MouseMode = Input.MouseModeEnum.Captured;
		GlobalObjectsContainer.Instance.PlayerSpringArm = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_rotation.X += _mouseRelative.Y * (_settings.MouseUpDownFlipped ? -1 : 1);
		_rotation.Y += _mouseRelative.X;
		_rotation.X = Mathf.Clamp(_rotation.X, -70, -25);
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
