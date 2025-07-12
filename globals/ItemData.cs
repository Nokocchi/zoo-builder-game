using Godot;
using System;

public partial class ItemData : Node
{
    public static ItemData Instance { get; private set; }
    
    public override void _EnterTree()
    {
        if (Instance != null)
        {
            QueueFree();
        }

        Instance = this;
    }
    
    private const string AppleName = "Apple";
    private static readonly Texture2D AppleImg = ImageTexture.CreateFromImage(Image.LoadFromFile("res://art/apple.jpg"));
    private const string DescApple = "A juicy, firm and sweet fruit";

    private const string BananaName = "Banana";
    private static readonly Texture2D BananaImg = ImageTexture.CreateFromImage(Image.LoadFromFile("res://art/banana.jpg"));
    private const string BananaDesc = "This fruit has a clever, yellow surface which is easy to clean and can be removed quickly.";
    
    private const string OrangeName = "Orange";
    private static readonly Texture2D OrangeImg = ImageTexture.CreateFromImage(Image.LoadFromFile("res://art/orange.png"));
    private const string OrangeDesc = "Well, it is orange.";

    
    public static readonly ItemDataResource Apple = new(AppleName, AppleImg, DescApple);
    public static readonly ItemDataResource Banana = new(BananaName, BananaImg, BananaDesc);
    public static readonly ItemDataResource Orange = new(OrangeName, OrangeImg, OrangeDesc);

}
