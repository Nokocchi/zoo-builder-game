using Godot;

[Tool]
[GlobalClass]
public partial class Banana : ItemDataResource
{
    public override string ItemName { get; set; } = "Banana";

    public override Texture2D Texture { get; set; } =
        PathToTexture("res://art/banana.jpg");

    public override string Description { get; set; } =
        "This fruit has a handy, yellow surface which is easy to clean and can be removed quickly.";
}