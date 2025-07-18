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

    public int InventoryIndex { get; set; }
    private TextureRect _itemIcon;
    private Label _stackSizeLabel;
    private Panel _panelWithBorder;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _itemIcon = GetNode<TextureRect>("%ItemIcon");
        _stackSizeLabel = GetNode<Label>("%StackSize");
        _panelWithBorder = GetNode<Panel>("%PanelWithBorder");
        Render();
    }

    public void ClearStackResource()
    {
        ItemStackResource = null;
        Render();
    }

    private void Render()
    {
        if (ItemStackResource != null)
        {
            SetTexture(ItemStackResource.ItemData.Texture);
            SetStackSize(ItemStackResource.Amount + "");
        }
        else
        {
            SetTexture(null);
            SetStackSize(null);
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

    public override void _Pressed()
    {
        base._Pressed();
        GD.Print("InventoryItemStack.Pressed()");
    }

    public void Selected(bool shouldBeSelected)
    {
        // Just draft code. Should probably be built with swappable resources or themes or something. 
        StyleBoxFlat styleBox = (StyleBoxFlat)_panelWithBorder.GetThemeStylebox("panel");
        if (shouldBeSelected)
        {
            styleBox.BorderColor = new Color(0, 0, 0, 255);
        }
        else
        {
            styleBox.BorderColor = new Color(255, 255, 255, 255);
        }
    }
}