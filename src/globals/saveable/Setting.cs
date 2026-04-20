#nullable enable
using System;
using System.Text.Json;
using Godot;

namespace ZooBuilder.ui.settings;

public abstract class Setting<T> : ISetting
{
    public string Key { get; }

    private T _value;
    private readonly Action<T>? _onSave;

    protected Setting(string key, T defaultValue, Action<T>? onSave = null)
    {
        Key = key;
        _value = defaultValue;
        _onSave = onSave;
    }

    public T GetValue() => _value;

    public object GetValueUntyped() => _value!;

    public void SaveNewValue(T value)
    {
        _value = value;
        _onSave?.Invoke(value);
    }

    public void LoadFromJson(JsonElement element)
    {
        SaveNewValue(Deserialize(element));
    }

    public object SaveToJson()
    {
        return Serialize(_value);
    }

    protected abstract T Deserialize(JsonElement element);
    protected abstract object Serialize(T value);
}