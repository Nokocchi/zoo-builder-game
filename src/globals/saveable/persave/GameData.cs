using System;
using System.IO;
using System.Text.Json;
using Godot;
using Godot.Collections;
using static ZooBuilder.globals.saveable.GameDataSingleton;
using FileAccess = Godot.FileAccess;

namespace ZooBuilder.globals.saveable;

public class GameData
{
    public int InventorySize = 24;
    public Vector3 PlayerGlobalPosition = Vector3.Zero;
    public Vector3 PlayerRotation = Vector3.Zero;
    public Vector3 CameraRotation = Vector3.Zero;
    public int GameTime;
    public Array<ItemStackResource> Inventory;

    public static GameData LoadFromDisk()
    {
        using FileAccess file = FileAccess.Open(GAME_DATA_LOCATION, FileAccess.ModeFlags.Read);
        if (file == null)
        {
            Error openError = FileAccess.GetOpenError();
            if (openError is Error.DoesNotExist or Error.FileNotFound)
            {
                // TODO: Handle other error cases
                return new GameData();
            }

            GD.Print(openError);
        }

        string json = file.GetAsText();
        int version = GetVersion(json);
        
        IVersionedGameData data = version switch
        {
            1 => JsonSerializer.Deserialize<GameDataV1>(json)!,
            _ => throw new Exception($"Unsupported save version: {version}")
        };

        return data.ConvertToCurrentFormat();
    }
    
    public void SaveToDisk()
    {
        string serializedJson = JsonSerializer.Serialize(ToDto());
        using FileAccess file = FileAccess.Open(GAME_DATA_LOCATION, FileAccess.ModeFlags.Write);
        file.StoreString(serializedJson);
    }

    private GameDataV1 ToDto()
    {
        return new GameDataV1()
        {
            InventorySize = InventorySize,
            PlayerGlobalPosition = Vector3Dto.FromVector3(PlayerGlobalPosition),
            PlayerRotation = Vector3Dto.FromVector3(PlayerRotation),
            CameraRotation = Vector3Dto.FromVector3(CameraRotation),
            GameTime = GameTime
            
        };
    }

    public static int GetVersion(string json)
    {
        using var doc = JsonDocument.Parse(json);

        if (!doc.RootElement.TryGetProperty("Version", out var versionProp))
            throw new Exception("Save file missing version");

        return versionProp.GetInt32();
    }
    
    public void IncrementGameTime()
    {
        GameTime++;
        EventBus.Publish(new GameTimeUpdatedEvent(GameTime));
    }
}