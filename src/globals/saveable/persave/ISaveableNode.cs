namespace ZooBuilder.globals.saveable;

public interface ISaveableNode
{
    void SaveTo(GameData data);
    void LoadFrom(GameData data);
}