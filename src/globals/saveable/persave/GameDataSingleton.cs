using System;
using System.Collections.Generic;
using System.Text.Json;
using Godot;
using GodotSteam;

namespace ZooBuilder.globals.saveable;

[GlobalClass]
public partial class GameDataSingleton : Node
{
    public static readonly string GAME_DATA_LOCATION = "user://saves";
    public static readonly string SAVEABLE_NODE_GROUP = "saveable_node";

    public static GameData Data { get; private set; }
    public static GameDataSingleton Instance { get; private set; }

    public override void _EnterTree()
    {
        Instance = this;
    }

    public static void LoadDataFromInstanceIntoGame()
    {
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

    public static SortedList<long, GameData> GetSortedListOfSaves()
    {
        SortedList<long, GameData> sortedList = new();

        // 1. Check if directory exists
        if (!DirAccess.DirExistsAbsolute(GAME_DATA_LOCATION))
            return sortedList;

        using DirAccess dir = DirAccess.Open(GAME_DATA_LOCATION);
        if (dir == null)
            return sortedList;

        // Collect list of existing save files
        dir.ListDirBegin();
        while (true)
        {
            string fileNameWithExtension = dir.GetNext();

            // End of iteration - break!
            if (fileNameWithExtension == "")
                break;

            if (dir.CurrentIsDir() || !fileNameWithExtension.EndsWith(".json"))
                continue;

            string fileNameWithoutExt = fileNameWithExtension.GetBaseName();
            string fileFullPath = $"{GAME_DATA_LOCATION}/{fileNameWithExtension}";

            if (!long.TryParse(fileNameWithoutExt, out long timestamp))
                continue;

            using FileAccess file = FileAccess.Open(fileFullPath, FileAccess.ModeFlags.Read);
            GameData loadedSaveFile = LoadSaveFileFromSavedDirectoryUnchecked(file);
            sortedList.Add(timestamp, loadedSaveFile);
        }

        dir.ListDirEnd();

        return sortedList;
    }

    private static GameData LoadSaveFileFromSavedDirectoryUnchecked(FileAccess saveFile)
    {
        string json = saveFile.GetAsText();
        int version = GetVersion(json);

        IVersionedGameData data = version switch
        {
            1 => JsonSerializer.Deserialize<GameDataV1>(json)!,
            2 => JsonSerializer.Deserialize<GameDataV2>(json)!,
            _ => throw new Exception($"Unsupported save version: {version}")
        };

        return data.ConvertToCurrentFormat();
    }

    public static int GetVersion(string json)
    {
        using var doc = JsonDocument.Parse(json);

        if (!doc.RootElement.TryGetProperty("Version", out var versionProp))
            throw new Exception("Save file missing version");

        return versionProp.GetInt32();
    }

    public static void SetLoadedSaveFile(GameData selectedSave)
    {
        Data = selectedSave;
    }
}