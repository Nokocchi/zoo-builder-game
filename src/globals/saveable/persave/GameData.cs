using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Godot;
using static ZooBuilder.globals.saveable.GameDataSingleton;
using FileAccess = Godot.FileAccess;

namespace ZooBuilder.globals.saveable;

public class GameData
{
    public Vector3 PlayerGlobalPosition = Vector3.Zero;
    public Vector3 PlayerRotation = Vector3.Zero;
    public Vector3 CameraRotation = Vector3.Zero;
    public int GameTime;

    public List<InventorySlotResource> Inventory = Enumerable.Range(0, 24)
        .Select(i => new InventorySlotResource(i, null))
        .ToList();

    public void SaveToDisk(string saveSlotName)
    {
        if (!DirAccess.DirExistsAbsolute(GAME_DATA_LOCATION))
        {
            DirAccess.MakeDirRecursiveAbsolute(GAME_DATA_LOCATION);
        }

        string folderPath = $"{GAME_DATA_LOCATION}/{saveSlotName}";
        if (!DirAccess.DirExistsAbsolute(folderPath))
        {
            DirAccess.MakeDirRecursiveAbsolute(folderPath);
        }

        string serializedJson = JsonSerializer.Serialize(ToDto());
        long unixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        string filePath = $"{folderPath}/{unixTime}.json";
        using FileAccess file = FileAccess.Open(filePath, FileAccess.ModeFlags.Write);
        file.StoreString(serializedJson);
    }

    private GameDataV2 ToDto()
    {
        return new GameDataV2()
        {
            InventorySize = Inventory.Capacity,
            PlayerGlobalPosition = Vector3Dto.FromVector3(PlayerGlobalPosition),
            PlayerRotation = Vector3Dto.FromVector3(PlayerRotation),
            CameraRotation = Vector3Dto.FromVector3(CameraRotation),
            GameTime = GameTime,
            Inventory = InventorySlotDto.FromInventorySlotResource(Inventory)
        };
    }

    public void IncrementGameTime()
    {
        GameTime++;
        EventBus.Publish(new GameTimeUpdatedEvent(GameTime));
    }
}