#nullable enable
using System;
using System.Text.Json;
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
        OnSaveCallback(GetValue());
    }
    
    protected override CustomInputEvent Deserialize(JsonElement element)
    {
        Key keycode = (Key)element.GetInt64();
        return new CustomInputEvent(Key, keycode);
    }

    protected override object Serialize(CustomInputEvent value)
    {
        return (long)value.PhysicalKeycode;
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