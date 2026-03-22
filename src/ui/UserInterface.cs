using System;
using Godot;

public partial class UserInterface : CanvasLayer
{
    // TODO: Save game label could be handled by eventbus
    private GameSaveLabel _gameSaveLabel;

    public override void _Ready()
    {
        _gameSaveLabel = GetNode<GameSaveLabel>("%GameSaveLabel");
    }

    public void ShowGameSaveText(Action saveAction)
    {
        _gameSaveLabel.ShowText(saveAction);
    }
}