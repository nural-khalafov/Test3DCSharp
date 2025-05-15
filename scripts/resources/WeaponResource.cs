using Godot;
using System;

public enum WeaponType
{
    None,
    Melee,
    Pistol,
    AutomaticRifle
}

[GlobalClass]
public partial class WeaponResource : Resource
{
    [ExportCategory("Weapon Data")]
    [Export] public StringName WeaponName { get; set; } = "";
    [Export] public WeaponType WeaponType { get; set; } = WeaponType.None;
    [Export] public StringName WeaponCaliber { get; set; }
    [Export] public float WeaponDamage { get; set; }

    [ExportCategory("Weapon Transform Data")]
    [Export] public Vector3 WeaponPosition { get; set; }
    [Export] public Vector3 WeaponRotation { get; set; }
    [Export] public Vector3 WeaponScale { get; set; }

    [ExportCategory("Left Hand Transform Data")]
    [Export] public Vector3 LeftHandPosition { get; set; }
    [Export] public Vector3 LeftHandRotation { get; set; }

    [ExportCategory("Weapon Visual Data")]
    [Export] public PackedScene WeaponPacketScene { get; set; } = null;

    [ExportCategory("Weapon Path")]
    [Export] public string WeaponPath = null;


    public WeaponResource()
    {
        WeaponName = new StringName();
        WeaponType = WeaponType.None;

        WeaponPosition = new Vector3();
        WeaponRotation = new Vector3();
        WeaponScale = new Vector3();

        WeaponPacketScene = new PackedScene();
    }

    public WeaponResource(StringName weaponName, WeaponType weaponType,
        Vector3 weaponPos, Vector3 weaponRot,
        Vector3 lHandPos, Vector3 lHandRot)
    {
        WeaponName = weaponName;
        WeaponType = weaponType;

        WeaponPosition = weaponPos;
        WeaponRotation = weaponRot;

        LeftHandPosition = lHandPos;
        LeftHandRotation = lHandRot;
    }
}
