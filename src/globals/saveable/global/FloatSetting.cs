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
    
    protected override float Deserialize(JsonElement element)
        => (float)element.GetDouble();

    protected override object Serialize(float value)
        => value;
}