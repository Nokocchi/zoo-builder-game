using Godot;
using System;
using System.Collections.Generic;
using System.Globalization;
using ZooBuilder.globals;

public partial class Settings : Control
{

	private HSlider _mouseSensitivitySlider;
	private SpinBox _mouseSensitivityText;
	private HSlider _audioLevelSlider;
	private SpinBox _audioLevelText;
	private CheckBox _mouseUpDownFlipped;
	private CheckBox _hotbarScrollDirectionFlipped;
	private CheckBox _hideMinimap;
	private SettingsResource _settings;
	private int _audioBusIndexMaster;
	private int _audioBusIndexBgMusic;
	private int _audioBusIndexSfx;
	private OptionButton _languageSelector;

	private VBoxContainer _vBoxContainer;
	
	public override void _Ready()
	{
		_mouseSensitivitySlider = GetNode<HSlider>("%MouseSensitivitySlider");
		_mouseSensitivityText = GetNode<SpinBox>("%MouseSensitivityText");
		_audioLevelSlider = GetNode<HSlider>("%AudioLevelSlider");
		_audioLevelText = GetNode<SpinBox>("%AudioLevelText");
		_mouseUpDownFlipped = GetNode<CheckBox>("%MouseUpDownFlipped");
		_hotbarScrollDirectionFlipped = GetNode<CheckBox>("%HotbarScrollDirectionFlipped");
		_languageSelector = GetNode<OptionButton>("%LanguageSelector");
		_hideMinimap = GetNode<CheckBox>("%HideMinimap");
		_vBoxContainer = GetNode<VBoxContainer>("%VBoxContainer3");

		Visible = false;
		
		_settings = SettingsResource.Load();
		
		_mouseSensitivitySlider.Value = _settings.MouseSensitivity;
		_mouseSensitivityText.Value = _settings.MouseSensitivity;
		_audioLevelSlider.Value = _settings.BackgroundAudioVolume;
		_audioLevelText.Value = _settings.BackgroundAudioVolume;
		_mouseUpDownFlipped.SetPressed(_settings.MouseUpDownFlipped);
		_hotbarScrollDirectionFlipped.SetPressed(_settings.HotbarScrollDirectionFlipped);
		string selectedLocale = _settings.SelectedLocale;

		_audioBusIndexMaster = AudioServer.GetBusIndex("Master");
		_audioBusIndexBgMusic = AudioServer.GetBusIndex("BgMusic");
		_audioBusIndexSfx = AudioServer.GetBusIndex("SFX");

		PopulateLanguageSelector(selectedLocale);
		_languageSelector.ItemSelected += OnLanguageSelected;
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

	private void OnMouseSensitivitySliderUpdated(float mouseSensitivity)
	{
		_mouseSensitivityText.Value = mouseSensitivity;
		_settings.MouseSensitivity = mouseSensitivity;
		_settings.Save();
	}
	
	private void OnMouseSensitivityTextUpdated(float mouseSensitivity)
	{
		_mouseSensitivitySlider.Value = mouseSensitivity;
		_settings.MouseSensitivity = mouseSensitivity;
		_settings.Save();
	}
	
	private void OnBackgroundAudioVolumeSliderUpdated(float audioVolume)
	{
		//_audioLevelText.Value = audioVolumeAdjusted;
		_settings.BackgroundAudioVolume = audioVolume;
		AudioServer.SetBusVolumeLinear(_audioBusIndexMaster,  audioVolume / 100);
		_settings.Save();
	}
	
	private void OnBackgroundAudioVolumeTextUpdated(float audioVolume)
	{
		//_audioLevelSlider.Value = audioVolumeAdjusted;
		_settings.BackgroundAudioVolume = audioVolume;
		AudioServer.SetBusVolumeLinear(_audioBusIndexMaster,  audioVolume / 100);
		_settings.Save();
	}

	private void OnMouseUpDownFlippedUpdated(bool mouseUpDownFlipped)
	{
		_settings.MouseUpDownFlipped = mouseUpDownFlipped;
		_settings.Save();
	}
	
	private void OnHotbarScrollDirectionFlipped(bool scrollbarDirectionFlipped)
	{
		_settings.HotbarScrollDirectionFlipped = scrollbarDirectionFlipped;
		_settings.Save();
	}
	
	private void OnHideMinimap(bool hideMinimap)
	{
		_settings.HideMinimap = hideMinimap;
		_settings.Save();
	}
	
	private void OnNorthFacingMinimap(bool northFacingMinimap)
	{
		_settings.NorthFacingMinimap = northFacingMinimap;
		_settings.Save();
	}
	
	private void OnLanguageSelected(long index)
	{
		string locale = (string) _languageSelector.GetItemMetadata((int)index);
		_settings.SelectedLocale = locale;
		TranslationServer.SetLocale(locale);
		_settings.Save();
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
			return locale; // TODO: Is this correct?
		}
	}

	private string Capitalize(string text)
	{
		if (string.IsNullOrEmpty(text)) return text;
		return char.ToUpper(text[0]) + text.Substring(1);
	}

	public void DoSomeStuff()
	{
		foreach (Node child in _vBoxContainer.GetChildren())
		{
			child.QueueFree();
		}

		foreach (KeyValuePair<string, InputEventKey[]> action in InputManager.InputMappings)
		{
			InputRemapButton remapButton = InputRemapButton.Create(action.Key, action.Value);
			_vBoxContainer.AddChild(remapButton);
		}
	}
}
