using System;
using Godot;

public partial class UserInterface : Control
{
    private HudClock _hudClock;
    private GameSaveLabel _gameSaveLabel;

    public override void _Ready()
    {
        _hudClock = GetNode<HudClock>("HUDClock");
        _gameSaveLabel = GetNode<GameSaveLabel>("%GameSaveLabel");
    }

    // TODO: Maybe I should just create a "gameTimeListener" group instead of passing around these signals..
    private void OnTimeUpdated(int gameTime)
    {
        _hudClock.OnTimeUpdated(gameTime);
    }

    public void ShowGameSaveText(Action saveAction)
    {
        _gameSaveLabel.ShowText(saveAction);
    }
}