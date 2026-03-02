using Godot;
using GodotSteam;
using ZooBuilder.data.stats;
using ZooBuilder.globals;

public partial class Main : Node
{

	[Export]
	public PackedScene MobScene { get; set; }
	
	[Export]
	public DayNightCycleResource DayNightCycleResource { get; set; }

	private AnimationPlayer _skyBoxAnimationPlayer;
	private UserInterface _userInterface;
	private Player _player;

	public override void _Ready()
	{
		GlobalObjectsContainer.Instance.GameScene = this;
		DrawLine3D.Instance.PrepareDebugLines(this);
		_userInterface = GetNode<UserInterface>("UserInterface");
		_skyBoxAnimationPlayer = GetNode<AnimationPlayer>("SkyBoxAnimationPlayer");
		_player = GetNode<Player>("Player");
		
		// TODO: How can I guarantee that TempStats is set by the SteamSetup before this is run? 
		SteamDataCache.GamesPlayed += 1;
		Steam.SetStatInt(SteamStatNames.IntStats.NumGames, SteamDataCache.GamesPlayed);
		Steam.StoreStats();
		
		// TODO: Check if _skyBoxAnimationPlayer.CurrentAnimationLength is indeed 40
		
		_skyBoxAnimationPlayer.SpeedScale = (float)
			_skyBoxAnimationPlayer.CurrentAnimationLength / DayNightCycleResource.DayNightTotalLengthSeconds;
		
		// Initializing the day-night cycle animation to be in sync with the loaded time of day
		int howManySecondsIntoDayNightAnimation = GlobalObjectsContainer.Instance.GameData.GameTime % DayNightCycleResource.DayNightTotalLengthSeconds;
		_skyBoxAnimationPlayer.Advance(howManySecondsIntoDayNightAnimation);
	}

	public override void _ExitTree()
	{
		//DrawLine3D.Instance.Test(this);
	}

	private void OnMobTimerTimeout()
	{
		Mob mob = MobScene.Instantiate<Mob>();
		PathFollow3D mobSpawnLocation = GetNode<PathFollow3D>("SpawnPath/SpawnLocation");
		mobSpawnLocation.ProgressRatio = GD.Randf();

		Vector3 playerPosition = _player.Position;
		mob.Initialize(mobSpawnLocation.Position, playerPosition);
		
		AddChild(mob);
		mob.Squashed += GetNode<ScoreLabel>("UserInterface/ScoreLabel").OnMobSquashed;
	}
	
	// Stop spawning mobs when the player is hit and dies
	private void OnPlayerHit()
	{
		GetNode<Timer>("MobTimer").Stop();
	}

	private void OnGameSaveTimerTimeout()
	{
		_userInterface.ShowGameSaveText(SaveAction);
		return;

		void SaveAction()
		{
			Steam.StoreStats();
			GlobalObjectsContainer.Instance.GameData.Save();
		}
	}


}
