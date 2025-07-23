using Godot;
using System;
using ZooBuilder.globals;
using ZooBuilder.util;

public partial class Main : Node
{

	[Export]
	public PackedScene MobScene { get; set; }

	public override void _Ready()
	{
		GlobalObjectsContainer.Instance.Game = this;
		DrawLine3D.Instance.PrepareDebugLines(this);
	}
	
	private void OnMobTimerTimeout()
	{
		Mob mob = MobScene.Instantiate<Mob>();
		PathFollow3D mobSpawnLocation = GetNode<PathFollow3D>("SpawnPath/SpawnLocation");
		mobSpawnLocation.ProgressRatio = GD.Randf();

		Vector3 playerPosition = GetNode<Player>("Player").Position;
		mob.Initialize(mobSpawnLocation.Position, playerPosition);
		
		AddChild(mob);
		mob.Squashed += GetNode<ScoreLabel>("UserInterface/ScoreLabel").OnMobSquashed;
	}
	
	// Stop spawning mobs when the player is hit and dies
	private void OnPlayerHit()
	{
		GetNode<Timer>("MobTimer").Stop();
	}


}
