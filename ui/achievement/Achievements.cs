using Godot;
using System;
using Godot.Collections;
using GodotSteam;
using ZooBuilder.ui.achievement.AchievementsList;

public partial class Achievements : CanvasLayer
{

	private VBoxContainer _achievementsListVertical;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Visible = false;
		_achievementsListVertical = GetNode<VBoxContainer>("%AchievementsListVertical");
	}

	private void Load()
	{
		foreach (Node child in _achievementsListVertical.GetChildren())
		{
			child.QueueFree();
		}
		
		AchievementList.AchievementLines.ForEach(AddAchievementLineToUi);
	}

	private void AddAchievementLineToUi(IAchievementLine al)
	{
		HBoxContainer hBoxContainer = new HBoxContainer();
		foreach (string achievementName in al.AchievementNamesOrdered())
		{
			Dictionary achievement = Steam.GetAchievement(achievementName);
			Label label = new Label();
			label.Text = achievementName + ": " + achievement["achieved"] + " | ";
			hBoxContainer.AddChild(label);
		}
		_achievementsListVertical.AddChild(hBoxContainer);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("open_achievements"))
		{
			Visible = !Visible;
			Input.MouseMode = Visible ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
			if (Visible)
			{
				Load();
			}
		}
	}
}
