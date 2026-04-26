using Godot;
using Godot.Collections;

namespace ZooBuilder.globals.saveable;

[GlobalClass]
public partial class GameData : Resource
{
    private const string SaveFileLocation = "user://GameData.tres";

    // TODO: Use ConfigFile?? 
    [Export] public int InventorySize = 24;
    [Export] public Vector3 PlayerGlobalPosition = Vector3.Zero;
    [Export] public Vector3 PlayerRotation = Vector3.Zero;
    [Export] public Vector3 CameraRotation = Vector3.Zero;
    [Export] public int GameTime;
    [Export] public Array<ItemStackResource> Inventory;

    public static GameData Load()
    {
        bool fileExists = ResourceLoader.Exists(SaveFileLocation);
        if (!fileExists)
        {
            new GameData().Save();
        }

        return ResourceLoader.Load<GameData>(SaveFileLocation);
    }

    public void Save()
    {
        ResourceSaver.Save(this, SaveFileLocation);
    }

    public void IncrementGameTime()
    {
        GameTime++;
        EventBus.Publish(new GameTimeUpdatedEvent(GameTime));
    }
}