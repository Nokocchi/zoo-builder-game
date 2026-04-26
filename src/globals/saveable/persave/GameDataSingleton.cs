using Godot;
using Godot.Collections;

namespace ZooBuilder.globals.saveable;

public class GameDataSingleton
{
    // TODO: save slots
    public static readonly string GAME_DATA_LOCATION = "user://game_data.json";

    public static GameData Instance { get; private set; }

    public static void LoadDataFromDisk()
    {
        Instance = GameData.LoadFromDisk();
    }

    public static void SaveToDisk()
    {
        /*_userInterface.ShowGameSaveText(SaveAction);
            return;

        void SaveAction()
        {
            Steam.StoreStats();
            GlobalObjectsContainer.Instance.GameData.Save();
        }
        */

        // Listen to GameShouldSaveEvent, or have the game save timer call SaveToDisk directly? How do I show a popup that stays for 5 seconds?
        // Fetch PlayerRotation, PlayerGlobalPosition etc. from the right places. Or should those places just update the GameData 60 times per second?
        Instance.SaveToDisk();
    }
}