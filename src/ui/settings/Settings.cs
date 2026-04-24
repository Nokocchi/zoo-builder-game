using System.Collections.Generic;
using Godot;
using ZooBuilder.ui.settings;
using static GlobalDataSingleton;

public partial class Settings : Control
{
    private VBoxContainer _settingsCategoriesContainer;
    
    public override void _Ready()
    {
        Visible = false;
        _settingsCategoriesContainer = GetNode<VBoxContainer>("%SettingCategoriesContainer");
    }

    public void Initialize()
    {
        foreach (Node child in _settingsCategoriesContainer.GetChildren())
        {
            child.QueueFree();
        }

        foreach (KeyValuePair<string, List<ISetting>> settingsCategory in Instance.ActiveSettings)
        {
            string settingsCategoryKey = settingsCategory.Key;
            List<ISetting> settings = settingsCategory.Value;
            SettingsCategoryContainer container = SettingsCategoryContainer.Create(settingsCategoryKey, settings);
            _settingsCategoriesContainer.AddChild(container);
        }
    }
}