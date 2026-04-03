using Godot;
using System;
using ZooBuilder.globals;

public partial class InputRemapButton : Button
{
    private static readonly PackedScene InputRemapButtonScene = GD.Load<PackedScene>("res://src/ui/settings/InputRemapButton.tscn");
    private Label _inputKeyLabel;
    private string _actionKey;
    private InputEventKey _actionInputEventKey;

    public override void _Ready()
    {
        _inputKeyLabel = GetNode<Label>("%InputKeyLabel");
        UpdateText();
    }

    public static InputRemapButton Create(string actionKey, InputEventKey inputEventKey)
    {
        InputRemapButton btn = InputRemapButtonScene.Instantiate<InputRemapButton>();
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
            InputManager.ListeningToInput = true;
            _inputKeyLabel.Text = "Press something..";
        }
        else
        {
            UpdateText();
        }
    }

    private void OnFocusLost()
    {
        InputManager.ListeningToInput = false;
        ButtonPressed = false;
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        // TODO: Provide input mappings loaded from settings file
        // TODO: If the player picks a key that is already used, show what it is used for and give the option to cancel or swap. 
        // TODO: Standardize focus + pressed + static boolean. Right now it's spaghetti
        
        if (!ButtonPressed) return;
        
        if (@event is InputEventKey key)
        {
            _actionInputEventKey.PhysicalKeycode = key.PhysicalKeycode;
            ReleaseFocus();
        }
    }
}
