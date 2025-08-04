using Godot;

[Tool]
[GlobalClass]
public partial class TimeResource : Resource
{
    [Export] public int GameTime { get; set; }
}