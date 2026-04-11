using Godot;
using ZooBuilder.globals;
using static ZooBuilder.globals.saveable.GlobalDataConstants;


public partial class PlayerSpringArm : SpringArm3D
{
	[Export] public int LookUpDownMin = -70;
	[Export] public int LookUpDownMax = -25;
	
	private Vector3 _rotation;
	private Camera3D _camera;
	private Vector2 _mouseRelative;
	private float _mouseSensitivityBaseline = 0.1f;
	private const float MouseSpeedScale = 100f;
	private GlobalObjectsContainer _globals;
	
	
	public override void _Ready()
	{
		_camera = GetNode<Camera3D>("PlayerCamera");
		SpringLength = _camera.Position.Z;
		Input.MouseMode = Input.MouseModeEnum.Captured;
		_globals = GlobalObjectsContainer.Instance;
		_globals.PlayerSpringArm = this;
		_globals.PlayerCamera = _camera;
		// TODO: Why do we have a _rotation variable?
		RotationDegrees = GlobalObjectsContainer.Instance.GameData.CameraRotation;
	}

	
	public override void _Process(double delta)
	{
		Vector3 newRotation = RotationDegrees;
		newRotation.X += _mouseRelative.Y * (GlobalData.MouseYFlipped ? -1 : 1);
		newRotation.Y += _mouseRelative.X;
		newRotation.X = Mathf.Clamp(newRotation.X, LookUpDownMin, LookUpDownMax);
		RotationDegrees = newRotation;
		_mouseRelative = new Vector2();
		GlobalObjectsContainer.Instance.GameData.CameraRotation = RotationDegrees;
	}

	public override void _Input(InputEvent @event)
	{
		if (!UIManager.IsMenuOpen() && @event is InputEventMouseMotion eventMouseMotion)
		{
			float speed = (GlobalData.MouseSensitivity / MouseSpeedScale) * _mouseSensitivityBaseline;
			_mouseRelative = -eventMouseMotion.Relative * speed;
		}
	}
}
