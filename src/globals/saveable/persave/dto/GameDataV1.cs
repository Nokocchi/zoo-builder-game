namespace ZooBuilder.globals.saveable;

public class GameDataV1 : IVersionedGameData
{
    public int Version { get; private set; } = 1;
    public int InventorySize { get; init; }
    public Vector3Dto PlayerGlobalPosition { get; init; }
    public Vector3Dto PlayerRotation { get; init; }
    public Vector3Dto CameraRotation { get; init; }
    public int GameTime { get; init; }

    public GameData ConvertToCurrentFormat()
    {
        return new GameData()
        {
            InventorySize = InventorySize,
            PlayerGlobalPosition = PlayerGlobalPosition.AsVector3(),
            PlayerRotation = PlayerRotation.AsVector3(),
            CameraRotation = CameraRotation.AsVector3(),
            GameTime = GameTime
        };
    }
}