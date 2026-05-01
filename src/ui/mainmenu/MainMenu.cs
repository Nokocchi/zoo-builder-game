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
		GlobalDataSingleton.LoadSettingsFromDisk();
		TranslationServer.SetLocale(GlobalDataSingleton.SelectedLocale);
		_contentContainer = GetNode<PanelContainer>("%ContentContainer");
		_saveFileList = GetNode<SaveFileList>("%SaveFileList");
		ItemDatabase.Initialize();
		SortedList<long, GameData> sortedListOfSaves = GameDataSingleton.GetSortedListOfSaves();
		_settings = SettingsScene.Instantiate<Settings>();
		_settings.Visible = false;
		_contentContainer.AddChild(_settings);
		_settings.Initialize();
		_saveFileList.AddSaveFiles(sortedListOfSaves, OnSaveFileSelected);
	}

	
	public override void _Process(double delta)
	{
	}

	private void OnPlayPressed()
	{
		_saveFileList.Visible = true;
	}
	
	private void OnSaveFileSelected(GameData selectedSave)
	{
		GetTree().ChangeSceneToPacked(GameScene);
		GameDataSingleton.SetLoadedSaveFile(selectedSave);
	}
	
	private void OnSetingsPressed()
	{
		_settings.Visible = true;
	}
	
	private void OnQuitPressed()
	{
		GetTree().Quit();
	}
}
