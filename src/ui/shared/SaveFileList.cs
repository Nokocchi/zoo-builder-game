using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using ZooBuilder.globals.saveable;

// TODO:
// Fix the DrawLine3D.Instance.PrepareDebugLines which fails after reload
// Add something more interesting like play time (how long), date to the "select this save"-button.
// Save slots should be fetched in the order of the newest save file inside. Doesn't seem to work at the moment

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
            Button button = new();
            button.Text = timestampGameDataPair.Key + "";
            button.Pressed += () => onSaveFileSelected.Invoke(timestampGameDataPair.Value);
            
            _vBoxContainer.AddChild(button);
        }
    }
}
