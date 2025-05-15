using Godot;
using System;

public partial class Weapon : Node3D, IInteractable
{
    [Export] public WeaponResource WeaponData { get; set; }

    [ExportCategory("Weapon Pickup Components")]
    [Export] public Node3D WeaponObject { get; set; }
    [Export] public CollisionShape3D CollisionShape { get; set; }

    public int CurrentAmmo {  get; private set; }
    public int ReserveAmmo { get; private set; }

    public void Interact()
    {
        GD.Print("WEAPON DETECTED");
    }

    public void Interact(Node interactor)
    {
        if (interactor is Weapon weapon)
        {
            GlobalSingleton.WeaponManager.PickUpWeapon(this, WeaponObject);
            CollisionShape.Disabled = true;
            GD.Print("Interacted with: " + this.WeaponData.WeaponName);
            QueueFree();
        }
        else 
        {
            GD.Print("Interactor is not a WeaponManager or WeaponData missing");
        }
    }
}
