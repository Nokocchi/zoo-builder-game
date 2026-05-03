using System;
using System.Collections.Generic;
using System.Linq;
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

    // Save slots are sorted with the slot containing the most recent save first.
    // The save file list is sorted with the most recent save first. 
    public static SortedDictionary<string, SortedList<long, GameData>> GetSortedListOfSavesForSaveSlots()
    {
        // Temporary storage including max timestamp per folder
        Dictionary<string, (long maxTimestamp, SortedList<long, GameData> saves)> temp = new();

        if (!DirAccess.DirExistsAbsolute(GAME_DATA_LOCATION))
            return new SortedDictionary<string, SortedList<long, GameData>>();

        using DirAccess baseDir = DirAccess.Open(GAME_DATA_LOCATION);
        if (baseDir == null)
            return new SortedDictionary<string, SortedList<long, GameData>>();

        baseDir.ListDirBegin();

        while (true)
        {
            string subfolderName = baseDir.GetNext();
            if (subfolderName == "")
                break;

            if (!baseDir.CurrentIsDir() || subfolderName == "." || subfolderName == "..")
                continue;

            string subfolderPath = $"{GAME_DATA_LOCATION}/{subfolderName}";

            using DirAccess subDir = DirAccess.Open(subfolderPath);
            if (subDir == null)
                continue;

            SortedList<long, GameData> sortedList = new(
                Comparer<long>.Create((x, y) => y.CompareTo(x)) // newest first
            );

            long maxTimestamp = long.MinValue;

            subDir.ListDirBegin();

            while (true)
            {
                string fileNameWithExtension = subDir.GetNext();
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

                if (timestamp > maxTimestamp)
                    maxTimestamp = timestamp;
            }

            subDir.ListDirEnd();

            if (sortedList.Count > 0)
            {
                temp[subfolderName] = (maxTimestamp, sortedList);
            }
            else
            {
                // Treat empty folders as lowest priority
                temp[subfolderName] = (long.MinValue, sortedList);
            }
        }

        baseDir.ListDirEnd();

        // Now sort folders by maxTimestamp DESC
        var ordered = temp
            .OrderByDescending(kvp => kvp.Value.maxTimestamp)
            .ToList();

        // Materialize into a SortedDictionary with a stable index-based comparer
        var result = new SortedDictionary<string, SortedList<long, GameData>>(
            Comparer<string>.Create((a, b) =>
            {
                int indexA = ordered.FindIndex(x => x.Key == a);
                int indexB = ordered.FindIndex(x => x.Key == b);
                return indexA.CompareTo(indexB);
            })
        );

        foreach (var kvp in ordered)
        {
            result[kvp.Key] = kvp.Value.saves;
        }

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