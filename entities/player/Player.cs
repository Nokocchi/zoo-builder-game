using Godot;
using System;
using Godot.Collections;
using ZooBuilder.globals;

public partial class Player : CharacterBody3D
{
    // Don't forget to rebuild the project so the editor knows about the new export variable.

    // How fast the player moves in meters per second.
    [Export] public int Speed { get; set; } = 14;
    [Export] public int RunSpeed { get; set; } = 25;

    // The downward acceleration when in the air, in meters per second squared.
    [Export] public int FallAcceleration { get; set; } = 75;

    // Vertical impulse applied to the character upon jumping in meters per second.
    [Export] public int JumpImpulse { get; set; } = 20;

    // Vertical impulse applied to the character upon bouncing over a mob in meters per second.
    [Export] public int BounceImpulse { get; set; } = 16;

    private enum State
    {
        WALKING,
        IN_AIR
    }

    private State _state;
    private SettingsSingleton _settings;
    private SpringArm3D _playerSpringArm;
    private Area3D _itemPullZone;
    private Area3D _itemPickupZone;
    private ItemDataResource _appleResource;
    private ItemDataResource _orangeResource;
    private ItemDataResource _bananaResource;

    [Signal]
    public delegate void HitEventHandler();

    private Vector3 _targetVelocity = Vector3.Zero;

    public override void _Ready()
    {
        _state = State.WALKING;
        _playerSpringArm = GetNode<SpringArm3D>("PlayerSpringArm");
        _itemPullZone = GetNode<Area3D>("ItemPullZone");
        _itemPickupZone = GetNode<Area3D>("ItemImmediatePickupZone");
        _appleResource = ResourceLoader.Load<ItemDataResource>("res://entities/item/item_apple.tres");
        _orangeResource = ResourceLoader.Load<ItemDataResource>("res://entities/item/item_orange.tres");
        _bananaResource = ResourceLoader.Load<ItemDataResource>("res://entities/item/item_banana.tres");
        _settings = SettingsSingleton.Load();
        GlobalObjectsContainer.Instance.Player = this;
    }

    private void Die()
    {
        EmitSignal(SignalName.Hit);
        QueueFree();
    }

    // We also specified this function name in PascalCase in the editor's connection window.
    private void OnMobDetectorBodyEntered(Node3D body)
    {
        //Die();
    }

    private void ItemPullZoneBodyEntered(Node3D body)
    {
        MoveNearbyItemsToPlayer();
    }

    private void MoveNearbyItemsToPlayer()
    {
        Array<Node3D> overlappingBodies = _itemPullZone.GetOverlappingBodies();
        foreach (Node3D overlappingBody in overlappingBodies)
        {
            if (overlappingBody is OverworldItem overworldItem)
            {
                overworldItem.MoveToPlayer = this;
            }
        }
    }

    private void ItemImmediatePickupZoneBodyEntered(Node3D body)
    {
        PickupNearbyItems();
    }

    // Immediately picks up everything within a short radius
    private void PickupNearbyItems()
    {
        Array<Node3D> overlappingBodies = _itemPickupZone.GetOverlappingBodies();
        foreach (Node3D overlappingBody in overlappingBodies)
        {
            if (overlappingBody is OverworldItem overworldItem)
            {
                InventorySingleton.Instance.AddItem(overworldItem.ItemStackResource);
                overworldItem.QueueFree();
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        // We create a local variable to store the input direction.
        Vector3 direction = Vector3.Zero;

        InventorySingleton inventorySingleton = InventorySingleton.Instance;

        // We do not want to update the player's direction or speed while menu is open. Just animate and process physics
        if (!inventorySingleton.MenuOpen)
        {
            // We check for each move input and update the direction accordingly.
            if (Input.IsActionPressed("move_right"))
            {
                direction.X += 1.0f;
            }

            if (Input.IsActionPressed("move_left"))
            {
                direction.X -= 1.0f;
            }

            if (Input.IsActionPressed("move_back"))
            {
                // Notice how we are working with the vector's X and Z axes.
                // In 3D, the XZ plane is the ground plane.
                direction.Z += 1.0f;
            }

            if (Input.IsActionPressed("move_forward"))
            {
                direction.Z -= 1.0f;
            }
        }

        // UP => Y=1
        // Rotate direction vector by the direction the camera is facing
        direction = direction.Rotated(Vector3.Up, _playerSpringArm.Rotation.Y);

        if (direction != Vector3.Zero)
        {
            direction = direction.Normalized();
            // Setting the basis property will affect the rotation of the node.
            GetNode<Node3D>("Pivot").Basis = Basis.LookingAt(direction);
            GetNode<AnimationPlayer>("AnimationPlayer").SpeedScale = 4;
        }
        else
        {
            GetNode<AnimationPlayer>("AnimationPlayer").SpeedScale = 1;
        }

        int speed = Input.IsActionPressed("run") ? RunSpeed : Speed;
        // Ground velocity
        _targetVelocity.X = direction.X * speed;
        _targetVelocity.Z = direction.Z * speed;

        // Vertical velocity
        if (!IsOnFloor()) // If in the air, fall towards the floor. Literally gravity
        {
            _targetVelocity.Y -= FallAcceleration * (float)delta;
        }

        // Moving the character
        Velocity = _targetVelocity;

        // Jumping.
        if (IsOnFloor() && Input.IsActionJustPressed("jump"))
        {
            _targetVelocity.Y = JumpImpulse;
        }

        MoveAndSlide();

        // Iterate through all collisions that occurred this frame.
        for (int index = 0; index < GetSlideCollisionCount(); index++)
        {
            // We get one of the collisions with the player.
            KinematicCollision3D collision = GetSlideCollision(index);

            // If the collision is with a mob.
            // With C# we leverage typing and pattern-matching
            // instead of checking for the group we created.
            if (collision.GetCollider() is Mob mob)
            {
                // We check that we are hitting it from above.
                if (Vector3.Up.Dot(collision.GetNormal()) > 0.1f)
                {
                    // If so, we squash it and bounce.
                    mob.Squash();
                    int item = new Random().Next(3);

                    switch (item)
                    {
                        case 1:
                            inventorySingleton.AddItem(new ItemStackResource(_appleResource, 1));
                            break;
                        case 2:
                            inventorySingleton.AddItem(new ItemStackResource(_orangeResource, 1));
                            break;
                        default:
                            inventorySingleton.AddItem(new ItemStackResource(_bananaResource, 1));
                            break;
                    }


                    _targetVelocity.Y = BounceImpulse;
                    // Prevent further duplicate calls.
                    break;
                }
            }
        }

        Node3D pivot = GetNode<Node3D>("Pivot");
        pivot.Rotation = new Vector3(Mathf.Pi / 6.0f * Velocity.Y / JumpImpulse, pivot.Rotation.Y, pivot.Rotation.Z);
    }
}