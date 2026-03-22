using Godot;
using System;
using ZooBuilder.globals;
using GameData = ZooBuilder.globals.saveable.GameData;

public partial class TimeKeeper : Node
{

    private GameData _gameData;

    public override void _Ready()
    {
        _gameData = GlobalObjectsContainer.Instance.GameData;
    }

    // Connected to Child timer's timeout
    private void OnSecondPassedSignalHandler()
    {
        _gameData.IncrementGameTime();
    }
}