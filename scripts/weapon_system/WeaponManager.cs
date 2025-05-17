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
        GlobalSingleton.WeaponManager = this;

        WeaponSlots = new Godot.Collections.Dictionary<WeaponSlot, Weapon>()
        {
            { WeaponSlot.None, null},
            { WeaponSlot.Primary, null },
            { WeaponSlot.Secondary, null},
            { WeaponSlot.Pistol, null},
            { WeaponSlot.Melee, null}
        };
    }

    public override void _Process(double delta)
    {
        HandleWeaponSwitchInput();
        HandleDropInput();
    }

    public async void ProcessCommand(IWeaponCommand command)
    {
        await command.Execute(this);
    }

    private async Task ProcessCommandAsync(IWeaponCommand command)
    {
        await command.Execute(this);
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

    internal async Task PickUpAndEquip(Weapon weaponOnGround)
    {
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

        if (CurrentSlot != WeaponSlot.None &&
            WeaponSlots.TryGetValue(CurrentSlot, out Weapon currentWeaponNode) &&
            currentWeaponNode != null)
        {
            currentWeaponNode.Visible = false;
        }

        CurrentSlot = targetSlot;

        if (CurrentSlot != WeaponSlot.None &&
            WeaponSlots.TryGetValue(CurrentSlot, out Weapon newWeaponNode) &&
            newWeaponNode != null)
        {
            if(GlobalSingleton.PlayerAnimationController != null)
            {
                GlobalSingleton.PlayerAnimationController.SetArmedState(true, newWeaponNode.WeaponData.WeaponType);
            }

            newWeaponNode.Visible = true;
            OnWeaponSwitched?.Invoke(CurrentSlot, newWeaponNode);
            GD.Print($"Switched to weapon in slot: {CurrentSlot}");
        }
        else if (CurrentSlot == WeaponSlot.None)
        {
            if(GlobalSingleton.PlayerAnimationController != null)
            {
                GlobalSingleton.PlayerAnimationController.SetArmedState(false, WeaponType.None);
            }
            OnWeaponSwitched?.Invoke(WeaponSlot.None, null);
            GD.Print("Switched to empty hands.");
        }
    }
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
}
