using System.Threading.Tasks;

public interface IWeaponCommand
{
    Task Execute(WeaponManager weaponManager);
}
