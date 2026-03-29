using Godot;
using ZooBuilder.globals;


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
	private IInventory _inventory;
	private GlobalObjectsContainer _globals;
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_camera = GetNode<Camera3D>("PlayerCamera");
		_settings = SettingsResource.Load();
		_inventory = InventorySingleton.Instance;
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
		newRotation.X += _mouseRelative.Y * (_settings.MouseUpDownFlipped ? -1 : 1);
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
			float speed = (_settings.MouseSensitivity / MouseSpeedScale) * _mouseSensitivityBaseline;
			_mouseRelative = -eventMouseMotion.Relative * speed;
		}
	}
}
