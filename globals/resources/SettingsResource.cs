using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Godot;

[GlobalClass]
public partial class SettingsResource : Resource
{
    [Export] public float MouseSensitivity = 50;
    [Export] public float BackgroundAudioVolume = 100;
    [Export] public bool MouseUpDownFlipped;
    [Export] public bool HotbarScrollDirectionFlipped;
    [Export] public bool HideMinimap;
    [Export] public bool NorthFacingMinimap;
    [Export] public bool SettingsOpen;

    public static SettingsResource Load()
    {
        return ResourceLoader.Load<SettingsResource>("res://globals/resources/SettingsResource.tres");
    }
    
    public void Save()
    {
        ResourceSaver.Save(this, GetPath());
    }
}