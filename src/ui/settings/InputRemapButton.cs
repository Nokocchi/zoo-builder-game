using Godot;
using System;
using ZooBuilder.globals;
using ZooBuilder.ui.settings;

public partial class InputRemapButton : Button, ISettingInput
{
    public static readonly string INPUT_REMAP_BUTTON_GROUP_NAME = "input_remap_button_group";
    private static readonly PackedScene InputRemapButtonScene = GD.Load<PackedScene>("res://src/ui/settings/InputRemapButton.tscn");
    private Label _inputKeyLabel;
    private string _actionKey;
    private InputSetting _inputSetting;
    private CustomInputEvent _actionInputEventKey;

    public override void _Ready()
    {
        AddToGroup(GlobalDataSingleton.SETTINGS_INPUT_GROUP_NAME);
        _inputKeyLabel = GetNode<Label>("%InputKeyLabel");
        UpdateText();
    }

    public static InputRemapButton Create(InputSetting inputSetting)
    {
        InputRemapButton btn = InputRemapButtonScene.Instantiate<InputRemapButton>();
        btn.AddToGroup(INPUT_REMAP_BUTTON_GROUP_NAME);
        btn._inputSetting = inputSetting;
        btn._actionKey = inputSetting.Key;
        btn._actionInputEventKey = new CustomInputEvent(inputSetting.GetValue().ActionKey, inputSetting.GetValue().PhysicalKeycode);
        return btn;
    }

    private void UpdateText()
    {
        if (ButtonPressed)
        {
            _inputKeyLabel.Text = "Press something..";
        }
        else
        {
            string text = LocaleUtil.Instance.GetLocalizedLabelForKeyCode(_actionInputEventKey.PhysicalKeycode);
            _inputKeyLabel.Text = _actionKey + ": " + text;
        }
    }
    
    private void OnButtonToggled(bool toggled)
    {
        UpdateText();
    }
    
    private void OnFocusLost()
    {
        ButtonPressed = false;
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        
        if (!ButtonPressed) return;
        
        if (@event is InputEventKey key)
        {
            _actionInputEventKey.SetKeyMapping(key.PhysicalKeycode);
            ButtonPressed = false;
        }
    }

    public void SaveInputStateToGlobalSetting()
    {
        _inputSetting.SaveNewValue(_actionInputEventKey);
        
    }
}
