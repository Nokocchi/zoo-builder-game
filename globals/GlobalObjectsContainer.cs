using Godot;

namespace ZooBuilder.globals;

public partial class GlobalObjectsContainer : Node
{
    // KISS = Keep it Singleton, Screwit!
    public static GlobalObjectsContainer Instance { get; private set; }
    public Player Player { get; set; }
    public PlayerSpringArm PlayerSpringArm { get; set; }
    public Main GameScene { get; set; }
    public Camera3D PlayerCamera { get; set; }

    public override void _EnterTree()
    {
        if (Instance != null)
        {
            QueueFree();
        }

        Instance = this;
    }
}