using System;
using System.Text.Json;

namespace ZooBuilder.ui.settings;

public interface ISetting
{
    string Key { get; }
    object GetValue();
    void SaveNewValue(object newValue);
    void LoadFromJson(JsonElement element);
}