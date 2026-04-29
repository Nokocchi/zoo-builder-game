using Godot;
using GodotSteam;
using ZooBuilder.data.stats;
using ZooBuilder.globals;
using ZooBuilder.globals.saveable;

public partial class Main : Node
{
    [Export] public PackedScene MobScene { get; set; }

    [Export] public DayNightCycleResource DayNightCycleResource { get; set; }

    private AnimationPlayer _skyBoxAnimationPlayer;
    private Player _player;

    public override void _Ready()
    {
        GlobalObjectsContainer.Instance.GameScene = this;
        DrawLine3D.Instance.PrepareDebugLines(this);
        _skyBoxAnimationPlayer = GetNode<AnimationPlayer>("SkyBoxAnimationPlayer");
        _player = GetNode<Player>("Player");

        // TODO: How can I guarantee that TempStats is set by the SteamSetup before this is run? 
        SteamDataCache.GamesPlayed += 1;
        Steam.SetStatInt(SteamStatNames.IntStats.NumGames, SteamDataCache.GamesPlayed);
        Steam.StoreStats();

        GameDataSingleton.LoadDataFromDisk();

        // TODO: Check if _skyBoxAnimationPlayer.CurrentAnimationLength is indeed 40

        _skyBoxAnimationPlayer.SpeedScale = (float)
            _skyBoxAnimationPlayer.CurrentAnimationLength / DayNightCycleResource.DayNightTotalLengthSeconds;

        // Initializing the day-night cycle animation to be in sync with the loaded time of day
        int howManySecondsIntoDayNightAnimation = GameDataSingleton.Data.GameTime % DayNightCycleResource.DayNightTotalLengthSeconds;
        _skyBoxAnimationPlayer.Advance(howManySecondsIntoDayNightAnimation);
        EventBus.Publish(new GameFinishedLoadingEvent());
    }

    private void OnMobTimerTimeout()
    {
        Mob mob = MobScene.Instantiate<Mob>();
        PathFollow3D mobSpawnLocation = GetNode<PathFollow3D>("SpawnPath/SpawnLocation");
        mobSpawnLocation.ProgressRatio = GD.Randf();

        Vector3 playerPosition = _player.Position;
        mob.Initialize(mobSpawnLocation.Position, playerPosition);

        AddChild(mob);
    }

    // Stop spawning mobs when the player is hit and dies
    private void OnPlayerHit()
    {
        GetNode<Timer>("MobTimer").Stop();
    }

    private void OnGameSaveTimerTimeout()
    {
        GameDataSingleton.SaveToDisk();
    }
}