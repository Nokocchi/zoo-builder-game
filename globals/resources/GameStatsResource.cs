using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Godot;

[GlobalClass]
public partial class GameStatsResource : Resource
{
    [Export] public float DistanceWalked;

    public static GameStatsResource Load()
    {
        return ResourceLoader.Load<GameStatsResource>("res://globals/resources/GameStatsResource.tres");
    }
    
    public void Save()
    {
        ResourceSaver.Save(this, GetPath());
    }
}