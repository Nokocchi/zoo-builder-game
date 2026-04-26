namespace ZooBuilder.globals.saveable;

public class GameDataV1 : IVersionedGameData
{
    public int Version = 1;
    public int InventorySize;
    public Vector3Dto PlayerGlobalPosition;
    public Vector3Dto PlayerRotation;
    public Vector3Dto CameraRotation;
    public int GameTime;
    
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