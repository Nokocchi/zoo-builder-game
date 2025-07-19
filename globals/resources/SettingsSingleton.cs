using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Godot;

[GlobalClass]
public partial class SettingsSingleton : Resource
{
    [Export] public float MouseSensitivity = 50;
    [Export] public bool MouseUpDownFlipped;
    [Export] public bool HotbarScrollDirectionFlipped;
    [Export] public bool HideMinimap;
    [Export] public bool NorthFacingMinimap;
    [Export] public bool SettingsOpen;

    public static SettingsSingleton Load()
    {
        return ResourceLoader.Load<SettingsSingleton>("res://globals/resources/settings.tres");
    }
}