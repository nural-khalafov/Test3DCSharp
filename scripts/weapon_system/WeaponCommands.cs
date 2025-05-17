using Godot;
using System.Threading.Tasks;

public class PickUpAndEquipCommand : IWeaponCommand
{
    public Weapon WeaponOnGround;

    public PickUpAndEquipCommand(Weapon weaponOnGround)
    {
        WeaponOnGround = weaponOnGround;
    }

    public async Task Execute(WeaponManager weaponManager)
    {
        if(WeaponOnGround == null || WeaponOnGround.WeaponData == null)
        {
            GD.PrintErr("PickUpAndEquipCommand: Weapon on ground or its data is null");
            return;
        }
        await weaponManager.PickUpAndEquip(WeaponOnGround);
    }
}

public class SwitchActiveWeaponCommand : IWeaponCommand
{
    public WeaponSlot TargetSlot { get; }

    public SwitchActiveWeaponCommand(WeaponSlot targetSlot)
    {
        TargetSlot = targetSlot;
    }
    public async Task Execute(WeaponManager weaponManager)
    {
        await weaponManager.SwitchActiveWeapon(TargetSlot);
    }
}

public class DropCurrentWeaponCommand : IWeaponCommand
{
    public async Task Execute(WeaponManager weaponManager)
    {
        await weaponManager.DropCurrentWeapon();
    }
}