using System.Collections.Generic;
using Godot;

namespace ZooBuilder.globals;

public class InputManager
{
    public static readonly string ACTION_MOVE_RIGHT = "move_right";
    public static readonly string ACTION_MOVE_LEFT = "move_left";
    public static readonly string ACTION_MOVE_FORWARD = "move_forward";
    public static readonly string ACTION_MOVE_BACK = "move_back";
    public static readonly string ACTION_JUMP = "jump";
    public static readonly string ACTION_RUN = "run";
    public static readonly string ACTION_OPEN_SETTINGS = "open_settings";
    public static readonly string ACTION_OPEN_ACHIEVEMENTS = "open_achievements";
    public static readonly string ACTION_OPEN_INVENTORY = "open_inventory";
    public static readonly string ACTION_TOSS_SINGLE_ITEM = "toss_single_item";
    
    public static readonly Dictionary<string, CustomInputEvent> ChosenInputMappings = new();
    public static readonly Dictionary<string, InputEventKey> DefaultInputMappings = new()
    {
        // TODO: Consider just having a list of Keycodes? If I want alt+/ctrl+/shift+ combinations, I would need another solution
        [ACTION_OPEN_INVENTORY] = new InputEventKey { PhysicalKeycode = Key.E },
        [ACTION_JUMP] = new InputEventKey { PhysicalKeycode = Key.Space },
        [ACTION_MOVE_RIGHT] = new InputEventKey { PhysicalKeycode = Key.D },
        [ACTION_MOVE_LEFT] = new InputEventKey { PhysicalKeycode = Key.A },
        [ACTION_MOVE_FORWARD] = new InputEventKey { PhysicalKeycode = Key.W },
        [ACTION_MOVE_BACK] = new InputEventKey { PhysicalKeycode = Key.S },
        [ACTION_OPEN_SETTINGS] = new InputEventKey { PhysicalKeycode = Key.O },
        [ACTION_OPEN_ACHIEVEMENTS] = new InputEventKey { PhysicalKeycode = Key.P },
        [ACTION_RUN] = new InputEventKey { PhysicalKeycode = Key.Shift },
        [ACTION_TOSS_SINGLE_ITEM] = new InputEventKey { PhysicalKeycode = Key.Q },
    };
    
    // TODO: Use default keybinding if loaded settings don't contain a keybinding for an action
    // TODO: Same for other settings
    // TODO: Each settings category should have a defaults list, and a way to restore defaults and save etc. - maybe a common interface?
    
    public static void LoadActions()
    {
        foreach (KeyValuePair<string, InputEventKey> action in DefaultInputMappings)
        {
            CustomInputEvent copy = new() { PhysicalKeycode = action.Value.PhysicalKeycode };
            ChosenInputMappings.Add(action.Key, copy);
            EraseAction(action.Key);
            AddAction(action.Key, copy);
        }
    }

    public static void RestoreDefaults()
    {
        foreach (KeyValuePair<string, InputEventKey> action in DefaultInputMappings)
        {
            ChosenInputMappings[action.Key].UpdateKeyMapping(action.Value.PhysicalKeycode);
        }
    }

    public static void EraseAction(string actionKey)
    {
        if (InputMap.HasAction(actionKey))
        {
            InputMap.ActionEraseEvents(actionKey);
            InputMap.EraseAction(actionKey);
        }
    }

    public static void AddAction(string actionKey, InputEventKey actionEvent)
    {
        InputMap.AddAction(actionKey);
        InputMap.ActionAddEvent(actionKey, actionEvent);
    }


}