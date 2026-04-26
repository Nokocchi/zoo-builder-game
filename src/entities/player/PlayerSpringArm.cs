using Godot;
using ZooBuilder.globals;
using ZooBuilder.globals.saveable;
using static ZooBuilder.globals.saveable.GameDataSingleton;


public partial class PlayerSpringArm : SpringArm3D, ISaveableNode
{
	[Export] public int LookUpDownMin = -70;
	[Export] public int LookUpDownMax = -25;
	
	private Camera3D _camera;
	private Vector2 _mouseRelative;
	private float _mouseSensitivityBaseline = 0.3f;
	private const float MouseSpeedScale = 100f;
	private GlobalObjectsContainer _globals;
	
	// TODO: Instead of all nodes just winging it and setting their initial states in _ready
	//  Maybe it would be nicer with a IShouldInitializeWithSavedData interface that is called in a group or something?
	public override void _Ready()
	{
		_camera = GetNode<Camera3D>("PlayerCamera");
		SpringLength = _camera.Position.Z;
		Input.MouseMode = Input.MouseModeEnum.Captured;
		_globals = GlobalObjectsContainer.Instance;
		_globals.PlayerSpringArm = this;
		_globals.PlayerCamera = _camera;
		AddToGroup(SAVEABLE_NODE_GROUP);
	}

	
	public override void _Process(double delta)
	{
		Vector3 newRotation = RotationDegrees;
		newRotation.X += _mouseRelative.Y * (GlobalDataSingleton.MouseYFlipped ? -1 : 1);
		newRotation.Y += _mouseRelative.X;
		newRotation.X = Mathf.Clamp(newRotation.X, LookUpDownMin, LookUpDownMax);
		RotationDegrees = newRotation;
		_mouseRelative = new Vector2();
	}

	public override void _Input(InputEvent @event)
	{
		if (!UIManager.IsMenuOpen() && @event is InputEventMouseMotion eventMouseMotion)
		{
			float speed = (GlobalDataSingleton.MouseSensitivity / MouseSpeedScale) * _mouseSensitivityBaseline;
			_mouseRelative = -eventMouseMotion.Relative * speed;
		}
	}
	
	public void SaveTo(GameData data)
	{
		data.CameraRotation = RotationDegrees;
	}

	public void LoadFrom(GameData data)
	{
		RotationDegrees = data.CameraRotation;
	}
}
