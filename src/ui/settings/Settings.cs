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

    private void PopulateLanguageSelector(string selectedLocale)
    {
        string currentLocale = selectedLocale ?? TranslationServer.GetLocale();
        List<(string locale, string name)> entries = LocaleUtil.GetSortedListOfLocalesForLocale(currentLocale);

        foreach (var entry in entries)
        {
            _languageSelector.AddItem(entry.name);
            _languageSelector.SetItemMetadata(_languageSelector.ItemCount - 1, entry.locale);

            if (entry.locale == currentLocale)
            {
                _languageSelector.Select(_languageSelector.ItemCount - 1);
            }
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

        foreach (KeyValuePair<string, Dictionary<string, ISetting>> settingsCategory in GlobalDataSingleton.Instance.ActiveSettings)
        {
            string settingsCategoryKey = settingsCategory.Key;
            Dictionary<string, ISetting> settings = settingsCategory.Value;

            foreach (KeyValuePair<string, ISetting> settingPair in settings)
            {
                string settingKey = settingPair.Key;
                ISetting setting = settingPair.Value;
                switch (setting)
                {
                    case Setting<float> floatSetting:
                    {
                        (float min, float max) = GlobalDataSingleton.FloatSettingMinMax[settingKey];
                        FloatSettingInput input = FloatSettingInput.CreateWithValue(settingKey, floatSetting.Value, min, max);
                        _vBoxContainer1.AddChild(input);
                        break;
                    }

                    case Setting<bool> boolSetting:
                    {
                        BooleanSettingInput input = BooleanSettingInput.CreateWithValue(settingKey, boolSetting.Value);
                        _vBoxContainer2.AddChild(input);
                        break;
                    }

                    case Setting<string> stringSetting:
                    {
                        if (settingKey == GlobalDataSingleton.KEY_SELECTED_LOCALE)
                        {
                            _languageSelector = new OptionButton();
                            PopulateLanguageSelector(stringSetting.Value);
                            _vBoxContainer3.AddChild(_languageSelector);
                        }

                        break;
                    }
                }
            }
        }
    }

    private void SaveBtnClickedSignalHandler()
    {
        GlobalData globalDataDtoFromUiState = GenerateGlobalDataDtoFromUiState();
        GlobalDataSingleton.Save(globalDataDtoFromUiState);
    }

    private GlobalData GenerateGlobalDataDtoFromUiState()
    {
        GlobalData copy = GlobalDataSingleton.Instance.GetCopy();

        IEnumerable<ISettingInput> inputs = GetTree()
            .GetNodesInGroup(GlobalDataSingleton.SETTINGS_INPUT_GROUP_NAME)
            .OfType<ISettingInput>();

        foreach (ISettingInput input in inputs)
        {
            copy.SetSetting(input.GetAsSetting());
        }
        
        copy.SetSetting(new Setting<string>(GlobalDataSingleton.KEY_SELECTED_LOCALE, (string)_languageSelector.GetItemMetadata(_languageSelector.Selected)));

        return copy;
    }
}