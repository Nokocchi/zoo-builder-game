#nullable enable
using System;
using System.Text.Json;
using Godot;

namespace ZooBuilder.ui.settings;

public class Setting<T> : ISetting
{
    public string Key { get; }
    public T Value { get; private set; }
    private readonly Action<T>? _onSaveCallback;

    public Setting(string key, T defaultValue, Action<T>? onSaveCallback = null)
    {
        Key = key;
        Value = defaultValue;
        _onSaveCallback = onSaveCallback;
    }

    public object GetValue() => Value;
    
    public void SaveNewValue(object newValue)
    {
        Value = (T)newValue;
        executeOnSaveCallback();
    }

    private void executeOnSaveCallback()
    {
        _onSaveCallback?.Invoke(Value);
    }
    
    public virtual void LoadFromJson(JsonElement element)
    {
        object value;

        if (typeof(T) == typeof(bool))
            value = element.GetBoolean();
        else if (typeof(T) == typeof(float))
            value = (float)element.GetDouble();
        else if (typeof(T) == typeof(string))
            value = element.GetString();
        else
            throw new Exception($"Unsupported type {typeof(T)}");

        SaveNewValue((T)value);
    }
}