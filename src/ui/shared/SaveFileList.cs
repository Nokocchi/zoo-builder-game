using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using ZooBuilder.globals.saveable;

// TODO:
// 1: Add something more interesting like play time (how long), date to the button.
// 2: Once this works, add the ability to make multiple save slots. It should be possible to name them.
public partial class SaveFileList : Control
{
    private VBoxContainer _vBoxContainer;
    
    public override void _Ready()
    {
        _vBoxContainer = GetNode<VBoxContainer>("%VBoxContainer");
    }

    public void AddSaveFile(Button button)
    {
        _vBoxContainer.AddChild(button);
    }
    
    public void AddSaveFiles(SortedList<long, GameData> sortedSaveFiles, Action<GameData> onSaveFileSelected)
    {
        foreach (KeyValuePair<long, GameData> timestampGameDataPair in sortedSaveFiles.Reverse())
        {
            Button button = new();
            button.Text = timestampGameDataPair.Key + "";
            button.Pressed += () => onSaveFileSelected.Invoke(timestampGameDataPair.Value);
            
            _vBoxContainer.AddChild(button);
        }
        
    }
}
