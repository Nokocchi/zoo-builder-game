using Godot;
using System;
using ZooBuilder.globals;
using ZooBuilder.ui.inventory;

// Marked with mouse: ignore so that when you move the mouse up, you don't hit the tooltip and run InventoryItemStack.onMouseExited, followed by moving the tooltip up one pixel, followed by InventoryItemStack.onMouseEntered
// Should maybe be shown with Control.hint_tooltip, _make_custom_tooltip() instead of a custom gui_input+onMouseXXXHandler https://docs.godotengine.org/en/stable/classes/class_control.html#class-control-property-tooltip-text
public partial class ItemTooltip : PanelContainer
{
	private Label _itemNameLabel;
	private Label _itemDescriptionLabel;
	private IInventory _inventory;
	private ItemStackResource _itemStack;
	private static readonly PackedScene TooltipScene = GD.Load<PackedScene>("res://src/ui/ingamemenu/inventory/item_tooltip.tscn");
	
	public override void _Ready()
	{
		_itemNameLabel = GetNode<Label>("%ItemNameLabel");
		_itemDescriptionLabel = GetNode<Label>("%ItemDescriptionLabel");
		_inventory = InventorySingleton.Instance;
		_itemNameLabel.Text = _itemStack?.ItemData.ItemName;
		_itemDescriptionLabel.Text = _itemStack?.ItemData.Description;
	}

	public static ItemTooltip CreateWithData(ItemStackResource itemStack)
	{
		ItemTooltip t = TooltipScene.Instantiate<ItemTooltip>();
		t._itemStack = itemStack;
		return t;
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
