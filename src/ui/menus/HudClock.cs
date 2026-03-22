using Godot;
using System;
using ZooBuilder.globals;

public partial class HudClock : Control
{

	private Label _label;

	public override void _Ready()
	{
		_label = GetNode<Label>("%ClockLabel");
		EventBus.Subscribe<GameTimeUpdatedEvent>(OnTimeUpdated);
	}

	public void OnTimeUpdated(GameTimeUpdatedEvent e)
	{
		int gameTimeSeconds = e.newGameTime;
		int timeSecondsPart = gameTimeSeconds % 60;
		double timeMinutesPart = Math.Floor(gameTimeSeconds / 60d);
		_label.Text = $"{timeMinutesPart}:{timeSecondsPart}";
	}
}
