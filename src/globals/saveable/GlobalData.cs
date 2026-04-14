using System.Collections.Generic;
using static GlobalDataSingleton;

namespace ZooBuilder.globals.saveable;

public class GlobalData
{
    public Dictionary<string, string> StringSettings { get; private init; } = new()
    {
        [KEY_SELECTED_LOCALE] = "en"
    };

    public Dictionary<string, float> FloatSettings { get; private init; } = new()
    {
        [KEY_MOUSE_SENSITIVITY] = 50,
        [KEY_BACKGROUND_MUSIC_AUDIO_VOLUME] = 100
    };

    public Dictionary<string, bool> BooleanSettings { get; private init; } = new()
    {
        [KEY_MOUSE_Y_FLIPPED] = false,
        [KEY_HOTBAR_SCROLL_DIRECTION_FLIPPED] = false,
        [KEY_HIDE_MINIMAP] = false,
        [KEY_NORTH_FACING_MINIMAP] = false
    };

    public GlobalData GetCopy()
    {
        GlobalData globalData = new()
        {
            StringSettings = new Dictionary<string, string>(StringSettings),
            FloatSettings = new Dictionary<string, float>(FloatSettings),
            BooleanSettings = new Dictionary<string, bool>(BooleanSettings),
        };
        return globalData;
    }
}