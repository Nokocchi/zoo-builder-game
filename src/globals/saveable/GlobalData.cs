using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Godot;
using ZooBuilder.ui.settings;
using static GlobalDataSingleton;

namespace ZooBuilder.globals.saveable;

public class GlobalData
{
    // TODO: Find a way to define EVERYTHING here in one single spot.
    //  MinMax values for floats, categories for all keys, String representation for all keys, etc.
    // TODO: Provide a OnSaveCallback for each Setting so that the save handler can just call a list of OnSaveCallbacks when saving.  
    
    // <SettingsCategory, <settingsKey, Setting>>
    public readonly Dictionary<string, Dictionary<string, ISetting>> ActiveSettings = new();
    private readonly Dictionary<string, string> _settingsKeyCategoryMap = new();

    public GlobalData()
    {
        SetDataFromDto(new GlobalDataJsonDTO());
    }

    public GlobalData(GlobalDataJsonDTO dto)
    {
        SetDataFromDto(dto);
    }

    private void SetDataFromDto(GlobalDataJsonDTO dto)
    {
        foreach (KeyValuePair<string, List<SettingEntryDto>> categoryEntry in dto.Settings)
        {
            string settingsCategory = categoryEntry.Key;
            List<SettingEntryDto> entries = categoryEntry.Value;
            ActiveSettings[settingsCategory] = new Dictionary<string, ISetting>();

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
                    ActiveSettings[settingsCategory][key] = new Setting<bool>(key, (bool)convertedValue);
                }
                else if (type == FLOAT_TYPE_NAME)
                {
                    ActiveSettings[settingsCategory][key] = new Setting<float>(key, Convert.ToSingle(convertedValue));
                }
                else if (type == STRING_TYPE_NAME)
                {
                    ActiveSettings[settingsCategory][key] = new Setting<string>(key, (string)convertedValue);
                }
                else
                {
                    throw new Exception("Unsupported type: " + type);
                }

                _settingsKeyCategoryMap[key] = settingsCategory;
            }
        }
    }

    private GlobalDataJsonDTO AsJsonDto()
    {
        GlobalDataJsonDTO dto = new();

        foreach (KeyValuePair<string, Dictionary<string, ISetting>> categoryEntry in ActiveSettings)
        {
            string category = categoryEntry.Key;
            Dictionary<string, ISetting> settings = categoryEntry.Value;
            dto.Settings[category] = [];
            
            foreach (KeyValuePair<string, ISetting> settingEntry in settings)
            {
                string key = settingEntry.Key;
                ISetting setting = settingEntry.Value;
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

                SettingEntryDto entry = new(key, type, value);
                dto.Settings[category].Add(entry);
            }
        }

        return dto;
    }

    public T Get<T>(string key)
    {
        string category = _settingsKeyCategoryMap[key];
        ISetting setting = ActiveSettings[category][key];
        return (T) setting.GetValue();
    }

    public void Set<T>(string key, T value)
    {
        Setting<T> setting = new(key, value);
        string category = _settingsKeyCategoryMap[key];
        ActiveSettings[category][key] = setting;
    }
    
    public void SetSetting(ISetting setting)
    {
        string category = _settingsKeyCategoryMap[setting.Key];
        ActiveSettings[category][setting.Key] = setting;
    }

    public GlobalData GetCopy()
    {
        return new GlobalData(AsJsonDto());
    }

    public void SaveToDisk()
    {
        string serializedJson = JsonSerializer.Serialize(AsJsonDto());
        using FileAccess file = FileAccess.Open(SETTINGS_LOCATION, FileAccess.ModeFlags.Write);
        file.StoreString(serializedJson);
    }

    public static GlobalData LoadFromDisk()
    {
        using FileAccess file = FileAccess.Open(SETTINGS_LOCATION, FileAccess.ModeFlags.Read);
        if (file == null)
        {
            Error openError = FileAccess.GetOpenError();
            if (openError is Error.DoesNotExist or Error.FileNotFound)
            {
                // TODO: Handle other error cases
                return new GlobalData();
            }

            GD.Print(openError);
        }

        string json = file.GetAsText();
        GlobalDataJsonDTO loadedFromDisk = JsonSerializer.Deserialize<GlobalDataJsonDTO>(json);
        return new GlobalData(loadedFromDisk);
    }
}