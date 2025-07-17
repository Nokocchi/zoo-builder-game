using Godot;
using System;

public partial class OverworldItem() : RigidBody3D
{
	
	[Export]
	public ItemStackResource ItemStackResource { get; set; }

	public Player MoveToPlayer { get; set; }
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MeshInstance3D meshInstance = GetNode<MeshInstance3D>("ItemMesh");

		// Normally, this would not be enough. The Mesh and Material would be shared and you would have to duplicate the mesh with subresources here.
		// I've marked both mesh and material as "local to scene" in the OverworldItem scene though, and that accomplishes the same.
		QuadMesh mesh = (QuadMesh) meshInstance.Mesh;
		StandardMaterial3D material = (StandardMaterial3D) mesh.Material;
		material.AlbedoTexture = ItemStackResource.ItemData.Texture;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (MoveToPlayer != null)
		{
			Tween tween = CreateTween();
			tween.TweenProperty(this, new NodePath("position"), MoveToPlayer.Position, 1.0);
		}


	}
}
