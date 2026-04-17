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
    // <SettingsCategory, <settingsKey, Setting>>
    private readonly Dictionary<string, (string, int)> _settingsKeyCategoryIndexMap = new();

    public readonly Dictionary<string, List<ISetting>> ActiveSettings = new()
    {
        ["Gameplay"] =
        [
            new Setting<bool>(KEY_MOUSE_Y_FLIPPED, false),
            new Setting<bool>(KEY_HOTBAR_SCROLL_DIRECTION_FLIPPED, false),
            new Setting<bool>(KEY_HIDE_MINIMAP, false),
            new Setting<bool>(KEY_NORTH_FACING_MINIMAP, false),
            new FloatSetting(KEY_MOUSE_SENSITIVITY, 50, 0, 100),
            new FloatSetting(KEY_BACKGROUND_MUSIC_AUDIO_VOLUME, 100, 0, 100, audioVolume =>
            {
                int audioBusIndexBgMusic = AudioServer.GetBusIndex("BgMusic");
                AudioServer.SetBusVolumeLinear(audioBusIndexBgMusic, audioVolume / 100);
            })
        ],
        ["Other"] = [new Setting<string>(KEY_SELECTED_LOCALE, "en", TranslationServer.SetLocale)]
    };

    public GlobalData()
    {
        Initialize();
    }

    public GlobalData(GlobalDataJsonDTO dto)
    {
        Initialize(dto);
    }

    private void Initialize(GlobalDataJsonDTO dto = null)
    {
        InitializeLookupTables();
        if (dto != null)
        {
            SetDataFromDto(dto);
        }
    }

    private void InitializeLookupTables()
    {
        foreach (KeyValuePair<string, List<ISetting>> keyValuePair in ActiveSettings)
        {
            string category = keyValuePair.Key;
            List<ISetting> settingsInCategory = keyValuePair.Value;
            for (int index = 0; index < settingsInCategory.Count; index++)
            {
                ISetting setting = settingsInCategory[index];
                _settingsKeyCategoryIndexMap[setting.Key] = (category, index);
            }
        }
    }

    private void SetDataFromDto(GlobalDataJsonDTO dto)
    {
        foreach (KeyValuePair<string, List<SettingEntryDto>> categoryEntry in dto.Settings)
        {
            string settingsCategory = categoryEntry.Key;
            List<SettingEntryDto> entries = categoryEntry.Value;

            foreach (SettingEntryDto entry in entries)
            {
                string key = entry.Key;
                string type = entry.Type;
                object rawValue = entry.Value;

                object convertedValue;

                if (rawValue is JsonElement jsonElement)
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

                (string category, int index) = _settingsKeyCategoryIndexMap[key];
                if (type == BOOL_TYPE_NAME)
                {
                    ActiveSettings[settingsCategory][index].SaveNewValue((bool)convertedValue);
                }
                else if (type == FLOAT_TYPE_NAME)
                {
                    FloatSetting floatSetting = (FloatSetting)ActiveSettings[settingsCategory][index];
                    floatSetting.SaveNewValue(Convert.ToSingle(convertedValue));
                }
                else if (type == STRING_TYPE_NAME)
                {
                    ActiveSettings[settingsCategory][index].SaveNewValue((string)convertedValue);
                }
                else
                {
                    throw new Exception("Unsupported type: " + type);
                }
            }
        }
    }

    private GlobalDataJsonDTO AsJsonDto()
    {
        Dictionary<string, List<SettingEntryDto>> dataToStore = [];
        foreach (KeyValuePair<string, List<ISetting>> categoryEntry in ActiveSettings)
        {
            string category = categoryEntry.Key;
            List<ISetting> settings = categoryEntry.Value;
            dataToStore[category] = [];

            foreach (ISetting setting in settings)
            {
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

                SettingEntryDto entry = new(setting.Key, type, value);
                dataToStore[category].Add(entry);
            }
        }

        return new GlobalDataJsonDTO(dataToStore);
    }

    public T Get<T>(string key)
    {
        (string category, int index) = _settingsKeyCategoryIndexMap[key];
        ISetting setting = ActiveSettings[category][index];
        return (T)setting.GetValue();
    }

    public void OverrideSettings(List<ISetting> newSettingsToSave)
    {
        foreach (ISetting setting in newSettingsToSave)
        {
            (string category, int index) = _settingsKeyCategoryIndexMap[setting.Key];
            ActiveSettings[category][index] = setting;
        }
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