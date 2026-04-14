using Godot;

namespace ZooBuilder.ui.settings;

public class Setting<T> : ISetting
{
    public string Key { get; }
    public T Value { get; set; }

    public Setting(string key, T defaultValue)
    {
        Key = key;
        Value = defaultValue;
    }

    public object GetValue() => Value;

    public void SetValue(object value)
    {
        Value = (T)value;
    }
}