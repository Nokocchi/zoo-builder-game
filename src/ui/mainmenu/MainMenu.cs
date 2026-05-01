using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using ZooBuilder.globals;
using ZooBuilder.globals.saveable;

public partial class MainMenu : CanvasLayer
{

	[Export] public PackedScene GameScene;
	
	private PanelContainer _contentContainer;
	private SaveFileList _saveFileList;
	private Settings _settings;
	private static readonly PackedScene SettingsScene = GD.Load<PackedScene>("res://src/ui/ingamemenu/settings/settings.tscn");
	
	public override void _Ready()
	{
		_contentContainer = GetNode<PanelContainer>("%ContentContainer");
		_saveFileList = GetNode<SaveFileList>("%SaveFileList");
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
		SortedList<long, GameData> sortedListOfSaves = GameDataSingleton.GetSortedListOfSaves();
		_saveFileList.SetSaveFiles(sortedListOfSaves, OnSaveFileSelected);
	}

	private void OnPlayPressed()
	{
		_settings.Visible = false;
		_saveFileList.Visible = true;
	}
	
	private void OnSaveFileSelected(GameData selectedSave)
	{
		GameDataSingleton.SetLoadedSaveFile(selectedSave);
		GetTree().ChangeSceneToPacked(GameScene);
	}
	
	private void OnSetingsPressed()
	{
		_settings.Visible = true;
		_saveFileList.Visible = false;
	}
	
	private void OnQuitPressed()
	{
		GetTree().Quit();
	}
}
