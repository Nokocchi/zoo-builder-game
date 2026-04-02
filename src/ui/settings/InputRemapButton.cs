using Godot;
using System;

public partial class InputRemapButton : Button
{
    private static readonly PackedScene InputRemapButtonScene = GD.Load<PackedScene>("res://src/ui/settings/InputRemapButton.tscn");
    private Label _inputKeyLabel;
    private string _actionKey;
    private InputEventKey[] _actionInputEventKeys;

    public override void _Ready()
    {
        _inputKeyLabel = GetNode<Label>("%InputKeyLabel");
        SetText(_actionInputEventKeys[0].PhysicalKeycode);
    }

    public static InputRemapButton Create(string actionKey, InputEventKey[] inputEventKeys)
    {
        InputRemapButton btn = InputRemapButtonScene.Instantiate<InputRemapButton>();
        btn._actionKey = actionKey;
        btn._actionInputEventKeys = inputEventKeys;
        return btn;
    }

    private void SetText(Key physicalKeycode)
    {
        // Not sure what is going on in this AI generated madness
        // But it should ensure that on a Danish layout, buttons like ÆØÅ are printed correctly, and symbols like .,^¨ are printed as symbols, but space bar specifically is printed as a localized word, not " ". 
        // Supposedly this should also work on other keyboards and languages. Time will tell. 
        
        Key localizedKeyboardKey = DisplayServer.KeyboardGetLabelFromPhysical(physicalKeycode);
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
        
        _inputKeyLabel.Text = _actionKey + "-" + text;
    }

    private void OnButtonToggled(bool toggled)
    {
        if (toggled)
        {
            _inputKeyLabel.Text = "Press something..";
        }
        else
        {
            SetText(_actionInputEventKeys[0].PhysicalKeycode);
        }
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        // TODO: If you click on another button or close inventory etc., then stop listening and un-press this button. Maybe make a popup instead of a toggle button?
        // TODO: The comma key still shows up as the English text "comma". It should be the symbol instead.
        // TODO: Provide input mappings loaded from settings file
        
        if (!ButtonPressed) return;
        
        if (@event is InputEventKey key)
        {
            _actionInputEventKeys[0].PhysicalKeycode = key.PhysicalKeycode;
            ButtonPressed = false;
        }
    }
}
