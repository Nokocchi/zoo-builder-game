using Godot;
using System;

public partial class InventoryItemStack : Button
{
    private ItemStackResource _itemStackResource;

    public ItemStackResource ItemStackResource
    {
        get => _itemStackResource;
        set
        {
            _itemStackResource = value;
            Render();
        }
    }

    private TextureRect _itemIcon;
    private Label _stackSizeLabel;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _itemIcon = GetNode<TextureRect>("ItemIcon");
        _stackSizeLabel = GetNode<Label>("ItemIcon/StackSize");
        Render();
    }

    public void ClearStackResource()
    {
        SetTexture(null);
        SetStackSize(null);
        ItemStackResource = null;
    }

    private void Render()
    {
        if (ItemStackResource != null)
        {
            SetTexture(ItemStackResource.ItemData.Texture);
            SetStackSize(ItemStackResource.Amount + "");
        }
    }

    private void SetTexture(Texture2D texture)
    {
        _itemIcon.Texture = texture;
    }

    private void SetStackSize(string stackSize)
    {
        _stackSizeLabel.Text = "" + stackSize;
    }

// Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}