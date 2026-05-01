using Godot;
using System;
using ZooBuilder.globals;

public partial class GameSaveLabel : Control
{
	private Label _label;
	
	public override void _Ready()
	{
		_label = GetNode<Label>("%GameSaveLabel");
		EventBus.Subscribe<GameIsAboutToBeSavedEvent>(ShowText);
	}

	public override void _ExitTree()
	{
		EventBus.Unsubscribe<GameIsAboutToBeSavedEvent>(ShowText);
	}

	private async void ShowText(GameIsAboutToBeSavedEvent e)
	{
		_label.Text = "Saving..";
		await ToSignal(GetTree().CreateTimer(2.0f), SceneTreeTimer.SignalName.Timeout);
		_label.Text = "";
	}
}
