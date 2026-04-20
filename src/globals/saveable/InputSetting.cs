#nullable enable
using System;
using Godot;
using ZooBuilder.globals;

namespace ZooBuilder.ui.settings;

public class InputSetting : Setting<CustomInputEvent>
{
    
    public InputSetting(string actionKey, CustomInputEvent customInputEvent) : base(actionKey, customInputEvent, OnSaveCallback)
    {
    }

    public void StoreKeyMapping()
    {
        OnSaveCallback(Value);
    }

    private static void OnSaveCallback(CustomInputEvent customInputEvent)
    {
        string actionKey = customInputEvent.ActionKey;
        
        if (InputMap.HasAction(actionKey))
        {
            InputMap.ActionEraseEvents(actionKey);
            InputMap.EraseAction(actionKey);
        }
        
        InputMap.AddAction(actionKey);
        InputMap.ActionAddEvent(actionKey, customInputEvent);
    }
}