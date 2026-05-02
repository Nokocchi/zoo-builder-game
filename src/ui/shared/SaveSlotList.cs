using Godot;
using System;
using System.Collections.Generic;
using ZooBuilder.globals.saveable;

public partial class SaveSlotList : Control
{
    private Dictionary<string, SortedList<long, GameData>> _sortedListOfSavesForSaveSlots;
    private VBoxContainer _slotsList;
    private MarginContainer _saveFileListContainer;
    private LineEdit _newSaveSlotNameInput;
    private Button _newSaveSlotButton;
    private static readonly PackedScene SaveFileListScene = GD.Load<PackedScene>("res://src/ui/shared/SaveFileList.tscn");
    private static readonly PackedScene MainScene = GD.Load<PackedScene>("res://src/main.tscn");
    
    public override void _Ready()
    {
        _slotsList = GetNode<VBoxContainer>("%SlotsList");
        _saveFileListContainer = GetNode<MarginContainer>("%SaveFileListContainer");
        _newSaveSlotNameInput = GetNode<LineEdit>("%NewSaveSlotNameInput");
        _newSaveSlotButton = GetNode<Button>("%NewSaveSlotButton");
    }

    public void Render()
    {
        foreach (KeyValuePair<string, SortedList<long, GameData>> sortedListOfSavesForSaveSlot in _sortedListOfSavesForSaveSlots)
        {
            string saveSlotName = sortedListOfSavesForSaveSlot.Key;
            SortedList<long, GameData> saveFiles = sortedListOfSavesForSaveSlot.Value;
            Button button = new();
            button.Text = saveSlotName;
            button.Pressed += () =>
            {
                foreach (Node child in _saveFileListContainer.GetChildren())
                {
                    child.Free();
                }
                SaveFileList saveFileList = SaveFileListScene.Instantiate<SaveFileList>();
                _saveFileListContainer.AddChild(saveFileList);
                saveFileList.SetSaveFiles(saveFiles, OnSaveFileSelected);
            };
            _slotsList.AddChild(button);
        }
    }
    
    private void OnSaveFileSelected(GameData selectedSave)
    {
        GameDataSingleton.SetLoadedSaveFile(selectedSave);
        GetTree().ChangeSceneToPacked(MainScene);
    }

    private void OnNewSaveSlotButtonClickedSignalHandler()
    {
        string newSaveSlotName = _newSaveSlotNameInput.Text;
        if (newSaveSlotName == null || newSaveSlotName.Trim() == "" || _sortedListOfSavesForSaveSlots.ContainsKey(newSaveSlotName))
        {
            return;
        }
        
        GameDataSingleton.SaveSlotName = newSaveSlotName;
        GetTree().ChangeSceneToPacked(MainScene);
    }

    public void Initialize()
    {
        _sortedListOfSavesForSaveSlots = GameDataSingleton.GetSortedListOfSavesForSaveSlots();
        Render();
    }
}
