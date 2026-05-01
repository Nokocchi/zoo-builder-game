using System.Collections.Generic;
using Godot;
using ZooBuilder.globals.saveable;
using ZooBuilder.ui.settings;
using static GlobalDataSingleton;

public partial class Settings : Control
{
    
    public override void _Ready()
    {
    }

    public void Initialize()
    {
        foreach (Node child in GetChildren())
        {
            child.QueueFree();
        }

        foreach (KeyValuePair<string, List<ISetting>> settingsCategory in Instance.ActiveSettings)
        {
            string settingsCategoryKey = settingsCategory.Key;
            List<ISetting> settings = settingsCategory.Value;
            SettingsCategoryContainer container = SettingsCategoryContainer.Create(settingsCategoryKey, settings);
            AddChild(container);
        }
    }
}