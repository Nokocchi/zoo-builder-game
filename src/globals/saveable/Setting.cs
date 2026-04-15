#nullable enable
using System;
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
    public void SetValue(object newValue) => Value = (T) newValue;
    
    public void executeOnSaveCallback()
    {
        _onSaveCallback?.Invoke(Value);
    }
}