using Godot;
using System;
using ZooBuilder.globals;

// Is control so that it renders on top of other menus, like Inventory
public partial class MouseWithItemMarker : Control
{
    private InventoryItemStack _itemStack;

    public InventoryItemStack ItemStack => _itemStack;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GlobalObjectsContainer.Instance.MouseWithMarker = this;
        _itemStack = GetNode<InventoryItemStack>("InventoryItemStack");
        Visible = false;
    }

    public void HoldItemStack(InventoryItemStack holdStack)
    {
        if (holdStack == null)
        {
            return;
        }

        Visible = true;
        _itemStack.ItemStackResource = holdStack.ItemStackResource;
        _itemStack.InventoryIndex = holdStack.InventoryIndex;
    }

    public void ClearItemStack()
    {
        Visible = false;
        _itemStack.ItemStackResource = null;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion eventMouseMotion)
        {
            // Itemstack's bottom left corner follows mouse. When clicking, you technically click the InventoryItemStack, but this is marked as mouse = ignore. 
            Position = new Vector2(eventMouseMotion.Position.X, (eventMouseMotion.Position.Y - _itemStack.Size.Y));
        }
    }
}