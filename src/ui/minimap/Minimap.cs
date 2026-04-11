using Godot;
using System;
using ZooBuilder.globals;

public partial class Minimap : Control
{
    private GlobalObjectsContainer _globals;
    private Node3D _cameraPivot;


    public override void _Ready()
    {
        _globals = GlobalObjectsContainer.Instance;
        _cameraPivot = GetNode<Node3D>("%MinimapCameraPivot");
    }

    
    public override void _Process(double delta)
    {
        Visible = !GlobalData.HideMinimap;
        _cameraPivot.Position = _globals.Player.Position;
        
        if (!GlobalData.NorthFacingMinimap)
        {
            // This ensures that "up" on the minimap is always where the main player camera is looking. In other words, "up" on the minimap is always "forwards" for the player. 
            _cameraPivot.Rotation = new Vector3(_cameraPivot.Rotation.X, _globals.PlayerSpringArm.Rotation.Y,
                _cameraPivot.Rotation.Z);
        }
    }
}