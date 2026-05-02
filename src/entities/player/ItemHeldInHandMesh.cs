using Godot;
using System;
using ZooBuilder.entities.player;
using ZooBuilder.globals;

// Strategy: Save the InventorySlotResource that is currently selected/highlighted in the hotbar.
// All changes to the item data inside is listened to by the Render() method, keeping the visuals of this node up to date
public partial class ItemHeldInHandMesh : MeshInstance3D
{
	private StandardMaterial3D _itemHeldInHandMaterial;
	private InventorySlotResource _slotResource;

	public override void _EnterTree()
	{
		EventBus.Subscribe<SelectedHotbarSlotChangedItemEvent>(UpdateItemInHand);
		EventBus.Subscribe<GameFinishedLoadingEvent>(OnSaveDataLoaded);
	}

	public override void _ExitTree()
	{
		EventBus.Unsubscribe<SelectedHotbarSlotChangedItemEvent>(UpdateItemInHand);
		EventBus.Unsubscribe<GameFinishedLoadingEvent>(OnSaveDataLoaded);
	}

	public override void _Ready()
	{
		QuadMesh quadMesh = (QuadMesh)Mesh;
		_itemHeldInHandMaterial = (StandardMaterial3D)quadMesh.Material;
	}

	private void OnSaveDataLoaded(GameFinishedLoadingEvent e)
	{
		UpdateItemInHand(0);
	}

	private void UpdateItemInHand(SelectedHotbarSlotChangedItemEvent e)
	{
		UpdateItemInHand(e.Index);
	}

	private void UpdateItemInHand(int newSelectedItemIndex)
	{
		if (_slotResource != null)
		{
			_slotResource.SlotContentChanged -= Render;
		}
		
		_slotResource = InventorySingleton.Instance.GetInventory()[newSelectedItemIndex];
		_slotResource.SlotContentChanged += Render;
		Render();
	}
	
	private void Render()
	{
		if (_slotResource.IsEmpty())
		{
			Visible = false;
		}
		else
		{
			Visible = true;
			_itemHeldInHandMaterial.AlbedoTexture = _slotResource.GetItem().ItemData.Texture;
		}
	}

	public override void _Process(double delta)
	{
		if (_slotResource == null) return;
	}
}
