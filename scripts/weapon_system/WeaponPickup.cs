using Godot;
using System;

public partial class WeaponPickup : Node3D, IInteractable
{
    [Export] public WeaponResource WeaponData { get; set; }
    [Export] public Node3D WeaponObject { get; set; }

    public int CurrentAmmo {  get; private set; }
    public int ReserveAmmo { get; private set; }

    public void Interact()
    {
        GD.Print("WEAPON DETECTED");
    }

    public void Interact(Node interactor)
    {
        if (interactor is WeaponPickup weapon)
        {
            GlobalSingleton.WeaponManager.PickUpWeapon(this, WeaponObject);
            GD.Print("Interacted with: " + this.WeaponData.WeaponName);
            QueueFree();
        }
        else 
        {
            GD.Print("Interactor is not a WeaponManager or WeaponData missing");
        }
    }
}
