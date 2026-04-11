using Godot;
using System;
using ZooBuilder.globals;

public partial class InputRemapButton : Button
{
    public static readonly string INPUT_REMAP_BUTTON_GROUP_NAME = "input_remap_button_group";
    private static readonly PackedScene InputRemapButtonScene = GD.Load<PackedScene>("res://src/ui/settings/InputRemapButton.tscn");
    private Label _inputKeyLabel;
    private string _actionKey;
    private CustomInputEvent _actionInputEventKey;

    public override void _Ready()
    {
        _inputKeyLabel = GetNode<Label>("%InputKeyLabel");
        _actionInputEventKey.KeyChanged += UpdateText;
        UpdateText();
    }

    public static InputRemapButton Create(string actionKey, CustomInputEvent inputEventKey)
    {
        InputRemapButton btn = InputRemapButtonScene.Instantiate<InputRemapButton>();
        btn.AddToGroup(INPUT_REMAP_BUTTON_GROUP_NAME);
        btn._actionKey = actionKey;
        btn._actionInputEventKey = inputEventKey;
        return btn;
    }

    private void UpdateText()
    {
        // Not sure what is going on in this AI generated madness
        // But it should ensure that on a Danish layout, buttons like ÆØÅ are printed correctly, and symbols like .,^¨ are printed as symbols, but space bar specifically is printed as a localized word, not " ". 
        // Supposedly this should also work on other keyboards and languages. Time will tell. 
        
        Key localizedKeyboardKey = DisplayServer.KeyboardGetLabelFromPhysical(_actionInputEventKey.PhysicalKeycode);
        string text;
        if (localizedKeyboardKey == Key.Space)
        {
            text = Tr("SPACE");
        }
        else if ((int)localizedKeyboardKey < 128 || char.IsLetterOrDigit((char)localizedKeyboardKey) || (char)localizedKeyboardKey > 127)
        {
            text = char.ConvertFromUtf32((int)localizedKeyboardKey);
        }
        else
        {
            text = OS.GetKeycodeString(localizedKeyboardKey);
        }
        
        _inputKeyLabel.Text = _actionKey + ": " + text;
    }
    
    private void OnButtonToggled(bool toggled)
    {
        if (toggled)
        {
            _inputKeyLabel.Text = "Press something..";
        }
        else
        {
            UpdateText();
        }
    }
    
    private void OnFocusLost()
    {
        ButtonPressed = false;
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        // TODO: Provide input mappings loaded from settings file
        // TODO: If the player picks a key that is already used, show what it is used for and give the option to cancel or swap. 
        //  What if it's one of the built in ones, like navigating the ui? Can you get yourself stuck on a gamepad? Maybe just block the remapping of those keys?
        
        if (!ButtonPressed) return;
        
        if (@event is InputEventKey key)
        {
            _actionInputEventKey.UpdateKeyMapping(key.PhysicalKeycode);
            ButtonPressed = false;
        }
    }
}
