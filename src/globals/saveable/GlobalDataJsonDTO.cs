using System;
using System.Collections.Generic;
using static GlobalDataSingleton;

namespace ZooBuilder.globals.saveable;

public class GlobalDataJsonDTO(Dictionary<string, List<SettingEntryDto>> settings)
{
    public Dictionary<string, List<SettingEntryDto>> Settings { get; private init; } = settings;
}