using Godot;
using System;
using Godot.Collections;
using GodotSteam;
using ZooBuilder.data.stats;
using ZooBuilder.ui.achievement.AchievementsList;

public partial class AchievementsUI : VBoxContainer
{
	
	public override void _Ready()
	{
		Load();
	}

	private void Load()
	{
		foreach (Node child in GetChildren())
		{
			child.QueueFree();
		}
		
		AchievementList.AchievementLines.ForEach(AddAchievementLineToUi);
		HBoxContainer hBoxContainer = new HBoxContainer();

		float stat = Steam.GetStatFloat(SteamStatNames.FloatStats.DistanceWalkedStatName);
		Label label = new Label();
		label.Text = "FeetTraveled: " + stat;
		hBoxContainer.AddChild(label);
		
		float stat2 = Steam.GetStatInt(SteamStatNames.IntStats.NumGames);
		Label label2 = new Label();
		label2.Text = "NumGames: " + stat2;
		hBoxContainer.AddChild(label2);
		
		AddChild(hBoxContainer);
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
		AddChild(hBoxContainer);
	}
}
