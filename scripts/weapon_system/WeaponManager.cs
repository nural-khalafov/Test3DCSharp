using Godot;
using Godot.Collections;
using System;

public enum WeaponSlot 
{
    None,
    Primary,
    Secondary,
    Pistol,
    Melee
}

public partial class WeaponManager : Node
{
    [Export] public Node3D WeaponHolderSlot;

    [ExportCategory("Weapon Slots")]
    [Export] public Node3D PrimaryWeaponSlot {  get; set; }
    [Export] public Node3D SecondaryWeaponSlot { get; set; }

    private Node3D _currentPrimaryWeapon;

    public override void _EnterTree()
    {
        GlobalSingleton.WeaponManager = this;
    }

    public void PickUpWeapon(WeaponPickup weaponPickup, Node3D weaponNode) 
    {
        if (weaponPickup == null || weaponPickup.WeaponData == null) 
        {
            GD.PrintErr("Weapon or WeaponResource is NULL");
            return;
        }

        var weaponLoader = GD.Load<PackedScene>(weaponPickup.WeaponData.WeaponPath);
        weaponNode = weaponLoader.Instantiate<Node3D>();

        if (weaponNode == null) 
        {
            GD.PrintErr("Instantiated weapon is not a Node3D");
        }

        weaponNode.Position = weaponPickup.WeaponData.WeaponPosition;
        weaponNode.Rotation = weaponPickup.WeaponData.WeaponRotation;
        weaponNode.Scale = weaponPickup.WeaponData.WeaponScale;
        WeaponHolderSlot.AddChild(weaponNode);

        GD.Print("Intantiated weapon: " + weaponPickup.WeaponData.WeaponName);
    }
}
