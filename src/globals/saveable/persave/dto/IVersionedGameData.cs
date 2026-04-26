namespace ZooBuilder.globals.saveable;

public interface IVersionedGameData
{
    GameData ConvertToCurrentFormat();
}