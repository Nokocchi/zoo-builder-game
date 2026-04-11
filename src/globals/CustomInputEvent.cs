using Godot;

namespace ZooBuilder.globals;

public partial class CustomInputEvent : InputEventKey
{
    [Signal]
    public delegate void KeyChangedEventHandler();
    
    public void UpdateKeyMapping(Key newKey)
    {
        PhysicalKeycode = newKey;
        EmitSignal(SignalName.KeyChanged);
    }
}