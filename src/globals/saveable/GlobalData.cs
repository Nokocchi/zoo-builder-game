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
        [SETTINGS_CATEGORY_GAMEPLAY] =
        [
            new BoolSetting(KEY_MOUSE_Y_FLIPPED, false),
            new BoolSetting(KEY_HOTBAR_SCROLL_DIRECTION_FLIPPED, false),
            new BoolSetting(KEY_HIDE_MINIMAP, false),
            new BoolSetting(KEY_NORTH_FACING_MINIMAP, false),
            new FloatSetting(KEY_MOUSE_SENSITIVITY, 50, 0, 100),
            new FloatSetting(KEY_BACKGROUND_MUSIC_AUDIO_VOLUME, 100, 0, 100, audioVolume =>
            {
                int audioBusIndexBgMusic = AudioServer.GetBusIndex("BgMusic");
                AudioServer.SetBusVolumeLinear(audioBusIndexBgMusic, audioVolume / 100);
            })
        ],
        [SETTINGS_CATEGORY_OTHER] =
        [
            new StringSetting(KEY_SELECTED_LOCALE, "en", TranslationServer.SetLocale)
        ],
        [SETTINGS_CATEGORY_INPUT] =
        [
            new InputSetting(ACTION_OPEN_INVENTORY, new CustomInputEvent(ACTION_OPEN_INVENTORY, Key.E)),
            new InputSetting(ACTION_JUMP, new CustomInputEvent(ACTION_JUMP, Key.Space)),
            new InputSetting(ACTION_MOVE_RIGHT, new CustomInputEvent(ACTION_MOVE_RIGHT, Key.D)),
            new InputSetting(ACTION_MOVE_LEFT, new CustomInputEvent(ACTION_MOVE_LEFT, Key.A)),
            new InputSetting(ACTION_MOVE_FORWARD, new CustomInputEvent(ACTION_MOVE_FORWARD, Key.W)),
            new InputSetting(ACTION_MOVE_BACK, new CustomInputEvent(ACTION_MOVE_BACK, Key.S)),
            new InputSetting(ACTION_OPEN_SETTINGS, new CustomInputEvent(ACTION_OPEN_SETTINGS, Key.O)),
            new InputSetting(ACTION_OPEN_ACHIEVEMENTS, new CustomInputEvent(ACTION_OPEN_ACHIEVEMENTS, Key.P)),
            new InputSetting(ACTION_RUN, new CustomInputEvent(ACTION_RUN, Key.Shift)),
            new InputSetting(ACTION_TOSS_SINGLE_ITEM, new CustomInputEvent(ACTION_TOSS_SINGLE_ITEM, Key.Q)),
        ]
    };

    // Create with default data
    public GlobalData()
    {
        InitializeLookupTables();
    }

    // Create with loaded data and initialize
    public GlobalData(GlobalDataJsonDTO initializeWithDTO)
    {
        InitializeLookupTables();
        SetDataFromDto(initializeWithDTO);
        InitializeInputMappings();
    }

    private void InitializeInputMappings()
    {
        foreach (ISetting setting in ActiveSettings[SETTINGS_CATEGORY_INPUT])
        {
            InputSetting inputSetting = (InputSetting)setting;
            inputSetting.StoreKeyMapping();
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

    public T Get<T>(string key)
    {
        (string category, int index) = _settingsKeyCategoryIndexMap[key];
        ISetting setting = ActiveSettings[category][index];
        return (T)setting.GetValueUntyped();
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

    private void SetDataFromDto(GlobalDataJsonDTO dto)
    {
        foreach (var categoryEntry in dto.Settings)
        {
            foreach (var entry in categoryEntry.Value)
            {
                (string category, int index) = _settingsKeyCategoryIndexMap[entry.Key];
                ISetting setting = ActiveSettings[category][index];
                setting.LoadFromJson(entry.Value);
            }
        }
    }

    private GlobalDataJsonDTO AsJsonDto()
    {
        var data = new Dictionary<string, List<SettingEntryDto>>();
        foreach ((string category, List<ISetting> settings) in ActiveSettings)
        {
            data[category] = [];
            foreach (ISetting setting in settings)
            {
                object value = setting.SaveToJson();
                data[category].Add(new SettingEntryDto(setting.Key, JsonSerializer.SerializeToElement(value)));
            }
        }

        return new GlobalDataJsonDTO(data);
    }
}