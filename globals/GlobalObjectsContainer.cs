using Godot;

namespace ZooBuilder.globals;

public partial class GlobalObjectsContainer : Node
{
    
    public static GlobalObjectsContainer Instance { get; private set; }
    public Player Player { get; set; }
    public PlayerSpringArm PlayerSpringArm { get; set; }
    public MouseWithItemMarker MouseWithMarker { get; set; }
    
    public override void _EnterTree()
    {
        if (Instance != null)
        {
            QueueFree();
        }

        Instance = this;
    }
}