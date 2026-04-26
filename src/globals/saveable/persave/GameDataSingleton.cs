using Godot;
using GodotSteam;

namespace ZooBuilder.globals.saveable;

[GlobalClass]
public partial class GameDataSingleton : Node
{
    
    public static readonly string GAME_DATA_LOCATION = "user://game_data.json";
    public static readonly string SAVEABLE_NODE_GROUP = "saveable_node";

    public static GameData Data { get; private set; }
    public static GameDataSingleton Instance { get; private set; }

    public override void _EnterTree()
    {
        Instance = this;
    }

    public static void LoadDataFromDisk()
    {
        Data = GameData.LoadFromDisk();
        foreach (var node in Instance.GetTree().GetNodesInGroup(SAVEABLE_NODE_GROUP))
        {
            if (node is ISaveableNode saveable)
            {
                saveable.LoadFrom(Data);
            }
        }
    }

    public static void SaveToDisk()
    {
        EventBus.Publish(new GameIsAboutToBeSavedEvent());
        foreach (var node in Instance.GetTree().GetNodesInGroup(SAVEABLE_NODE_GROUP))
        {
            if (node is ISaveableNode saveable)
            {
                saveable.SaveTo(Data);
            }
        }
        Data.SaveToDisk();
        Steam.StoreStats();
    }
}