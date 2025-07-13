using Godot;
using System;

public partial class OverworldItem() : RigidBody3D
{
	
	[Export]
	public ItemDataResource ItemDataResource { get; set; }

	public Player MoveToPlayer { get; set; }
	
	private Camera3D _camera;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_camera = (Camera3D) GetTree().GetFirstNodeInGroup("global_camera");
		MeshInstance3D meshInstance = GetNode<MeshInstance3D>("ItemMesh");
		QuadMesh mesh = (QuadMesh) meshInstance.Mesh;
		StandardMaterial3D material = (StandardMaterial3D) mesh.Material;
		material.AlbedoTexture = ItemDataResource.Texture;
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
