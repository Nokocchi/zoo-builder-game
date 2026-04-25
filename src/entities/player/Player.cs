using Godot;
using System;
using Godot.Collections;
using GodotSteam;
using ZooBuilder.data.stats;
using ZooBuilder.globals;

public partial class Player : CharacterBody3D
{
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
    private PlayerSpringArm _playerSpringArm;
    private Area3D _itemPullZone;
    private Area3D _itemPickupZone;
    private IInventory _inventorySingleton;
    private Camera3D _playerCamera;
    private AudioStreamPlayer _itemPickupAudioPlayer;
    private Node3D _pivot;
    private GlobalObjectsContainer _globals;
    private ItemHeldInHandMesh _itemHeldInHandMesh;
    private AnimationPlayer _animationPlayer;
    
    [Signal]
    public delegate void HitEventHandler();

    private Vector3 _targetVelocity = Vector3.Zero;

    public override void _Ready()
    {
        _globals = GlobalObjectsContainer.Instance;
        _state = State.WALKING;
        _itemPickupAudioPlayer = GetNode<AudioStreamPlayer>("ItemPickupAudioPlayer");
        _playerSpringArm = GetNode<PlayerSpringArm>("PlayerSpringArm");
        _itemPullZone = GetNode<Area3D>("ItemPullZone");
        _itemPickupZone = GetNode<Area3D>("ItemImmediatePickupZone");
        _inventorySingleton = InventorySingleton.Instance;
        _pivot = GetNode<Node3D>("Pivot");
        _globals.Player = this;
        GlobalPosition = _globals.GameData.PlayerGlobalPosition;
        _pivot.Rotation = _globals.GameData.PlayerRotation;
        _itemHeldInHandMesh = GetNode<ItemHeldInHandMesh>("%ItemHeldInHandMesh");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    private void ItemPullZoneBodyEntered(Node3D ignored)
    {
        MoveNearbyItemsToPlayer();
    }

    private void MoveNearbyItemsToPlayer()
    {
        Array<Node3D> overlappingBodies = _itemPullZone.GetOverlappingBodies();
        foreach (Node3D overlappingBody in overlappingBodies)
        {
            if (overlappingBody is OverworldItem {CanPickUp: true} overworldItem)
            {
                overworldItem.MoveToPlayer = true;
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
            if (overlappingBody is not OverworldItem { CanPickUp: true } overworldItem) continue;
            // This check is to avoid picking up the same item multiple times
            if(overworldItem.IsQueuedForDeletion()) continue;
            bool pickedUp = InventorySingleton.Instance.TryAddItem(overworldItem.ItemStackResource);
            if (pickedUp)
            {
                _itemPickupAudioPlayer.Playing = true;
                overworldItem.QueueFree();
            }
            else
            {
                // Could not pick up item. Stop moving the item towards the player.
                overworldItem.MoveToPlayer = false;
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        //DrawLine3D.Instance.DrawDebugLines_Basis(_pivot, delta);
        _globals.GameData.PlayerGlobalPosition = GlobalPosition;
        
        // We create a local variable to store the input direction.
        Vector3 direction = Vector3.Zero;

        // We do not want to update the player's direction or speed while menu is open. Just animate and process physics
        if (!UIManager.IsMenuOpen())
        {
            // We check for each move input and update the direction accordingly.
            if (Input.IsActionPressed(GlobalDataSingleton.ACTION_MOVE_RIGHT))
            {
                direction.X += 1.0f;
            }

            if (Input.IsActionPressed(GlobalDataSingleton.ACTION_MOVE_LEFT))
            {
                direction.X -= 1.0f;
            }

            if (Input.IsActionPressed(GlobalDataSingleton.ACTION_MOVE_BACK))
            {
                direction.Z += 1.0f;
            }

            if (Input.IsActionPressed(GlobalDataSingleton.ACTION_MOVE_FORWARD))
            {
                direction.Z -= 1.0f;
            }
        }

        // UP => Y=1
        // Rotate direction vector by the direction the camera is facing
        // TODO: Does it make more sense to rotate the player's basis such that its Z is in the direction of the SpringArm? 
        // Do we risk that looking up or down with the camera affects the forwards movement speed?
        direction = direction.Rotated(Vector3.Up, _playerSpringArm.Rotation.Y);

        if (direction != Vector3.Zero)
        {
            direction = direction.Normalized();
            // Setting the basis property will affect the rotation of the node.
            _pivot.Basis = Basis.LookingAt(direction);
            _globals.GameData.PlayerRotation = _pivot.Rotation;
            _animationPlayer.SpeedScale = 4;
        }
        else
        {
            _animationPlayer.SpeedScale = 1;
        }

        int speed = Input.IsActionPressed(GlobalDataSingleton.ACTION_RUN) ? RunSpeed : Speed;
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
        if (IsOnFloor() && Input.IsActionJustPressed(GlobalDataSingleton.ACTION_JUMP))
        {
            _targetVelocity.Y = JumpImpulse;
        }

        MoveAndSlide();

        // There seems to be some residual Y value from previous frames when we landed on the ground. 
        // Either way, I assume we only want to count the XZ distance covered..
        SteamDataCache.DistanceWalked += new Vector3(_targetVelocity.X, 0.0f, _targetVelocity.Z).Length();
        Steam.SetStatFloat(SteamStatNames.FloatStats.DistanceWalkedStatName, SteamDataCache.DistanceWalked);
        
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
                    _targetVelocity.Y = BounceImpulse;
                    // Prevent further duplicate calls.
                    break;
                }
            }
        }
        
        _pivot.Rotation = new Vector3(Mathf.Pi / 6.0f * Velocity.Y / JumpImpulse, _pivot.Rotation.Y, _pivot.Rotation.Z);
    }
}