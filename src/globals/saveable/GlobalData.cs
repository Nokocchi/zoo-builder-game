using System.Collections.Generic;
using System.Text.Json;
using Godot;
using ZooBuilder.globals.saveable;
using static ZooBuilder.globals.saveable.GlobalDataConstants;

public class GlobalData
{
    public static GlobalData Instance { get; private set; }

    public string SelectedLocale = "en";
    
    public Dictionary<string, float> FloatSettings = new()
    {
        [KEY_MOUSE_SENSITIVITY] = 50,
        [KEY_BACKGROUND_MUSIC_AUDIO_VOLUME] = 100
    };

    public Dictionary<string, bool> BooleanSettings = new()
    {
        [KEY_MOUSE_Y_FLIPPED] = false,
        [KEY_HOTBAR_SCROLL_DIRECTION_FLIPPED] = false,
        [KEY_HIDE_MINIMAP] = false,
        [KEY_NORTH_FACING_MINIMAP] = false
    };

    public static bool MouseYFlipped => Instance.BooleanSettings[KEY_MOUSE_Y_FLIPPED];
    public static bool HotbarScrollDirectionFlipped => Instance.BooleanSettings[KEY_HOTBAR_SCROLL_DIRECTION_FLIPPED];
    public static bool HideMinimap => Instance.BooleanSettings[KEY_HIDE_MINIMAP];
    public static bool NorthFacingMinimap => Instance.BooleanSettings[KEY_NORTH_FACING_MINIMAP];
    public static float MouseSensitivity => Instance.FloatSettings[KEY_MOUSE_SENSITIVITY];

    public static GlobalData GetCopy()
    {
        GlobalData globalData = new()
        {
            FloatSettings = new Dictionary<string, float>(Instance.FloatSettings),
            BooleanSettings = new Dictionary<string, bool>(Instance.BooleanSettings)
        };
        return globalData;
    }

    public static void LoadFromDisk()
    {
        using FileAccess file = FileAccess.Open("user://settings.json", FileAccess.ModeFlags.Read);
        if (file == null)
        {
            Error openError = FileAccess.GetOpenError();
            if (openError is Error.DoesNotExist or Error.FileNotFound)
            {
                // TODO: Handle other error cases
                Instance = new GlobalData();
                return;
            }

            GD.Print(openError);
        }

        string json = file.GetAsText();
        Instance = JsonSerializer.Deserialize<GlobalData>(json);
    }

    public void Save()
    {
        // TODO: When you have saved once, then every change in the settings will take effect immediately, rendering the save button useless. Fix this
        string serializedJson = JsonSerializer.Serialize(this);
        using FileAccess file = FileAccess.Open("user://settings.json", FileAccess.ModeFlags.Write);
        file.StoreString(serializedJson);
        Instance = this;
        TranslationServer.SetLocale(SelectedLocale);
    }
}