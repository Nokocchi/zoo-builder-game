using Godot;

public partial class ItemDataResource : Resource
{
    
    [Export] public string ItemName { get; set; }
    [Export] public Texture2D Image { get; set; }
    [Export] public string Description { get; set; }

    public ItemDataResource(string itemName, Texture2D image, string description)
    {
        ItemName = itemName;
        Image = image;
        Description = description;
    }

}