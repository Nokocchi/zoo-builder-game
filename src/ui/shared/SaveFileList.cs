using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using ZooBuilder.globals.saveable;

[Tool]
public partial class SaveFileList : Control
{
    private static readonly PackedScene SaveFileListScene = GD.Load<PackedScene>("res://src/ui/shared/SaveFileList.tscn");
    private VBoxContainer _vBoxContainer;
    
    public override void _Ready()
    {
        _vBoxContainer = GetNode<VBoxContainer>("%VBoxContainer");
    }

    public static SaveFileList Create()
    {
        return SaveFileListScene.Instantiate<SaveFileList>();
    }

    public void SetSaveFiles(SortedList<long, GameData> sortedSaveFiles, Action<GameData> onSaveFileSelected)
    {
        foreach (Node child in _vBoxContainer.GetChildren())
        {
            child.Free();
        }
        
        foreach (KeyValuePair<long, GameData> timestampGameDataPair in sortedSaveFiles)
        {
            GD.Print("Adding save file ", timestampGameDataPair.Key);
            SaveFileCard fileCard = SaveFileCard.Create(timestampGameDataPair.Key, timestampGameDataPair.Value, onSaveFileSelected);
            _vBoxContainer.AddChild(fileCard);
        }
    }
}
