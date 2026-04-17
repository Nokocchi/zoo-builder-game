using System.Collections.Generic;
using System.Linq;
using Godot;
using ZooBuilder.globals;
using ZooBuilder.globals.saveable;
using ZooBuilder.ui.settings;

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

        foreach (KeyValuePair<string, CustomInputEvent> action in InputManager.ChosenInputMappings)
        {
            InputRemapButton remapButton = InputRemapButton.Create(action.Key, action.Value);
            _vBoxContainer3.AddChild(remapButton);
        }
    }

    private void OnRestoreDefaultKeyBindingsBtnPressed()
    {
        InputManager.RestoreDefaults();
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

        foreach (KeyValuePair<string, List<ISetting>> settingsCategory in GlobalDataSingleton.Instance.ActiveSettings)
        {
            string settingsCategoryKey = settingsCategory.Key;
            List<ISetting> settings = settingsCategory.Value;

            foreach (ISetting setting in settings)
            {
                string settingKey = setting.Key;
                switch (setting)
                {
                    case Setting<float> floatSetting:
                    {
                        FloatSetting castFloatSetting = (FloatSetting)floatSetting;
                        FloatSettingInput input = FloatSettingInput.CreateWithValue(castFloatSetting);
                        _vBoxContainer1.AddChild(input);
                        break;
                    }

                    case Setting<bool> boolSetting:
                    {
                        BooleanSettingInput input = BooleanSettingInput.CreateWithValue(boolSetting);
                        _vBoxContainer2.AddChild(input);
                        break;
                    }

                    case Setting<string> stringSetting:
                    {
                        if (settingKey == GlobalDataSingleton.KEY_SELECTED_LOCALE)
                        {
                            LanguageSelector languageSelector = LanguageSelector.CreateWithValue(stringSetting);
                            _vBoxContainer3.AddChild(languageSelector);
                        }

                        break;
                    }
                }
            }
        }
    }

    private void SaveBtnClickedSignalHandler()
    {
        IEnumerable<ISettingInput> inputs = GetTree()
            .GetNodesInGroup(GlobalDataSingleton.SETTINGS_INPUT_GROUP_NAME)
            .OfType<ISettingInput>();
        
        foreach (ISettingInput input in inputs)
        {
            input.SaveInputStateToGlobalSetting();
        }
        
        GlobalDataSingleton.SaveToDisk();
    }
}