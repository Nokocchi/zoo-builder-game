using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using ZooBuilder.ui.settings;
using static GlobalDataSingleton;

public partial class SettingsCategoryContainer : VBoxContainer
{
    private static readonly PackedScene SettingsCategoryContainerScene = GD.Load<PackedScene>("res://src/ui/settings/settings_category_container.tscn");
    private string _settingCategoryKey;
    private Label _title;
    private VBoxContainer _settingsContainer;
    private Button _saveButton;
    private Button _restoreDefaultsButton;
    private List<ISetting> _settings;

    public override void _Ready()
    {
        _title = GetNode<Label>("%Title");
        _settingsContainer = GetNode<VBoxContainer>("%SettingsContainer");
        _saveButton = GetNode<Button>("%SaveButton");
        _restoreDefaultsButton = GetNode<Button>("%RestoreDefaultsButton");
        _title.Text = _settingCategoryKey;
        foreach (ISetting setting in _settings)
        {
            ISettingInput input = CreateSettingsInput(setting);
            _settingsContainer.AddChild((Node)input);
        }
    }
    
    public static SettingsCategoryContainer Create(string settingCategoryKey, List<ISetting> settings)
    {
        SettingsCategoryContainer container = SettingsCategoryContainerScene.Instantiate<SettingsCategoryContainer>();
        container._settingCategoryKey = settingCategoryKey;
        container._settings = settings;
        return container;
    }

    private static ISettingInput CreateSettingsInput(ISetting setting)
    {
        string settingKey = setting.Key;
        switch (setting)
        {
            case FloatSetting floatSetting:
            {
                return FloatSettingInput.CreateWithValue(floatSetting);
            }

            case BoolSetting boolSetting:
            {
                return BooleanSettingInput.CreateWithValue(boolSetting);
            }

            case StringSetting stringSetting:
            {
                if (settingKey == KEY_SELECTED_LOCALE)
                {
                    return LanguageSelectorInput.CreateWithValue(stringSetting);
                }

                break;
            }

            case InputSetting inputSetting:
            {
                return InputRemapButton.Create(inputSetting);
            }
        }

        throw new Exception();
    }

    private void OnRestoreDefaultsButtonPressedSignalHandler()
    {
        foreach (ISettingInput input in GetInputs())
        {
            input.RestoreDefault();
        }

        SaveToDisk();
    }

    private void OnSaveButtonPressedSignalHandler()
    {
        foreach (ISettingInput input in GetInputs())
        {
            input.SaveInputStateToGlobalSetting();
        }

        SaveToDisk();
    }

    private IEnumerable<ISettingInput> GetInputs()
    {
        return _settingsContainer.GetChildren()
            .OfType<ISettingInput>();
    }
}