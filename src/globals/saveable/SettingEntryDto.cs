namespace ZooBuilder.globals.saveable;

public class SettingEntryDto(string key, string type, object value)
{
    public string Key { get; set; } = key;
    public string Type { get; set; } = type;
    public object Value { get; set; } = value;
}