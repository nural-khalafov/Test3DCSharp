using Godot;
using System;

public partial class Weapon : RigidBody3D, IInteractable
{
    [Export] public WeaponResource WeaponData { get; set; }

    [ExportCategory("Weapon Pickup Components")]
    [Export] public Node3D WeaponObject { get; set; }
    [Export] public CollisionShape3D CollisionShape { get; set; }
    [Export] public Marker3D LeftHandTarget { get; set; }
    [Export] public Marker3D RightHandTarget { get; set; }

    [ExportCategory("Shooting Mechanics Data")]
    [Export] public Marker3D MuzzlePoint { get; set; }
    [Export] public PackedScene BulletScene { get; set; }

    [ExportCategory("Procedural Idle Animation Data")]
    [Export] public float IdleFOV { get; set; } = 75f;
    [Export] public Vector3 IdleRightHandPosition { get; set; }
    [Export] public Vector3 IdleRightHandRotation { get; set; }
    [Export] public Vector3 IdleLeftHandPosition { get; set; }
    [Export] public Vector3 IdleLeftHandRotation { get; set; }

    [ExportCategory("Procedural Aiming Down Sights Data")]
    [Export] public float ADSFOV { get; set; } = 40f;
    [Export] public Vector3 ADSRightHandPosition { get; set; }
    [Export] public Vector3 ADSRightHandRotation { get; set; }
    [Export] public Vector3 ADSLeftHandPosition { get; set; }
    [Export] public Vector3 ADSLeftHandRotation { get; set; }

    [ExportGroup("Weapon Stats")]
    [Export] public int CurrentAmmo { get; set; } = 30;
    [Export] public int ReserveAmmo { get; set; } = 90;
    [Export] public int MagazineSize { get; set; } = 30;

    private WeaponManager _weaponManager;

    public override void _Ready()
    {
        _weaponManager = ServiceLocator.GetService<WeaponManager>();

        if (CollisionShape == null) 
        {
            CollisionShape = GetNodeOrNull<CollisionShape3D>("CollisionShape3D");
        }
    }

    public void Interact()
    {
        GD.Print("WEAPON DETECTED (Interact without interactor)");
    }

    public void Interact(Node interactor)
    {
        if (_weaponManager != null)
        {
            var pickUpCommand = new PickUpAndEquipCommand(this);
            _weaponManager.ProcessCommand(pickUpCommand);
        }
        else 
        {
            GD.PrintErr("WeaponManager is not in GlobalSingleton");
        }
    }

    /// <summary>
    /// Sets the weapon to be held or not.
    /// </summary>
    /// <param name="held">True - if weapon is in arms, false - if in the world map</param>
    public void SetHeld(bool held) 
    {
        if(held) 
        {
            this.Freeze = true;
            this.FreezeMode = RigidBody3D.FreezeModeEnum.Static;
            if(CollisionShape != null) 
            {
                CollisionShape.Disabled = true;
            }
        }
        else 
        {
            this.Freeze = false;
            this.FreezeMode = RigidBody3D.FreezeModeEnum.Kinematic;
            if (CollisionShape != null)
            {
                CollisionShape.Disabled = false;
            }
        }
    }

    public void Shoot() 
    {
        if (WeaponData == null)
        {
            GD.PrintErr("WeaponData is not set for the weapon. Cannot shoot.");
            return;
        }
        if (BulletScene == null) 
        {
            GD.PrintErr("BulletScene is not set for the weapon. Cannot shoot.");
            return;
        }
        if(MuzzlePoint == null) 
        {
            GD.PrintErr("MuzzlePoint is not set for the weapon. Cannot shoot.");
            return;
        }
        if(CurrentAmmo <= 0)
        {
            GD.Print($"{Name}: Out of ammo!");
            return;
        }

        CurrentAmmo--;
        GD.Print($"{Name}: Shooting! Ammo left: {CurrentAmmo}/{MagazineSize}");

        Bullet bulletInstance = BulletScene.Instantiate<Bullet>();

        if (bulletInstance == null)
        {
            GD.PrintErr("Failed to instantiate bullet from BulletScene.");
            return;
        }

        GetTree().Root.AddChild(bulletInstance);

        GD.Print($"Bullet instantiated at position: {MuzzlePoint.Rotation}");

        bulletInstance.GlobalTransform = MuzzlePoint.GlobalTransform;
        bulletInstance.Fire();
        bulletInstance.Initialize(bulletInstance.Damage, bulletInstance.Caliber);
    }
}
