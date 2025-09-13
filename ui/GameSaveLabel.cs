using Godot;
using System;

public partial class GameSaveLabel : CanvasLayer
{
	private Label _label;
	
	public override void _Ready()
	{
		_label = GetNode<Label>("%GameSaveLabel");
	}

	public async void ShowText(Action saveAction)
	{
		_label.Text = "Saving..";
		saveAction.Invoke();
		await ToSignal(GetTree().CreateTimer(2.0f), SceneTreeTimer.SignalName.Timeout);
		_label.Text = "";
	}
}
