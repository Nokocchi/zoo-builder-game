using System;
using System.Collections.Generic;
using static GlobalDataSingleton;

namespace ZooBuilder.globals.saveable;

public class GlobalDataJsonDTO
{

    public Dictionary<string, List<SettingEntryDto>> Settings { get; private init; } = new()
    {
        ["Gameplay"] =
        [
            new SettingEntryDto(KEY_MOUSE_Y_FLIPPED, BOOL_TYPE_NAME, false),
            new SettingEntryDto(KEY_HOTBAR_SCROLL_DIRECTION_FLIPPED, BOOL_TYPE_NAME, false),
            new SettingEntryDto(KEY_HIDE_MINIMAP, BOOL_TYPE_NAME, false),
            new SettingEntryDto(KEY_NORTH_FACING_MINIMAP, BOOL_TYPE_NAME, false),
            new SettingEntryDto(KEY_MOUSE_SENSITIVITY, FLOAT_TYPE_NAME, 50),
            new SettingEntryDto(KEY_BACKGROUND_MUSIC_AUDIO_VOLUME, FLOAT_TYPE_NAME, 100)
        ],
        ["Other"] = [new SettingEntryDto(KEY_SELECTED_LOCALE, STRING_TYPE_NAME, "en")]
    };
    
    private Dictionary<string, string> _categoryByKey;
    private Dictionary<string, SettingEntryDto> _entryByKey;
    
    public string GetCategory(string key)
    {
        EnsureLookupsBuilt();
        return _categoryByKey[key];
    }

    public SettingEntryDto GetEntry(string key)
    {
        EnsureLookupsBuilt();
        return _entryByKey[key];
    }
    
    private void EnsureLookupsBuilt()
    {
        if (_categoryByKey != null)
        {
            return;
        }

        _categoryByKey = new Dictionary<string, string>();
        _entryByKey = new Dictionary<string, SettingEntryDto>();

        foreach (KeyValuePair<string, List<SettingEntryDto>> categoryPair in Settings)
        {
            string category = categoryPair.Key;

            foreach (SettingEntryDto entry in categoryPair.Value)
            {
                _categoryByKey[entry.Key] = category;
                _entryByKey[entry.Key] = entry;
            }
        }
    }
}