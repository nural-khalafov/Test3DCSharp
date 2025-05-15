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
    public Dictionary<WeaponSlot, Node3D> WeaponSlots;

    public override void _EnterTree()
    {
        GlobalSingleton.WeaponManager = this;

        WeaponSlots = new Dictionary<WeaponSlot, Node3D>()
        {
            { WeaponSlot.Primary, null }
        };
    }

    public override void _Process(double delta)
    {

    }

    public void PickUpWeapon(Weapon weapon, Node3D weaponNode)
    {
        if (weapon == null || weapon.WeaponData == null)
        {
            GD.PrintErr("Weapon or WeaponResource is NULL");
            return;
        }

        var weaponLoader = GD.Load<PackedScene>(weapon.WeaponData.WeaponPath);
        weaponNode = weaponLoader.Instantiate<Node3D>();

        if (weaponNode == null)
        {
            GD.PrintErr("Instantiated weapon is not a Node3D");
        }

        weaponNode.Position = weapon.WeaponData.WeaponPosition;
        weaponNode.Rotation = weapon.WeaponData.WeaponRotation;
        weaponNode.Scale = weapon.WeaponData.WeaponScale;
        WeaponHolderSlot.AddChild(weaponNode);

        GD.Print("Intantiated weapon: " + weapon.WeaponData.WeaponName);
    }

    public void EquipWeaponToSlot(Node3D slot, ref Node3D currentWeapon, Node3D newWeapon)
    {
        if (currentWeapon != null && currentWeapon.IsInsideTree())
        {
            currentWeapon.QueueFree();
            // implement drop weapon logic later
        }

        slot.AddChild(newWeapon);
        newWeapon.GlobalTransform = slot.GlobalTransform;
        currentWeapon = newWeapon;
    }
}
