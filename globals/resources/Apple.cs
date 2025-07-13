using Godot;

[Tool]
[GlobalClass]
public partial class Apple : ItemDataResource
{
    public override string ItemName { get; set; } = "Apple";

    public override Texture2D Texture { get; set; } =
        PathToTexture("res://art/apple.jpg");

    public override string Description { get; set; } = "A juicy, firm and sweet fruit";
    
}