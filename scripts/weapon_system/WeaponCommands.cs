using Godot;

public class PickUpAndEquipCommand : IWeaponCommand
{
    private readonly Weapon _weaponOnGround;

    public PickUpAndEquipCommand(Weapon weaponOnGround)
    {
        _weaponOnGround = weaponOnGround;
    }

    public void Execute(WeaponManager weaponManager)
    {
        if(_weaponOnGround == null || _weaponOnGround.WeaponData == null)
        {
            GD.PrintErr("PickUpAndEquipCommand: Weapon on ground or its data is null");
            return;
        }
        weaponManager.PickUpAndEquip(_weaponOnGround);
    }
}

public class SwitchActiveWeaponCommand : IWeaponCommand
{
    private readonly WeaponSlot _weaponSlot;

    public SwitchActiveWeaponCommand(WeaponSlot targetSlot)
    {
        _weaponSlot = targetSlot;
    }
    public void Execute(WeaponManager weaponManager)
    {
        weaponManager.SwitchActiveWeapon(_weaponSlot);
    }
}

public class DropCurrentWeaponCommand : IWeaponCommand
{
    public void Execute(WeaponManager weaponManager)
    {
        weaponManager.DropCurrentWeapon();
    }
}