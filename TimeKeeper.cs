using Godot;
using System;

public partial class TimeKeeper : Node
{

    [Export] public TimeResource TimeResource;

    [Signal] public delegate void TimeUpdatedEventHandler(TimeResource timeResource);
    
    private void OnSecondPassed()
    {
        TimeResource.GameTime++;
        EmitSignal(SignalName.TimeUpdated, TimeResource);
    }
}
