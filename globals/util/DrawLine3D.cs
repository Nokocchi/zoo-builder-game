using Godot;

namespace ZooBuilder.util;

public partial class DrawLine3D : Node2D
{
    public static DrawLine3D Instance { get; private set; }
    private GodotObject _drawLine3D;

    public override void _EnterTree()
    {
        if (Instance != null)
        {
            QueueFree();
        }

        Instance = this;
        GDScript drawLine3DScript = GD.Load<GDScript>("res://globals/util/DrawLine3D.gd");
        _drawLine3D = (GodotObject)drawLine3DScript.New();
    }
    
    public void PrepareDebugLines(Main main)
    {
        main.AddChild((Node2D) _drawLine3D);
    }
    
    public void DrawDebugLines_Basis(Node3D node, double delta)
    {
        // Draw the basis of the node, at the location of the node
        _drawLine3D.Call("DrawLine", node.GlobalPosition, node.GlobalPosition+(3*node.GlobalBasis.X), new Color(1, 0, 0), delta);
        _drawLine3D.Call("DrawLine", node.GlobalPosition, node.GlobalPosition+(3*node.GlobalBasis.Y), new Color(0, 1, 0), delta);
        _drawLine3D.Call("DrawLine", node.GlobalPosition, node.GlobalPosition+(3*node.GlobalBasis.Z), new Color(0, 0, 1), delta);
    }
    
    
    
}