using Godot;
using System;
using ZooBuilder.globals.saveable;

public partial class SaveFileCard : Control
{
    private static readonly PackedScene SaveFileCardScene = GD.Load<PackedScene>("res://src/ui/shared/save_file_card.tscn");

    private Label _itemCountLabel;
    private Label _savedTimeAgoLabel;
    private Label _savedTimestampLabel;

    private long _timestamp;
    private GameData _gameData;
    private Action<GameData> _onSaveFileSelected;


    public override void _Ready()
    {
        _itemCountLabel = GetNode<Label>("%ItemCountLabel");
        _savedTimeAgoLabel = GetNode<Label>("%SavedTimeAgoLabel");
        _savedTimestampLabel = GetNode<Label>("%SavedTimestampLabel");
        long timeNow = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long timeBetweenNowAndSaveTimestamp = timeNow - _timestamp;
        _itemCountLabel.Text = "Items: " + _gameData.Inventory.FindAll(i => i.HasItem()).Count;
        _savedTimeAgoLabel.Text = $"Saved {FormatTimeAgo(timeBetweenNowAndSaveTimestamp)}";
        _savedTimestampLabel.Text = _timestamp + "";
    }

    public static SaveFileCard Create(long timestamp, GameData gameData, Action<GameData> onSaveFileSelected)
    {
        SaveFileCard saveFileCard = SaveFileCardScene.Instantiate<SaveFileCard>();
        saveFileCard._timestamp = timestamp;
        saveFileCard._gameData = gameData;
        saveFileCard._onSaveFileSelected = onSaveFileSelected;
        return saveFileCard;
    }
    
    private void OnSaveGameSelectedButtonPressed()
    {
        _onSaveFileSelected.Invoke(_gameData);
    }
    
    string FormatTimeAgo(long seconds)
    {
        if (seconds < 60)
            return $"{seconds} second{(seconds == 1 ? "" : "s")} ago";

        long minutes = seconds / 60;
        if (minutes < 60)
            return $"{minutes} minute{(minutes == 1 ? "" : "s")} ago";

        long hours = minutes / 60;
        if (hours < 24)
            return $"{hours} hour{(hours == 1 ? "" : "s")} ago";

        long days = hours / 24;
        if (days < 30)
            return $"{days} day{(days == 1 ? "" : "s")} ago";

        long months = days / 30;
        if (months < 12)
            return $"{months} month{(months == 1 ? "" : "s")} ago";

        long years = days / 365;
        return $"{years} year{(years == 1 ? "" : "s")} ago";
    }
}