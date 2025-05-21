using Godot;
using System;

public partial class PlayerUpperbodyState : State
{
    public FirstPersonController PlayerController;
    public WeaponAnimationController WeaponAnimationController;

    public override async void _Ready()
    {
        await ToSignal(Owner, "ready");
        PlayerController = Owner as FirstPersonController;
        WeaponAnimationController = ServiceLocator.GetService<WeaponAnimationController>();
    }
    public override void _Process(double delta)
    {
        // NULL
    }
}
