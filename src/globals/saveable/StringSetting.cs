#nullable enable
using System;
using System.Text.Json;
using Godot;

namespace ZooBuilder.ui.settings;

public class StringSetting : Setting<string>
{
    public StringSetting(string key, string value, Action<string>? onSaveCallback = null) : base(key, value, onSaveCallback)
    {
    }

    protected override string Deserialize(JsonElement element) => element.GetString()!;

    protected override object Serialize(string value) => value;
}