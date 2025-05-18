using Godot;
using System;

[GlobalClass]
public partial class GlobalSingleton : Node
{
    public static WeaponManager WeaponManager;
    public static FirstPersonController PlayerController;
    public static PlayerAnimationController PlayerAnimationController;
}
