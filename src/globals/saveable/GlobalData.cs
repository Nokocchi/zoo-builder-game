using System;
using System.Collections.Generic;
using ZooBuilder.ui.settings;
using static GlobalDataSingleton;

namespace ZooBuilder.globals.saveable;

public class GlobalData
{
    // TODO: In the GlobalDataJsonDTO, all settings are stored by category
    // Figure out if we want to do the same here, and build some lookup tables to know which category to use for each settings key
    // Or do we want to store the settings key as the key, and have a lookup table for which category to use for each settings key?
    // Keep in mind that we will use this class to build the UI, so it should be easy to build one category at a time, and also update this class one category at a time.
    // Each category will be saved at a time, so maybe Save() should provide a category key, and in that case it's okay to store the settings as Dictionary<categoryKey, Dictionary<settingsKey, ISetting>>?
    
    public Dictionary<string, ISetting> ActiveSettings { get; private init; }

    public static GlobalData FromJsonDto(GlobalDataJsonDTO dto)
    {
        Dictionary<string, ISetting> activeSettings = new Dictionary<string, ISetting>();

        foreach (KeyValuePair<string, List<SettingEntryDto>> categoryEntry in dto.Settings)
        {
            List<SettingEntryDto> entries = categoryEntry.Value;

            foreach (SettingEntryDto entry in entries)
            {
                string key = entry.Key;
                string type = entry.Type;
                object rawValue = entry.Value;

                object convertedValue;

                if (rawValue is System.Text.Json.JsonElement jsonElement)
                {
                    if (type == BOOL_TYPE_NAME)
                    {
                        convertedValue = jsonElement.GetBoolean();
                    }
                    else if (type == FLOAT_TYPE_NAME)
                    {
                        convertedValue = (float)jsonElement.GetDouble();
                    }
                    else if (type == STRING_TYPE_NAME)
                    {
                        convertedValue = jsonElement.GetString();
                    }
                    else
                    {
                        throw new Exception("Unsupported type: " + type);
                    }
                }
                else
                {
                    convertedValue = rawValue;
                }

                if (type == BOOL_TYPE_NAME)
                {
                    activeSettings[key] = new Setting<bool>(key, (bool)convertedValue);
                }
                else if (type == FLOAT_TYPE_NAME)
                {
                    activeSettings[key] = new Setting<float>(key, (float)convertedValue);
                }
                else if (type == STRING_TYPE_NAME)
                {
                    activeSettings[key] = new Setting<string>(key, (string)convertedValue);
                }
                else
                {
                    throw new Exception("Unsupported type: " + type);
                }
            }
        }

        return new GlobalData
        {
            ActiveSettings = activeSettings
        };
    }

    public GlobalDataJsonDTO AsJsonDto()
    {
        GlobalDataJsonDTO dto = new GlobalDataJsonDTO();

        foreach (KeyValuePair<string, ISetting> settingEntry in ActiveSettings)
        {
            string key = settingEntry.Key;
            ISetting setting = settingEntry.Value;

            string category = GetCategoryForKey(key);

            if (!dto.Settings.ContainsKey(category))
            {
                dto.Settings[category] = new List<SettingEntryDto>();
            }

            object value = setting.GetValue();
            string type;

            if (setting is Setting<bool>)
            {
                type = BOOL_TYPE_NAME;
            }
            else if (setting is Setting<float>)
            {
                type = FLOAT_TYPE_NAME;
            }
            else if (setting is Setting<string>)
            {
                type = STRING_TYPE_NAME;
            }
            else
            {
                throw new Exception("Unsupported setting type");
            }

            SettingEntryDto entry = new SettingEntryDto(key, type, value);
            dto.Settings[category].Add(entry);
        }

        return dto;
    }

    public T Get<T>(string key)
    {
        return ((Setting<T>)ActiveSettings[key]).Value;
    }

    public void Set<T>(string key, T value)
    {
        ((Setting<T>)ActiveSettings[key]).Value = value;
    }

    public GlobalData GetCopy()
    {
        GlobalData globalData = new()
        {
            ActiveSettings = new Dictionary<string, ISetting>(ActiveSettings)
        };
        return globalData;
    }
}