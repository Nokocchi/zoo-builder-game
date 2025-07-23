using Godot;
using System;
using Godot.Collections;
using ZooBuilder.globals;

public partial class OverworldItem() : RigidBody3D
{
    [Export] public ItemStackResource ItemStackResource { get; set; }
    [Export] public Vector3 LaunchVector { get; set; } = new(0.0f, 20.0f, -8.0f);
    [Export] public PackedScene OverworldItemScene { get; set; }

    private static readonly PackedScene OverworldItemScene2 =
        GD.Load<PackedScene>("res://entities/item/overworld_item.tscn");

    public bool MoveToPlayer { get; set; }
    private Player Player { get; set; }

    private bool _hasLanded;
    public bool CanPickUp => _hasLanded;
    private Area3D _nearbyItemDetector;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _nearbyItemDetector = GetNode<Area3D>("NearbyItemDetector");
        MeshInstance3D meshInstance = GetNode<MeshInstance3D>("ItemMesh");

        // Normally, this would not be enough. The Mesh and Material would be shared and you would have to duplicate the mesh with subresources here.
        // I've marked both mesh and material as "local to scene" in the OverworldItem scene though, and that accomplishes the same.
        QuadMesh mesh = (QuadMesh)meshInstance.Mesh;
        StandardMaterial3D material = (StandardMaterial3D)mesh.Material;
        material.AlbedoTexture = ItemStackResource.ItemData.Texture;
        Player = GlobalObjectsContainer.Instance.Player;
        BodyEntered += OnContact;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        if (MoveToPlayer && CanPickUp)
        {
            Tween tween = CreateTween();
            tween.TweenProperty(this, new NodePath("position"), Player.Position, 1.0);
        }
    }

    public override void _IntegrateForces(PhysicsDirectBodyState3D state)
    {
        if (_hasLanded)
        {
            LinearDamp = float.MaxValue;
        }
    }

    public void LaunchFromPlayer()
    {
        // Move to player, and a little up to not trigger the floor collision yet
        Vector3 startPosition =
            new Vector3(Player.GlobalPosition.X, Player.GlobalPosition.Y + 1, Player.GlobalPosition.Z);
        GlobalPosition = startPosition;

        // Also works, but don't know why
        //Vector3 cameraForwards = GlobalObjectsContainer.Instance.PlayerCamera.GlobalTransform.Basis.Z.Normalized();
        //Vector3 test = new Vector3(-1 * cameraForwards.X, 1f, -1 * cameraForwards.Z);
        //ApplyCentralImpulse(20 * test);

        // No idea how this really works
        // ApplyCentralImpulse takes a vector in global space/coordinates.
        // My guess here is that since the PlayerSpringArm's forward is always in the direction we're looking, I can transform the LaunchVector by the spring arm's basis.
        Basis playerSpringArmBasis = GlobalObjectsContainer.Instance.PlayerSpringArm.Basis;
        Vector3 launchDirectionUnrotated = playerSpringArmBasis * LaunchVector;
        ApplyCentralImpulse(launchDirectionUnrotated);

        // TODO: The camera's angle affects how far you can throw your item. Maybe this should be changed so that items are always thrown at a specific angle?
        //Basis playerSpringArmBasis = GlobalObjectsContainer.Instance.PlayerSpringArm.Basis;
        // We want to make a basis whose UP is orthogonal to the ground, and Z is parallel to the ground in the direction the camera is looking
        //Vector3 BasisZ = new Vector3(playerSpringArmBasis.Z.X, 0, playerSpringArmBasis.Z.Z);
        //Basis basis2 = new Basis(Vector3.Right, Vector3.Up, BasisZ);
        //Vector3 launchDirectionUnrotated = basis2 * LaunchVector;
        //ApplyCentralImpulse(launchDirectionUnrotated);
    }

    public static void SpawnItemAndLaunchFromPlayer(ItemStackResource itemStackResource)
    {
        GD.Print("Spawned stack of ", itemStackResource.Amount);
        PackedScene overworldItemScene = GD.Load<PackedScene>("res://entities/item/overworld_item.tscn");
        OverworldItem overworldItem = overworldItemScene.Instantiate<OverworldItem>();
        overworldItem.ItemStackResource = itemStackResource;
        GlobalObjectsContainer.Instance.Game.AddChild(overworldItem);
        overworldItem.LaunchFromPlayer();
    }

    private void OnContact(Node collideWithObject)
    {
        if (collideWithObject is StaticBody3D)
        {
            _hasLanded = true;
        }
    }

    private void OnOverlappingStackZoneEntered(Node3D node)
    {
        Array<Node3D> overlappingBodies = _nearbyItemDetector.GetOverlappingBodies();
        foreach (Node3D overlappingBody in overlappingBodies)
        {
            if (overlappingBody is not OverworldItem overworldItem) continue;
            if (!overworldItem.CanPickUp || overworldItem.ItemStackResource.ItemData !=
                ItemStackResource.ItemData) continue;
            // TODO: Could we get a concurrency issue here where two items overlap, and both call this method, and one calls QueueFree while the other one expects it to still exist?
            AbsorbAndDeleteItemStack(overworldItem);
        }
    }

    private void AbsorbAndDeleteItemStack(OverworldItem overworldItem)
    {
        ItemStackResource.Amount += overworldItem.ItemStackResource.Amount;
        overworldItem.QueueFree();
    }
}