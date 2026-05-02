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
    public static string SaveSlotName { get; set; }

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

        Data.SaveToDisk(SaveSlotName);
        Steam.StoreStats();
    }

    public static Dictionary<string, SortedList<long, GameData>> GetSortedListOfSavesForSaveSlots()
    {
        Dictionary<string, SortedList<long, GameData>> result = new();

        // First boot
        if (!DirAccess.DirExistsAbsolute(GAME_DATA_LOCATION))
            return result;
        
        using DirAccess baseDir = DirAccess.Open(GAME_DATA_LOCATION);
        if (baseDir == null)
            return result;

        baseDir.ListDirBegin();

        while (true)
        {
            string subfolderName = baseDir.GetNext();

            // No more files or folders, stop traversing
            if (subfolderName == "")
                break;

            // Skip non-directories and "." / ".."
            if (!baseDir.CurrentIsDir() || subfolderName == "." || subfolderName == "..")
                continue;

            string subfolderPath = $"{GAME_DATA_LOCATION}/{subfolderName}";

            using DirAccess subDir = DirAccess.Open(subfolderPath);
            if (subDir == null)
                continue;

            SortedList<long, GameData> sortedList = new(Comparer<long>.Create((x, y) => y.CompareTo(x))); // Reversed, most recent (highest timestamp) first

            subDir.ListDirBegin();

            while (true)
            {
                string fileNameWithExtension = subDir.GetNext();

                // No more files or folders, stop traversing
                if (fileNameWithExtension == "")
                    break;

                if (subDir.CurrentIsDir() || !fileNameWithExtension.EndsWith(".json"))
                    continue;

                string fileNameWithoutExt = fileNameWithExtension.GetBaseName();
                string fileFullPath = $"{subfolderPath}/{fileNameWithExtension}";

                if (!long.TryParse(fileNameWithoutExt, out long timestamp))
                    continue;

                using FileAccess file = FileAccess.Open(fileFullPath, FileAccess.ModeFlags.Read);
                if (file == null)
                    continue;

                GameData loadedSaveFile = LoadSaveFileFromSavedDirectoryUnchecked(file);
                sortedList[timestamp] = loadedSaveFile; 
            }

            subDir.ListDirEnd();

            // Only add non-empty folders (optional; remove if you want empty ones too)
            if (sortedList.Count > 0)
                result[subfolderName] = sortedList;
        }

        baseDir.ListDirEnd();

        return result;
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