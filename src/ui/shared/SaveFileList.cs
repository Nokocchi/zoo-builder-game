using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using ZooBuilder.globals.saveable;

// TODO:
// Fix the DrawLine3D.Instance.PrepareDebugLines which fails after reload
// In-game save selector no longer shows up
public partial class SaveFileList : Control
{
    private VBoxContainer _vBoxContainer;
    
    public override void _Ready()
    {
        _vBoxContainer = GetNode<VBoxContainer>("%VBoxContainer");
    }
    
    public void SetSaveFiles(SortedList<long, GameData> sortedSaveFiles, Action<GameData> onSaveFileSelected)
    {
        foreach (Node child in _vBoxContainer.GetChildren())
        {
            child.Free();
        }
        
        foreach (KeyValuePair<long, GameData> timestampGameDataPair in sortedSaveFiles)
        {
            SaveFileCard saveFileCard = SaveFileCard.Create(timestampGameDataPair.Key, timestampGameDataPair.Value, onSaveFileSelected);
            _vBoxContainer.AddChild(saveFileCard);
        }
    }
}
