using System.Text.Json;

namespace ZooBuilder.globals.saveable;

public class SettingEntryDto(string key, JsonElement value)
{
    public string Key { get; set; } = key;
    public JsonElement Value { get; set; } = value;
}