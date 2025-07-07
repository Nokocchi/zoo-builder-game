using Godot;
using System;

public partial class PlayerSpringArm : SpringArm3D
{
	[Export] public float MouseSensitivity { get; set; } = 0.1f;

	private Vector3 _rotation;
	private Camera3D _camera;
	private Vector2 _mouseRelative;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_camera = GetNode<Camera3D>("PlayerCamera");
		SpringLength = _camera.Position.Z;
		_rotation = RotationDegrees;
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_rotation.X += _mouseRelative.Y;
		_rotation.Y += _mouseRelative.X;
		_rotation.X = Mathf.Clamp(_rotation.X, -70, -25);
		RotationDegrees = _rotation;
		_mouseRelative = new Vector2(); ;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion eventMouseMotion)
			_mouseRelative = -eventMouseMotion.Relative * MouseSensitivity;
		else if (@event is InputEventKey eventKey)
		{
			if (eventKey.Pressed && eventKey.Keycode is Key.Escape)
			{
				Input.MouseMode = Input.MouseModeEnum.Visible;
			}
		}
		else if (@event is InputEventMouseButton eventMouseButton)
		{
			if (eventMouseButton.Pressed && eventMouseButton.ButtonIndex == MouseButton.Left)
			{
				Input.MouseMode = Input.MouseModeEnum.Captured;
			}
		}
	}
}
