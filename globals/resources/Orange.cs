using Godot;

[Tool]
[GlobalClass]
public partial class Orange : ItemDataResource
{
    public override string ItemName { get; set; } = "Orange";

    public override Texture2D Texture { get; set; } =
        PathToTexture("res://art/orange.png");

    public override string Description { get; set; } = "It is indeed orange.";
}