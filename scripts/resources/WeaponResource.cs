using Godot;
using System;

public enum WeaponType
{
    None,
    Melee,
    Pistol,
    AssaultRifle
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

    [ExportCategory("Weapon Path")]
    [Export] public string WeaponPath = null;

    [ExportCategory("Procedural Idle Animation Data")]
    [Export] public Vector3 IdlePosition { get; set; }
    [Export] public Vector3 IdleRotation { get; set; }
    [Export] public float IdleFOV { get; set; } = 70f;

    [ExportCategory("Procedural Aiming Down Sights Data")]
    [Export] public Vector3 ADSPosition { get; set; }
    [Export] public Vector3 ADSRotation { get; set; }
    [Export] public float ADSFOV { get; set; } = 40f;
}
