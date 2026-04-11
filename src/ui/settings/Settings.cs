using Godot;
using System;
using System.Collections.Generic;
using System.Globalization;
using ZooBuilder.globals;
using static ZooBuilder.globals.saveable.GlobalDataConstants;

public partial class Settings : Control
{
	private GlobalData _copyOfSettings;
	private int _audioBusIndexMaster;
	private int _audioBusIndexBgMusic;
	private int _audioBusIndexSfx;
	private OptionButton _languageSelector;
	private VBoxContainer _vBoxContainer1;
	private VBoxContainer _vBoxContainer2;
	private VBoxContainer _vBoxContainer3;
	
	public override void _Ready()
	{
		_languageSelector = GetNode<OptionButton>("%LanguageSelector");
		_vBoxContainer1 = GetNode<VBoxContainer>("%VBoxContainer1");
		_vBoxContainer2 = GetNode<VBoxContainer>("%VBoxContainer2");
		_vBoxContainer3 = GetNode<VBoxContainer>("%VBoxContainer3");

		Visible = false;

		_audioBusIndexMaster = AudioServer.GetBusIndex("Master");
		_audioBusIndexBgMusic = AudioServer.GetBusIndex("BgMusic");
		_audioBusIndexSfx = AudioServer.GetBusIndex("SFX");
		
		_languageSelector.ItemSelected += OnLanguageSelected;
		
		foreach (KeyValuePair<string, CustomInputEvent> action in InputManager.ChosenInputMappings)
		{
			InputRemapButton remapButton = InputRemapButton.Create(action.Key, action.Value);
			_vBoxContainer3.AddChild(remapButton);
		}
	}

	private void PopulateLanguageSelector(string selectedLocale)
	{
			_languageSelector.Clear();

			string[] locales = TranslationServer.GetLoadedLocales();
			string currentLocale = selectedLocale ?? TranslationServer.GetLocale();
			string sortLocale = currentLocale.Replace('_', '-');

			CultureInfo sortCulture;
			try
			{
				sortCulture = new CultureInfo(sortLocale);
			}
			catch
			{
				sortCulture = CultureInfo.InvariantCulture;
			}


			List<(string locale, string name)> entries = [];
			foreach (string locale in locales)
			{
				string normalized = locale.Replace('_', '-');

				try
				{
					CultureInfo culture = new(normalized);
					string name = Capitalize(culture.NativeName);
					entries.Add((locale, name));
				}
				catch
				{
					entries.Add((locale, locale));
				}
			}

			entries.Sort((a, b) =>
				sortCulture.CompareInfo.Compare(
					a.name,
					b.name,
					CompareOptions.StringSort
				)
			);

			foreach (var entry in entries)
			{
				_languageSelector.AddItem(entry.name);
				_languageSelector.SetItemMetadata(_languageSelector.ItemCount - 1, entry.locale);

				if (entry.locale == currentLocale) {
					_languageSelector.Select(_languageSelector.ItemCount - 1);
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
	
	private void OnLanguageSelected(long index)
	{
		//string locale = (string) _languageSelector.GetItemMetadata((int)index);
		//_settings.SelectedLocale = locale;
		//TranslationServer.SetLocale(locale);
		//_settings.Save();
	}
	
	private string GetNativeLanguageName(string locale)
	{
		// TODO: culture.Parent.NativeName?
		try
		{
			locale = locale.Replace('_', '-');
			var culture = new CultureInfo(locale);
			return Capitalize(culture.NativeName);
		}
		catch (CultureNotFoundException)
		{
			return locale; // TODO: Should I really return "locale" in the case of CultureNotFoundException?
		}
	}

	private string Capitalize(string text)
	{
		if (string.IsNullOrEmpty(text)) return text;
		return char.ToUpper(text[0]) + text.Substring(1);
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
		
		PopulateLanguageSelector(selectedLocale);
	}

	private void SaveBtnClickedSignalHandler()
	{
		_copyOfSettings.Save();
	}
}
