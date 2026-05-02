using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using ZooBuilder.globals.saveable;

// TODO:
// Fix the DrawLine3D.Instance.PrepareDebugLines which fails after reload
// 1: Add something more interesting like play time (how long), date to the button.
// 2: Once this works, add the ability to make multiple save slots. It should be possible to name them.
// 3: Make it possible to save and load games from ingame-menu (only from your already selected slot)
// Save game as soon as Main.Ready() is done
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
