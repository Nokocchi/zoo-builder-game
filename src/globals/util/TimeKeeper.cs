using Godot;
using System;
using ZooBuilder.globals;
using ZooBuilder.globals.saveable;
using GameData = ZooBuilder.globals.saveable.GameData;

public partial class TimeKeeper : Node
{

    // Connected to Child timer's timeout
    private void OnSecondPassedSignalHandler()
    {
        GameDataSingleton.Instance.IncrementGameTime();
    }
}