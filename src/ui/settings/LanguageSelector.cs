using Godot;
using System;
using System.Collections.Generic;
using ZooBuilder.ui.settings;

public partial class LanguageSelector : OptionButton, ISettingInput
{
	private static readonly PackedScene LanguageSelectorInputScene = GD.Load<PackedScene>("res://src/ui/settings/language_selector.tscn");

	private Setting<string> _setting;
	
	public override void _Ready()
	{
		AddToGroup(GlobalDataSingleton.SETTINGS_INPUT_GROUP_NAME);
		PopulateLanguageSelector(_setting.GetValue());
	}
	
	public static LanguageSelector CreateWithValue(Setting<string> setting)
	{
		LanguageSelector input = LanguageSelectorInputScene.Instantiate<LanguageSelector>();
		input._setting = setting;
		return input;
	}
	
	private void PopulateLanguageSelector(string selectedLocale)
	{
		string currentLocale = selectedLocale ?? TranslationServer.GetLocale();
		List<(string locale, string name)> entries = LocaleUtil.GetSortedListOfLocalesForLocale(currentLocale);

		foreach (var entry in entries)
		{
			AddItem(entry.name);
			SetItemMetadata(ItemCount - 1, entry.locale);

			if (entry.locale == currentLocale)
			{
				Select(ItemCount - 1);
			}
		}
	}

	private string GetSelectedLocale()
	{
		return (string)GetItemMetadata(Selected);
	}

	public void SaveInputStateToGlobalSetting()
	{
		_setting.SaveNewValue(GetSelectedLocale());
	}
}
