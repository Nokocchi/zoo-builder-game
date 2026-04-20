#nullable enable
using System;
using System.Text.Json;
using Godot;

namespace ZooBuilder.ui.settings;

public class FloatSetting : Setting<float>
{
    public float MinValue { get; }
    public float MaxValue { get; }

    public FloatSetting(string key, float value, float minValue, float maxValue, Action<float>? onSaveCallback = null) : base(key, value, onSaveCallback)
    {
        MinValue = minValue;
        MaxValue = maxValue;
    }
    
    public override void LoadFromJson(JsonElement element)
    {
        SaveNewValue((float)element.GetDouble());
    }
}