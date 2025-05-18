using Godot;
using System;
using System.Threading.Tasks;

public partial class WeaponAnimationController : PlayerAnimationController
{
    private WeaponManager _weaponManager;
    private FirstPersonController _playerController;
    private Camera3D _camera;

    public override void _Ready()
    {
    }
}
