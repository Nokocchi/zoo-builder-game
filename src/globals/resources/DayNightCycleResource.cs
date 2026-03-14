using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Godot;

[GlobalClass]
public partial class DayNightCycleResource : Resource
{
    [Export] public int DayNightTotalLengthSeconds = 60 * 24;
    
}