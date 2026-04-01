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
    
    
    private static Dictionary<string, InputEventKey[]> _inputMappings = new()
    {
        // TODO: Consider just having a list of Keycodes? If I want alt+/ctrl+/shift+ combinations, I would need another solution
        [ACTION_OPEN_INVENTORY] = [new InputEventKey { Keycode = Key.E }],
        [ACTION_JUMP] = [new InputEventKey { Keycode = Key.Space }, new InputEventKey { Keycode = Key.B }],
        [ACTION_MOVE_RIGHT] = [new InputEventKey { Keycode = Key.D }, new InputEventKey { Keycode = Key.Right }],
        [ACTION_MOVE_LEFT] = [new InputEventKey { Keycode = Key.A }, new InputEventKey { Keycode = Key.Left }],
        [ACTION_MOVE_FORWARD] = [new InputEventKey { Keycode = Key.W }, new InputEventKey { Keycode = Key.Up }],
        [ACTION_MOVE_BACK] = [new InputEventKey { Keycode = Key.S }, new InputEventKey { Keycode = Key.Down }],
        [ACTION_OPEN_SETTINGS] = [new InputEventKey { Keycode = Key.O }],
        [ACTION_OPEN_ACHIEVEMENTS] = [new InputEventKey { Keycode = Key.P }],
        [ACTION_RUN] = [new InputEventKey { Keycode = Key.Shift }],
        [ACTION_TOSS_SINGLE_ITEM] = [new InputEventKey { Keycode = Key.Q }],
    };

    // TODO: Provide input mappings loaded from settings file
    public static void LoadActions()
    {
        foreach (KeyValuePair<string, InputEventKey[]> action in _inputMappings)
        {
            if (InputMap.HasAction(action.Key))
            {
                InputMap.ActionEraseEvents(action.Key);
                InputMap.EraseAction(action.Key);
            }
            
            InputMap.AddAction(action.Key);
            foreach (InputEventKey eventKey in action.Value){
                InputMap.ActionAddEvent(action.Key, eventKey);
            }
        }
    }


}