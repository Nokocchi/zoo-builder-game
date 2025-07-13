using Godot;

[Tool]
[GlobalClass]
public partial class ItemDataResource : Resource
{
    [Export] public virtual string ItemName { get; set; }
    [Export] public virtual Texture2D Texture { get; set; } 
    [Export] public virtual string Description { get; set; }

    protected static Texture2D PathToTexture(string path)
    {
        return ImageTexture.CreateFromImage(Image.LoadFromFile(path));
    }
    
}