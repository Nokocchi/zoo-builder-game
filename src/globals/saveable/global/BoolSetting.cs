#nullable enable
using System;
using System.Text.Json;
using Godot;

namespace ZooBuilder.ui.settings;

public sealed class BoolSetting : Setting<bool>
{

    public BoolSetting(string key, bool value, Action<bool>? onSaveCallback = null) : base(key, value, onSaveCallback)
    {
    }
    
    protected override bool Deserialize(JsonElement element)
        => element.GetBoolean();

    protected override object Serialize(bool value)
        => value;
}