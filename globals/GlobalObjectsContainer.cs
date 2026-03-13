using Godot;
using ZooBuilder.globals.resources;

namespace ZooBuilder.globals;

// Probably doesn't make a lot of sense to use Instance/singleton logic when this is a global node in Godot..
public partial class GlobalObjectsContainer : Node
{
    // KISS = Keep it Singleton, Screwit!
    public static GlobalObjectsContainer Instance { get; private set; }
    public Player Player { get; set; }
    public PlayerSpringArm PlayerSpringArm { get; set; }
    public Main GameScene { get; set; }
    public Camera3D PlayerCamera { get; set; }
    public HotBarGridContainer HotBarGridContainer { set; get; }
    public SettingsResource Settings { get; private set; }
    public GameData GameData { get; private set; }

    public override void _EnterTree()
    {
        if (Instance != null)
        {
            QueueFree();
        }

        Instance = this;
        GameData = GameData.Load();
        Settings = SettingsResource.Load();
    }
}