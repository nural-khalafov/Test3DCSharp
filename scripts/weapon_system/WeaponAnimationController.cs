using Godot;
using System;
using System.Threading.Tasks;

public partial class WeaponAnimationController : PlayerAnimationController
{
    private WeaponManager _weaponManager;
    private FirstPersonController _playerController;
    private Camera3D _camera;

    private Marker3D _currentRightHandGrip;
    private Marker3D _currentLeftHandGrip;
    private Marker3D _currentAimPoint;

    private bool _isADS = false;

    public override void _EnterTree()
    {
        ServiceLocator.RegisterService(this);

    }

    public override void _Ready()
    {
        _weaponManager = ServiceLocator.GetService<WeaponManager>();
        _playerController = ServiceLocator.GetService<FirstPersonController>();

        if (_weaponManager == null)
            GD.PrintErr("WeaponAnimationController: WeaponManager not found.");
        if (_playerController == null)
            GD.PrintErr("WeaponAnimationController: PlayerController not found.");
        else
        {
            _camera = _playerController.Camera;
            if (_camera == null)
                GD.PrintErr("WeaponAnimationController: Camera not found.");
        }
    }

    public override void _Process(double delta)
    {
    }

    #region Weapon Procedural Animation Methods

    /// <summary>
    /// Procedurally sets the weapon idle animation for the current weapon.
    /// </summary>
    /// <param name="isIdle"></param>
    public void SetWeaponIdleAnimation(Weapon currentWeapon, bool isIdle) 
    {
        if(isIdle)
        {
        }
        else
        {
        }
    }

    /// <summary>
    /// Procedurally sets the weapon shoot animation for the current weapon.
    /// </summary>
    /// <param name="currentWeapon"></param>
    public void SetWeaponShootAnimation(Weapon currentWeapon, bool isADS)
    {
        if (isADS)
        {
            // apply shoot animation for ADS
            // each weapon has its own shoot procedural animation
        }
        else
        {
            // apply shoot animation for hip fire
            // each weapon has its own shoot procedural animation
        }
    }

    /// <summary>
    /// Procedurally sets aiming down sights animation for the current weapon.
    /// </summary>
    /// <param name="isADS"></param>
    public void SetAimingDownSightsAnimation(Weapon currentWeapon, bool isADS)
    {
        if (isADS)
        {
            // apply ads on weapon
            // each weapon has its own ADS animation
        }
        else
        {
            // retrn to idle animation
            // each weapon resets to idle animation
        }
    }

    public void SetWeaponReloadInAdsAnimation(Weapon currentWeapon, bool isADS)
    {
        if (isADS)
        {
            // apply reload animation for ADS
            // each weapon has its own reload animation in ADS
        }
        else
        {
            // dont play reload animation
        }
    }

    #endregion

    #region Weapon Animation Methods

    public void PlayWeaponTacticalReloadAnimation(Weapon currentWeapon)
    {
        // Play tactical reload animation
        // This is a placeholder for the actual animation logic
    }

    public void PlayWeaponFullReloadAnimation(Weapon currentWeapon)
    {
        // Play full reload animation
        // This is a placeholder for the actual animation logic
    }

    public void PlayWeaponSprintAnimation(Weapon currentWeapon)
    {
        // Play sprint animation
        // This is a placeholder for the actual animation logic
    }

    public void PlayWeaponSwitchFireModeAnimation(Weapon currentWeapon)
    {
        // Play switch fire mode animation
        // This is a placeholder for the actual animation logic
    }

    #endregion
}
