using System.Collections.Generic;

namespace ZooBuilder.globals.saveable;

public record GameDataV2 : IVersionedGameData
{
    public int Version { get; private set; } = 2;
    public int InventorySize { get; init; }
    public Vector3Dto PlayerGlobalPosition { get; init; }
    public Vector3Dto PlayerRotation { get; init; }
    public Vector3Dto CameraRotation { get; init; }
    public int GameTime { get; init; }
    public List<InventorySlotDto> Inventory { get; init; }

    public GameData ConvertToCurrentFormat()
    {
        return new GameData()
        {
            PlayerGlobalPosition = PlayerGlobalPosition.AsVector3(),
            PlayerRotation = PlayerRotation.AsVector3(),
            CameraRotation = CameraRotation.AsVector3(),
            GameTime = GameTime,
            Inventory = InventorySlotDto.AsInventorySlotResource(Inventory, InventorySize)
        };
    }
}