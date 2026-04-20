namespace ZooBuilder.globals.saveable;

public class SettingEntryDto(string key, object value)
{
    public string Key { get; set; } = key;
    public object Value { get; set; } = value;
}