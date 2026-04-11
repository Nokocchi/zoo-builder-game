using Godot;
using System;
using ZooBuilder.globals;

public partial class MainMenu : CanvasLayer
{

	[Export] public PackedScene GameScene;
	
	public override void _Ready()
	{
		GlobalData.LoadFromDisk();
		TranslationServer.SetLocale(GlobalData.Instance.SelectedLocale);
		InputManager.LoadActions();
	}

	
	public override void _Process(double delta)
	{
	}

	private void OnPlayPressed()
	{
		GetTree().ChangeSceneToPacked(GameScene);
	}
	
	private void OnSetingsPressed()
	{
		
	}
	
	private void OnQuitPressed()
	{
		GetTree().Quit();
	}
}
