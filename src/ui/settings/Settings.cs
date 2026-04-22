using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using ZooBuilder.globals;
using ZooBuilder.globals.saveable;
using ZooBuilder.ui.settings;
using static GlobalDataSingleton;

public partial class Settings : Control
{
    private OptionButton _languageSelector;
    private VBoxContainer _vBoxContainer1;
    private VBoxContainer _vBoxContainer2;
    private VBoxContainer _vBoxContainer3;

    public override void _Ready()
    {
        _vBoxContainer1 = GetNode<VBoxContainer>("%VBoxContainer1");
        _vBoxContainer2 = GetNode<VBoxContainer>("%VBoxContainer2");
        _vBoxContainer3 = GetNode<VBoxContainer>("%VBoxContainer3");

        Visible = false;
    }

    private void OnRestoreDefaultKeyBindingsBtnPressed()
    {
    }

    public void Initialize()
    {
        foreach (Node child in _vBoxContainer1.GetChildren())
        {
            child.QueueFree();
        }

        foreach (Node child in _vBoxContainer2.GetChildren())
        {
            child.QueueFree();
        }

        foreach (Node child in _vBoxContainer3.GetChildren())
        {
            child.QueueFree();
        }

        foreach (KeyValuePair<string, List<ISetting>> settingsCategory in Instance.ActiveSettings)
        {
            string settingsCategoryKey = settingsCategory.Key;
            List<ISetting> settings = settingsCategory.Value;

            VBoxContainer settingsContainer = null;
            if (settingsCategoryKey == SETTINGS_CATEGORY_GAMEPLAY)
            {
                settingsContainer = _vBoxContainer1;
            }
            else if (settingsCategoryKey == SETTINGS_CATEGORY_OTHER)
            {
                settingsContainer = _vBoxContainer2;
            }
            else if (settingsCategoryKey == SETTINGS_CATEGORY_INPUT)
            {
                settingsContainer = _vBoxContainer3;
            }

            foreach (ISetting setting in settings)
            {
                ISettingInput input = createSettingsInput(setting);
                settingsContainer.AddChild((Node) input);
            }
            
            Button button = new();
            button.Text = "Restore defaults";
            settingsContainer.AddChild(button);
            // TODO: When resetting settings, update the Settings
        }
    }

    private ISettingInput createSettingsInput(ISetting setting)
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

    private void SaveBtnClickedSignalHandler()
    {
        IEnumerable<ISettingInput> inputs = GetTree()
            .GetNodesInGroup(SETTINGS_INPUT_GROUP_NAME)
            .OfType<ISettingInput>();

        foreach (ISettingInput input in inputs)
        {
            input.SaveInputStateToGlobalSetting();
        }

        SaveToDisk();
    }
}