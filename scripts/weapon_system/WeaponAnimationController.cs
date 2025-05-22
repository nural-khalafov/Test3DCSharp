using Godot;
using System;
using System.Threading.Tasks;

public partial class WeaponAnimationController : PlayerAnimationController
{
    public bool IsADS = false;
    public bool IsShootable = false;

    private WeaponManager _weaponManager;
    private FirstPersonController _playerController;
    private Camera3D _camera;

    private Marker3D _currentRightHandGrip;
    private Marker3D _currentLeftHandGrip;
    private Marker3D _currentAimPoint;

    

    public override void _EnterTree()
    {
        ServiceLocator.RegisterService(this);
    }

    public override void _Ready()
    {
        if (AnimationTree == null)
            GD.PrintErr("WeaponAnimationController: AnimationTree is null.");
        if (Skeleton == null)
            GD.PrintErr("WeaponAnimationController: Skeleton is null.");
        if (RightHandIK == null)
            GD.PrintErr("WeaponAnimationController: RightHandIK is null.");
        if (LeftHandIK == null)
            GD.PrintErr("WeaponAnimationController: LeftHandIK is null.");
        if (HipsIK == null)
            GD.PrintErr("WeaponAnimationController: HipsIK is null.");

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
        Weapon activeWeapon = _weaponManager.GetActiveWeapon();

        if (activeWeapon == null)
            IsADS = false;

        if (Input.IsActionPressed("aim"))
        {
            IsADS = true;
        }
        else 
        {
            // set this into comment for testing aim down sights
            IsADS = false;
        }

        SetAimingDownSightsAnimation(activeWeapon, IsADS, (float)delta);
    }


    #region Weapon Procedural Animation Methods

    /// <summary>
    /// Procedurally sets the weapon idle animation for the current weapon.
    /// </summary>
    /// <param name="isIdle"></param>
    public void SetWeaponIdleAnimation(Weapon currentWeapon, bool isIdle)
    {
        if (isIdle)
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
    public void SetAimingDownSightsAnimation(Weapon currentWeapon, bool isAiming, float delta)
    {
        if (currentWeapon == null || _camera == null)
        {
            if (_camera != null && _playerController != null)
                return;
        }

        // apply ads on weapon
        if (isAiming)
        {
            _camera.Fov = Mathf.Lerp(_camera.Fov, currentWeapon.ADSFOV, delta * 7);

            // right hand target
            currentWeapon.RightHandTarget.Position = currentWeapon.ADSRightHandPosition;
            currentWeapon.RightHandTarget.Rotation = currentWeapon.ADSRightHandRotation;

            // left hand target
            currentWeapon.LeftHandTarget.Position = currentWeapon.ADSLeftHandPosition;
            currentWeapon.LeftHandTarget.Rotation = currentWeapon.ADSLeftHandRotation;
        }
        else
        {
            // reset camera FOV
            _camera.Fov = Mathf.Lerp(_camera.Fov, currentWeapon.IdleFOV, delta * 7);
            // right hand target
            currentWeapon.RightHandTarget.Position = currentWeapon.IdleRightHandPosition;
            currentWeapon.RightHandTarget.Rotation = currentWeapon.IdleRightHandRotation;
            // left hand target
            currentWeapon.LeftHandTarget.Position = currentWeapon.IdleLeftHandPosition;
            currentWeapon.LeftHandTarget.Rotation = currentWeapon.IdleLeftHandRotation;
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
