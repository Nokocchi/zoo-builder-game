using Godot;
using System;
using ZooBuilder.globals;
using GameData = ZooBuilder.globals.saveable.GameData;

public partial class TimeKeeper : Node
{
    [Signal]
    public delegate void TimeUpdatedEventHandler(int gameTimeSeconds);

    private GameData _gameData;

    public override void _Ready()
    {
        _gameData = GlobalObjectsContainer.Instance.GameData;
    }

    // Connected to Child timer's timeout
    private void OnSecondPassed()
    {
        _gameData.GameTime++;
        EmitSignal(SignalName.TimeUpdated, _gameData.GameTime);
    }
}