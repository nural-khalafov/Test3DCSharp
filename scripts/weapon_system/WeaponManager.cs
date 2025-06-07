using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
    [Export] public Godot.Collections.Dictionary<WeaponSlot, Weapon> WeaponSlots { get; private set; }

    // Events
    public event Action<WeaponSlot, Weapon> OnWeaponSwitched;

    // Private fields
    public WeaponSlot CurrentSlot = WeaponSlot.None;

    // References 
    private PlayerAnimationController _playerAnimationController;
    private WeaponAnimationController _weaponAnimationController;

    private readonly Dictionary<string, WeaponSlot> _actionToSlotMap = new()
    {
        { "empty_hands", WeaponSlot.None},
        { "primary", WeaponSlot.Primary},
        { "secondary", WeaponSlot.Secondary},
        { "pistol", WeaponSlot.Pistol},
        { "melee", WeaponSlot.Melee}
    };

    public override void _EnterTree()
    {
        ServiceLocator.RegisterService(this);

        WeaponSlots = new Godot.Collections.Dictionary<WeaponSlot, Weapon>()
        {
            { WeaponSlot.None, null}, // empty hands
            { WeaponSlot.Primary, null },
            { WeaponSlot.Secondary, null},
            { WeaponSlot.Pistol, null},
            { WeaponSlot.Melee, null}
        };
    }

    public override void _Ready()
    {
        _playerAnimationController = ServiceLocator.GetService<PlayerAnimationController>();
        _weaponAnimationController = ServiceLocator.GetService<WeaponAnimationController>();
    }

    public override void _Process(double delta)
    {
        HandleWeaponSwitchInput();
        HandleDropInput();
    }

    private void HandleWeaponSwitchInput()
    {
        foreach (var entry in _actionToSlotMap)
        {
            if (Input.IsActionJustPressed(entry.Key))
            {
                ProcessCommand(new SwitchActiveWeaponCommand(entry.Value));
                return;
            }
        }
    }

    private void HandleDropInput()
    {
        if (Input.IsActionJustPressed("drop"))
        {
            ProcessCommand(new DropCurrentWeaponCommand());
        }
    }

    #region WeaponManager Async Methods

    /// <summary>
    /// Command processor for weapon commands.
    /// </summary>
    /// <param name="command"></param>
    public async void ProcessCommand(IWeaponCommand command)
    {
        await command.Execute(this);
    }

    public Weapon GetActiveWeapon()
    {
        if (CurrentSlot != WeaponSlot.None && WeaponSlots.ContainsKey(CurrentSlot))
        {
            return WeaponSlots[CurrentSlot];
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Command task processor for weapon commands.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    private async Task ProcessCommandAsync(IWeaponCommand command)
    {
        await command.Execute(this);
    }

    internal async Task PickUpAndEquip(Weapon weaponOnGround)
    {
        GD.Print($"WeaponManager: Picking up {weaponOnGround.WeaponData.WeaponName}");
        if (weaponOnGround.WeaponData == null ||
            string.IsNullOrEmpty(weaponOnGround.WeaponData.WeaponPath))
        {
            GD.PrintErr("Weapon data is null or weapon path is empty");
            return;
        }

        var weaponScene = GD.Load<PackedScene>(weaponOnGround.WeaponData.WeaponPath);

        if (weaponScene == null)
        {
            GD.PrintErr($"Failed to load scene from path: " +
                $"{weaponOnGround.WeaponData.WeaponPath}");
            return;
        }

        Weapon newWeapon = weaponScene.Instantiate<Weapon>();
        if (newWeapon == null)
        {
            GD.PrintErr("Instantiated weapon is not of type Weapon or scene root is incorrect.");
            return;
        }

        // Set the weapon data
        newWeapon.SetHeld(true);
        newWeapon.Position = newWeapon.WeaponData.WeaponPosition;
        newWeapon.Rotation = newWeapon.WeaponData.WeaponRotation;
        newWeapon.Scale = newWeapon.WeaponData.WeaponScale;

        WeaponHolderSlot.AddChild(newWeapon);
        GD.Print("Instantiated weapon: " + newWeapon.WeaponData.WeaponName);

        // Equip weapon to available slot
        await EquipToAvailableSlot(newWeapon, newWeapon.WeaponData.WeaponType);

        // delete weapon from scene
        weaponOnGround.QueueFree();
    }

    /// <summary>
    /// Equips the weapon to the first available slot based on its type.
    /// </summary>
    /// <param name="weaponInstance">The weapon's instance</param>
    /// <param name="weaponType">The weapon's type enum</param>
    /// <returns></returns>
    private async Task EquipToAvailableSlot(Weapon weaponInstance, WeaponType weaponType)
    {
        WeaponSlot targetSlot = WeaponSlot.None;

        if (weaponType == WeaponType.AssaultRifle)
        {
            if (WeaponSlots[WeaponSlot.Primary] == null)
                targetSlot = WeaponSlot.Primary;
            else if (WeaponSlots[WeaponSlot.Secondary] == null)
                targetSlot = WeaponSlot.Secondary;
        }
        else if (weaponType == WeaponType.Pistol)
        {
            if (WeaponSlots[WeaponSlot.Pistol] == null)
                targetSlot = WeaponSlot.Pistol;
        }
        else if (weaponType == WeaponType.Melee)
        {
            if (WeaponSlots[WeaponSlot.Melee] == null)
                targetSlot = WeaponSlot.Melee;
        }

        // Check if the weapon type is valid and assign to the target slot
        if (targetSlot != WeaponSlot.None)
        {
            WeaponSlots[targetSlot] = weaponInstance;
            GD.Print($"Equipped {weaponInstance.WeaponData.WeaponName} to Slot: {targetSlot}");

            bool shouldSwitch = CurrentSlot == WeaponSlot.None;
            if (!shouldSwitch && WeaponSlots.ContainsKey(CurrentSlot))
            {
                shouldSwitch = WeaponSlots[CurrentSlot] == null;
            }
            if (shouldSwitch)
            {
                await ProcessCommandAsync(new SwitchActiveWeaponCommand(targetSlot));
            }
            else
            {
                weaponInstance.Visible = false;
            }
        }
        else
        {
            GD.Print($"No available slot for {weaponInstance.WeaponData.WeaponName} of type " +
                $"{weaponType}. Dropping(destroying picked up instance).");
            if (weaponInstance.GetParent() == WeaponHolderSlot)
            {
                WeaponHolderSlot.RemoveChild(weaponInstance);
            }
            weaponInstance.QueueFree();
        }
    }

    /// <summary>
    /// Switches the active weapon to the target slot.
    /// </summary>
    /// <param name="targetSlot">Target slot to switch</param>
    /// <returns></returns>
    internal async Task SwitchActiveWeapon(WeaponSlot targetSlot)
    {
        if (CurrentSlot == targetSlot)
            return;

        if (targetSlot != WeaponSlot.None && !WeaponSlots.ContainsKey(targetSlot))
        {
            GD.PrintErr($"Target slot: {targetSlot} does not exist in WeaponSlots dictionary.");
            return;
        }
        if (targetSlot != WeaponSlot.None && WeaponSlots[targetSlot] == null)
        {
            GD.PrintErr($"No weapon in slot: {targetSlot} to switch to.");
            return;
        }

        // Hide the current weapon
        if (CurrentSlot != WeaponSlot.None &&
            WeaponSlots.TryGetValue(CurrentSlot, out Weapon currentWeaponNode) &&
            currentWeaponNode != null)
        {
            currentWeaponNode.Visible = false;
        }

        CurrentSlot = targetSlot;

        // Equip the new weapon
        if (CurrentSlot != WeaponSlot.None &&
            WeaponSlots.TryGetValue(CurrentSlot, out Weapon newWeaponNode) &&
            newWeaponNode != null)
        {
            if (_playerAnimationController != null)
            {
                _playerAnimationController.SetArmedState(true, newWeaponNode.WeaponData.WeaponType);
                if (_playerAnimationController.LeftHandIK == null &&
                    _playerAnimationController.RightHandIK == null)
                {
                    GD.PrintErr("Critical Error: LeftHandIK or RightHandIK is NULL.");
                }
                else
                {
                    if (newWeaponNode.LeftHandTarget == null &&
                        newWeaponNode.RightHandTarget == null)
                    {
                        GD.PrintErr($"LeftHandTarget or RightHandTarget is NULL for weapon {newWeaponNode.Name}" +
                            $" in slot: {CurrentSlot}");
                    }
                    else
                    {
                        if (CurrentSlot == WeaponSlot.Melee)
                        {
                            if(_playerAnimationController.LeftHandIK != null)
                            {
                                _playerAnimationController.LeftHandIK.TargetNode = null;
                                _playerAnimationController.LeftHandIK.Stop();
                            }
                            else
                            {
                                GD.PrintErr("LeftHandIK is NULL, when switching to melee weapon.");
                            }
                            if(_playerAnimationController.RightHandIK != null)
                            {
                                _playerAnimationController.RightHandIK.TargetNode = null;
                                _playerAnimationController.RightHandIK.Stop();
                            }
                            else
                            {
                                GD.PrintErr("RightHandIK is NULL, when switching to melee weapon.");
                            }
                        }
                        else
                        {
                            if(_playerAnimationController.LeftHandIK != null)
                            {
                                if(newWeaponNode.LeftHandTarget != null) 
                                {
                                    _playerAnimationController.LeftHandIK.TargetNode = newWeaponNode.LeftHandTarget.GetPath();
                                    _playerAnimationController.LeftHandIK.Start();
                                }
                                else 
                                {
                                    GD.PrintErr($"LeftHandTarget is NULL, when switching to {newWeaponNode.Name}.");
                                    _playerAnimationController.LeftHandIK.TargetNode = null;
                                    _playerAnimationController.LeftHandIK.Stop();
                                }
                            }
                            else
                            {
                                GD.PrintErr($"LeftHandIK is NULL, for {newWeaponNode.Name}.");
                            }

                            if (_playerAnimationController.RightHandIK != null) 
                            {
                                if(newWeaponNode.RightHandTarget != null) 
                                {
                                    _playerAnimationController.RightHandIK.TargetNode = newWeaponNode.RightHandTarget.GetPath();
                                    _playerAnimationController.RightHandIK.Start();
                                }
                                else 
                                {
                                    GD.PrintErr($"RightHandTarget is NULL, when switching to {newWeaponNode.Name}.");
                                    _playerAnimationController.RightHandIK.TargetNode = null;
                                    _playerAnimationController.RightHandIK.Stop();
                                }
                            }
                            else
                            {
                                GD.PrintErr($"RightHandIK is NULL, for {newWeaponNode.Name}.");
                            }
                        }
                    }
                }
            }

            newWeaponNode.Visible = true;
            OnWeaponSwitched?.Invoke(CurrentSlot, newWeaponNode);
            GD.Print($"Switched to weapon in slot: {CurrentSlot}");
        }
        // Switch to empty hands
        else if (CurrentSlot == WeaponSlot.None)
        {
            if (_playerAnimationController != null)
            {
                _playerAnimationController.SetArmedState(false, WeaponType.None);
                if (_playerAnimationController.LeftHandIK != null)
                {
                    _playerAnimationController.LeftHandIK.TargetNode = null;
                    _playerAnimationController.LeftHandIK.Stop();
                }
                else
                {
                    GD.PrintErr("LeftHandIK is NULL, when switching to empty hands.");
                }

                if (_playerAnimationController.RightHandIK != null)
                {
                    _playerAnimationController.RightHandIK.TargetNode = null;
                    _playerAnimationController.RightHandIK.Stop();
                }
                else
                {
                    GD.PrintErr("RightHandIK is NULL, when switching to empty hands.");
                }
            }
            OnWeaponSwitched?.Invoke(WeaponSlot.None, null);
            GD.Print("Switched to empty hands.");
        }
    }

    /// <summary>
    /// Drops the current weapon from the player's hands.
    /// </summary>
    /// <returns></returns>
    internal async Task DropCurrentWeapon()
    {
        if (CurrentSlot == WeaponSlot.None ||
            !WeaponSlots.TryGetValue(CurrentSlot, out Weapon weaponToDrop) ||
            weaponToDrop == null)
        {
            GD.PrintErr("No current weapon to drop.");
            return;
        }

        var weaponScene = GD.Load<PackedScene>(weaponToDrop.WeaponData.WeaponPath);
        if (weaponScene == null)
        {
            GD.PrintErr($"Failed to load weapon scene for dropping: " +
                $"{weaponToDrop.WeaponData.WeaponPath}");
            return;
        }

        Weapon droppedInstance = weaponScene.Instantiate<Weapon>();
        if (droppedInstance == null)
        {
            GD.PrintErr("Instantiated dropped weapon is not of type weapon");
            return;
        }

        GetTree().Root.AddChild(droppedInstance);

        droppedInstance.GlobalPosition = weaponToDrop.GlobalPosition;
        droppedInstance.GlobalRotation = weaponToDrop.GlobalRotation;

        droppedInstance.SetHeld(false);

        //droppedInstance.ApplyImpulse(Vector3.Forward.Rotated(Vector3.Up, 
        //    droppedInstance.GlobalRotation.Y) * 5, Vector3.Zero);


        GD.Print($"Dropped weapon: {weaponToDrop.WeaponData.WeaponName}");

        WeaponHolderSlot.RemoveChild(weaponToDrop);
        weaponToDrop.QueueFree();
        WeaponSlots[CurrentSlot] = null;

        // Switch to empty hands
        await ProcessCommandAsync(new SwitchActiveWeaponCommand(WeaponSlot.None));
    }

    #endregion
}
