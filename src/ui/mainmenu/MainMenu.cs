using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using ZooBuilder.globals;
using ZooBuilder.globals.saveable;

public partial class MainMenu : CanvasLayer
{
	
	private PanelContainer _contentContainer;
	private Settings _settings;
	private SaveSlotList _saveSlotList;
	private static readonly PackedScene SettingsScene = GD.Load<PackedScene>("res://src/ui/ingamemenu/settings/settings.tscn");
	
	public override void _Ready()
	{
		_contentContainer = GetNode<PanelContainer>("%ContentContainer");
		_saveSlotList = GetNode<SaveSlotList>("%SaveSlotList");
		_settings = SettingsScene.Instantiate<Settings>();
		_settings.Visible = false;
		_contentContainer.AddChild(_settings);
		CallDeferred(nameof(Initialize));
	}

	private void Initialize()
	{
		GlobalDataSingleton.LoadSettingsFromDisk();
		TranslationServer.SetLocale(GlobalDataSingleton.SelectedLocale);
		ItemDatabase.Initialize();
		_settings.Initialize();
		_saveSlotList.Initialize();
	}

	private void OnPlayPressed()
	{
		_settings.Visible = false;
		_saveSlotList.Visible = true;
	}
	
	private void OnSetingsPressed()
	{
		_settings.Visible = true;
		_saveSlotList.Visible = false;
	}
	
	private void OnQuitPressed()
	{
		GetTree().Quit();
	}
}
