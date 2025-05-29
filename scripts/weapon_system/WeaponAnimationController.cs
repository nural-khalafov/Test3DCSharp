using Godot;
using System;
using System.Threading.Tasks;

public partial class WeaponAnimationController : PlayerAnimationController
{
    public bool IsADS = false;
    public bool IsShootable = true;

    /// <summary>
    /// Weapon Sway Variables
    /// </summary>
    [Export] private float _swayIntensity = 0.001f;
    [Export] private float _maxSwayOffset = 0.001f;
    [Export] private float _swaySmoothing = 10.0f;
    [Export] private float _adsTransitionSpeed = 15.0f;

    private Vector3 _currentSwayOffset = Vector3.Zero;

    /// <summary>
    /// Service Locator Components
    /// </summary>
    private WeaponManager _weaponManager;
    private FirstPersonController _playerController;
    private Camera3D _camera;
    private Weapon _currentWeapon;
    private CenterDot _centerDot;

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
        _centerDot = ServiceLocator.GetService<CenterDot>();

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
        _currentWeapon = _weaponManager.GetActiveWeapon();

        if (_currentWeapon == null)
            IsADS = false;

        if (Input.IsActionPressed("aim") && _currentWeapon != null && _currentWeapon.WeaponData.WeaponType != WeaponType.Melee)
        {
            IsADS = true;
        }
        else 
        {
            // set this into comment for setting-up aim down sights animation for weapons
            IsADS = false;
        }

        if (Input.IsActionJustPressed("shoot")) 
        {
            HandleShootInput();
        }

        CalculateWeaponSway((float)delta);

        SetAimingDownSightsAnimation(_currentWeapon, IsADS, (float)delta);
    }

    public override void _PhysicsProcess(double delta)
    {
    }

    private void HandleShootInput() 
    {
        if(_currentWeapon != null && _currentWeapon.WeaponData.WeaponType != WeaponType.Melee && IsShootable)
        {
            _currentWeapon.Shoot();
            SetWeaponShootAnimation(_currentWeapon, IsADS);
        }
    }


    #region Weapon Procedural Animation Methods

    // for later implementation
    private void CalculateWeaponSway(float delta)
    {
        //if (_playerController == null)
        //    return;

        //float mouseX = _playerController.FrameRotationInput;
        //float mouseY = _playerController.FrameTiltInput;

        //Vector3 targetSwayOffset;

        //if (IsADS && _currentWeapon != null) 
        //{
        //    targetSwayOffset = new Vector3(
        //        Mathf.Clamp(mouseX * _swayIntensity, -_maxSwayOffset, _maxSwayOffset),
        //        Mathf.Clamp(-mouseY * _swayIntensity, -_maxSwayOffset, _maxSwayOffset),
        //        0);
        //}
        //else 
        //{
        //    targetSwayOffset = Vector3.Zero;
        //}

        //_currentSwayOffset = _currentSwayOffset.Lerp(targetSwayOffset, _swaySmoothing * delta);

        //if(!IsADS || _currentWeapon == null)
        //{
        //    _currentSwayOffset = _currentSwayOffset.Lerp(Vector3.Zero, _swaySmoothing * delta);
        //    GD.Print(_currentSwayOffset);
        //    return;
        //}

        //Vector2 mouseDelta = Input.GetLastMouseVelocity();

        //float intensity = _swayIntensity;
        //if(_playerController != null)
        //    intensity *= _playerController.MouseSensitivity;

        //Vector3 targetSwayOffset = new Vector3(
        //    Mathf.Clamp(-mouseDelta.X * intensity, -_maxSwayOffset, _maxSwayOffset),
        //    Mathf.Clamp(-mouseDelta.Y * intensity, -_maxSwayOffset, _maxSwayOffset),
        //    0);

        //_currentSwayOffset = _currentSwayOffset.Lerp(targetSwayOffset, _swaySmoothing * delta);

        //GD.Print(_currentSwayOffset);

        if (_currentWeapon != null)
        {
        }
    }

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
        if (currentWeapon == null)
            return;

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
        float defaultPlayerFOV = 75f;

        if(_camera == null)
            return;

        if (currentWeapon == null) 
        {
            _camera.Fov = Mathf.Lerp(_camera.Fov, defaultPlayerFOV, delta * 7);

            return;
        }

        Vector3 targetRightHandPosition;
        Vector3 targetRightHandRotation;
        Vector3 targetLeftHandPosition;
        Vector3 targetLeftHandRotation;

        // apply ads on weapon
        if (isAiming && currentWeapon.WeaponData.WeaponType != WeaponType.Melee)
        {
            _centerDot.Visible = false;
            _camera.Fov = Mathf.Lerp(_camera.Fov, currentWeapon.ADSFOV, delta * 7);

            // right hand target
            targetRightHandPosition = currentWeapon.ADSRightHandPosition;
            targetRightHandRotation = currentWeapon.ADSRightHandRotation;

            // left hand target
            targetLeftHandPosition = currentWeapon.ADSLeftHandPosition;
            targetLeftHandRotation = currentWeapon.ADSLeftHandRotation;
        }
        else
        {
            _centerDot.Visible = true;

            // reset camera FOV
            _camera.Fov = Mathf.Lerp(_camera.Fov, currentWeapon.IdleFOV, delta * 7);
            // right hand target
            targetRightHandPosition = currentWeapon.IdleRightHandPosition;
            targetRightHandRotation = currentWeapon.IdleRightHandRotation;

            // left hand target
            targetLeftHandPosition = currentWeapon.IdleLeftHandPosition;
            targetLeftHandRotation = currentWeapon.IdleLeftHandRotation;
        }

        if(currentWeapon.RightHandTarget != null)
        {
            Vector3 finalRightHandPosition = targetRightHandPosition + (isAiming ? _currentSwayOffset : Vector3.Zero);
            currentWeapon.RightHandTarget.Position =
                currentWeapon.RightHandTarget.Position.Lerp(finalRightHandPosition, _adsTransitionSpeed * delta);
            currentWeapon.RightHandTarget.Rotation =
                currentWeapon.RightHandTarget.Rotation.Lerp(targetRightHandRotation, _adsTransitionSpeed * delta);
        }

        if(currentWeapon.LeftHandTarget != null)
        {
            Vector3 finalLeftHandPosition = targetLeftHandPosition + (isAiming ? _currentSwayOffset : Vector3.Zero);
            currentWeapon.LeftHandTarget.Position =
                currentWeapon.LeftHandTarget.Position.Lerp(finalLeftHandPosition, _adsTransitionSpeed * delta);
            currentWeapon.LeftHandTarget.Rotation =
                currentWeapon.LeftHandTarget.Rotation.Lerp(targetLeftHandRotation, _adsTransitionSpeed * delta);
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
