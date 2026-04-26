using System;
using System.Text.Json;

namespace ZooBuilder.ui.settings;

public interface ISetting
{
    string Key { get; }
    object GetValueUntyped();
    void LoadFromJson(JsonElement element);
    object SaveToJson();
}