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

    public static bool ListeningToInput;
    
    public static readonly Dictionary<string, InputEventKey[]> InputMappings = new()
    {
        // TODO: Consider just having a list of Keycodes? If I want alt+/ctrl+/shift+ combinations, I would need another solution
        [ACTION_OPEN_INVENTORY] = [new InputEventKey { PhysicalKeycode = Key.E }],
        [ACTION_JUMP] = [new InputEventKey { PhysicalKeycode = Key.Space }, new InputEventKey { PhysicalKeycode = Key.B }],
        [ACTION_MOVE_RIGHT] = [new InputEventKey { PhysicalKeycode = Key.D }, new InputEventKey { PhysicalKeycode = Key.Right }],
        [ACTION_MOVE_LEFT] = [new InputEventKey { PhysicalKeycode = Key.A }, new InputEventKey { PhysicalKeycode = Key.Left }],
        [ACTION_MOVE_FORWARD] = [new InputEventKey { PhysicalKeycode = Key.W }, new InputEventKey { PhysicalKeycode = Key.Up }],
        [ACTION_MOVE_BACK] = [new InputEventKey { PhysicalKeycode = Key.S }, new InputEventKey { PhysicalKeycode = Key.Down }],
        [ACTION_OPEN_SETTINGS] = [new InputEventKey { PhysicalKeycode = Key.O }],
        [ACTION_OPEN_ACHIEVEMENTS] = [new InputEventKey { PhysicalKeycode = Key.P }],
        [ACTION_RUN] = [new InputEventKey { PhysicalKeycode = Key.Shift }],
        [ACTION_TOSS_SINGLE_ITEM] = [new InputEventKey { PhysicalKeycode = Key.Q }],
    };
    
    public static void LoadActions()
    {
        foreach (KeyValuePair<string, InputEventKey[]> action in InputMappings)
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