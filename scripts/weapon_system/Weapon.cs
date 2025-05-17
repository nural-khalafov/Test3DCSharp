using Godot;
using System;

public partial class Weapon : RigidBody3D, IInteractable
{
    [Export] public WeaponResource WeaponData { get; set; }

    [ExportCategory("Weapon Pickup Components")]
    [Export] public Node3D WeaponObject { get; set; }
    [Export] public CollisionShape3D CollisionShape { get; set; }

    public int CurrentAmmo {  get; private set; }
    public int ReserveAmmo { get; private set; }

    public override void _Ready()
    {
        if(CollisionShape == null) 
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
        if (GlobalSingleton.WeaponManager != null)
        {
            var pickUpCommand = new PickUpAndEquipCommand(this);
            GlobalSingleton.WeaponManager.ProcessCommand(pickUpCommand);
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
}
