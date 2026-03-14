using Godot;
using System;
using ZooBuilder.entities.player;
using ZooBuilder.globals;

public partial class ItemHeldInHandMesh : MeshInstance3D
{
	private StandardMaterial3D _itemHeldInHandMaterial;
	private IInventory _inventory;

	public override void _EnterTree()
	{
		EventBus.Subscribe<SelectedHotbarSlotChangedItemEvent>(UpdateItemInHand);
	}

	public override void _ExitTree()
	{
		EventBus.Unsubscribe<SelectedHotbarSlotChangedItemEvent>(UpdateItemInHand);
	}

	private void UpdateItemInHand(SelectedHotbarSlotChangedItemEvent e)
	{
		ShowSelectedHotbarItemNextToPlayer(e.Index);
	}

	public override void _Ready()
	{
		QuadMesh quadMesh = (QuadMesh)Mesh;
		_itemHeldInHandMaterial = (StandardMaterial3D)quadMesh.Material;
		ShowSelectedHotbarItemNextToPlayer(0);
		_inventory = InventorySingleton.Instance;
	}

	private void ShowSelectedHotbarItemNextToPlayer(int hotbarIndex)
	{
		// TODO Inv: Shouldn't be necessary to check this, but it seems that this is called before _Ready()..
		if (_inventory == null)
		{
			return;
		}
		
		ItemStackResource itemStackResource = _inventory.GetInventory()[hotbarIndex];
		if (itemStackResource == null)
		{
			Visible = false;
		}
		else
		{
			Visible = true;
			_itemHeldInHandMaterial.AlbedoTexture = itemStackResource.ItemData.Texture;
		}
		
	}
	
	public override void _Process(double delta)
	{
	}
}
