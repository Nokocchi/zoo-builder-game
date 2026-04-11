using Godot;
using System;
using System.Collections.Generic;
using System.Globalization;
using ZooBuilder.globals;
using ZooBuilder.ui.settings;
using static ZooBuilder.globals.saveable.GlobalDataConstants;

public partial class Settings : Control
{
	private GlobalData _copyOfSettings;
	private int _audioBusIndexMaster;
	private int _audioBusIndexBgMusic;
	private int _audioBusIndexSfx;
	private VBoxContainer _vBoxContainer1;
	private VBoxContainer _vBoxContainer2;
	private VBoxContainer _vBoxContainer3;
	
	public override void _Ready()
	{
		_vBoxContainer1 = GetNode<VBoxContainer>("%VBoxContainer1");
		_vBoxContainer2 = GetNode<VBoxContainer>("%VBoxContainer2");
		_vBoxContainer3 = GetNode<VBoxContainer>("%VBoxContainer3");

		Visible = false;

		_audioBusIndexMaster = AudioServer.GetBusIndex("Master");
		_audioBusIndexBgMusic = AudioServer.GetBusIndex("BgMusic");
		_audioBusIndexSfx = AudioServer.GetBusIndex("SFX");
		
		foreach (KeyValuePair<string, CustomInputEvent> action in InputManager.ChosenInputMappings)
		{
			InputRemapButton remapButton = InputRemapButton.Create(action.Key, action.Value);
			_vBoxContainer3.AddChild(remapButton);
		}
	}

	private void PopulateLanguageSelector(OptionButton languageSelector, string selectedLocale)
	{
			string currentLocale = selectedLocale ?? TranslationServer.GetLocale();
			List<(string locale, string name)> entries = LocaleUtil.GetSortedListOfLocalesForLocale(currentLocale);
			
			foreach (var entry in entries)
			{
				languageSelector.AddItem(entry.name);
				languageSelector.SetItemMetadata(languageSelector.ItemCount - 1, entry.locale);

				if (entry.locale == currentLocale) {
					languageSelector.Select(languageSelector.ItemCount - 1);
				}
			}
	}

	private void OnBackgroundAudioVolumeSliderUpdated(float audioVolume)
	{
		//_audioLevelText.Value = audioVolumeAdjusted;
		//_settings.BackgroundAudioVolume = audioVolume;
		//AudioServer.SetBusVolumeLinear(_audioBusIndexMaster,  audioVolume / 100);
		//_settings.Save();
	}
	
	private void OnBackgroundAudioVolumeTextUpdated(float audioVolume)
	{
		//_audioLevelSlider.Value = audioVolumeAdjusted;
		//_settings.BackgroundAudioVolume = audioVolume;
		//AudioServer.SetBusVolumeLinear(_audioBusIndexMaster,  audioVolume / 100);
		//_settings.Save();
	}

	private void OnRestoreDefaultKeyBindingsBtnPressed()
	{
		InputManager.RestoreDefaults();
	}

	public void InitializeWithData(GlobalData globalDataCopy)
	{
		_copyOfSettings = globalDataCopy;
		
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
		
		foreach ((string settingKey, float settingValue) in GlobalData.Instance.FloatSettings)
		{
			FloatSettingInput input = FloatSettingInput.CreateWithValue(settingKey, settingValue);
			input.ValueChanged += (newValue) => _copyOfSettings.FloatSettings[settingKey] = newValue;
			_vBoxContainer1.AddChild(input);
		}
		
		foreach ((string settingKey, bool settingValue) in GlobalData.Instance.BooleanSettings)
		{
			BooleanSettingInput input = BooleanSettingInput.CreateWithValue(settingKey, settingValue);
			input.ValueChanged += (newValue) => _copyOfSettings.BooleanSettings[settingKey] = newValue;
			_vBoxContainer2.AddChild(input);
		}

		string selectedLocale = "en";
		OptionButton languageSelector = new();
		PopulateLanguageSelector(languageSelector, selectedLocale);
		languageSelector.ItemSelected += (selectedIndex) =>
		{
			_copyOfSettings.SelectedLocale = (string)languageSelector.GetItemMetadata((int)selectedIndex);
		};
		_vBoxContainer3.AddChild(languageSelector);
		
	}

	private void SaveBtnClickedSignalHandler()
	{
		_copyOfSettings.Save();
	}
}
