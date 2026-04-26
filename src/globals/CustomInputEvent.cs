using Godot;

namespace ZooBuilder.globals;

public partial class CustomInputEvent : InputEventKey
{

    public string ActionKey { get; private set; }
    
    public CustomInputEvent(string actionKey, Key physicalKeyCode)
    {
        ActionKey = actionKey;
        PhysicalKeycode = physicalKeyCode;
    }

    public void SetKeyMapping(Key newKey)
    {
        PhysicalKeycode = newKey;
    }
}