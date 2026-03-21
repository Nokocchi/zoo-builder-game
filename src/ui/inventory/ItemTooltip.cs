using Godot;
using System;
using ZooBuilder.globals;
using ZooBuilder.ui.inventory;

// Marked with mouse: ignore so that when you move the mouse up, you don't hit the tooltip and run InventoryItemStack.onMouseExited, followed by moving the tooltip up one pixel, followed by InventoryItemStack.onMouseEntered
// Should maybe be shown with Control.hint_tooltip, _make_custom_tooltip() instead of a custom gui_input+onMouseXXXHandler https://docs.godotengine.org/en/stable/classes/class_control.html#class-control-property-tooltip-text
public partial class ItemTooltip : Control
{
	private Label _itemNameLabel;
	private Label _itemDescriptionLabel;
	private IInventory _inventory;
	private ItemStackResource _itemStack;
	
	public override void _Ready()
	{
		_itemNameLabel = GetNode<Label>("%ItemNameLabel");
		_itemDescriptionLabel = GetNode<Label>("%ItemDescriptionLabel");
		_inventory = InventorySingleton.Instance;
		CustomMinimumSize = new Vector2(100, 50);
		_itemNameLabel.Text = _itemStack?.ItemData.ItemName;
		_itemDescriptionLabel.Text = _itemStack?.ItemData.Description;
	}

	public void SetItemStack(ItemStackResource itemStack)
	{
		_itemStack = itemStack;
	}

	
	private void HandleHoverEvent(InventoryItemStackOnHoverEvent e)
	{
		// Mouse exited, stop showing
		if (e.ItemStackResource == null)
		{
			Visible = false;
			return;
		}

		// Mouse entered, but we are holding an item. Do nothing
		if (_inventory.GetHeldItem() != null) return;
		
		// Mouse entered, and we are not holding an item. Show
		Visible = true;
		_itemNameLabel.Text = e.ItemStackResource.ItemData.ItemName;
		_itemDescriptionLabel.Text = e.ItemStackResource.ItemData.Description;
	}
	
}
