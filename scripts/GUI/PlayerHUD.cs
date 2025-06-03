using Godot;
using System;

public partial class PlayerHUD : Control
{
    [ExportCategory("HUD Ammo Panel Components")]
    [Export] private PanelContainer _ammoCounterPanel;
    [Export] private Label _currentAmmoLabel;
    [Export] private Label _reserveAmmoLabel;

    private WeaponManager _weaponManager;

    public override void _EnterTree()
    {
        if(_ammoCounterPanel == null)
            GD.PrintErr("PlayerHUD: AmmoCounterPanel not found.");
        if(_currentAmmoLabel == null)
            GD.PrintErr("PlayerHUD: CurrentAmmoLabel not found.");
        if(_reserveAmmoLabel == null)
            GD.PrintErr("PlayerHUD: ReserveAmmoLabel not found.");
    }

    public override void _Ready()
    {
        _weaponManager = ServiceLocator.GetService<WeaponManager>();
        if (_weaponManager == null)
        {
            GD.PrintErr("PlayerHUD: WeaponManager not found.");
            return;
        }
    }

    public override void _Process(double delta)
    {
        if(_weaponManager == null || _ammoCounterPanel == null || _currentAmmoLabel == null || _reserveAmmoLabel == null)
            return;

        if (_weaponManager.CurrentSlot != WeaponSlot.None && _weaponManager.CurrentSlot != WeaponSlot.Melee)
        {
            var currentWeapon = _weaponManager.WeaponSlots[_weaponManager.CurrentSlot];
            if (currentWeapon != null)
            {
                _ammoCounterPanel.Visible = true;
                _currentAmmoLabel.Text = currentWeapon.CurrentAmmo.ToString() + " / " + currentWeapon.MagazineSize.ToString();
                _reserveAmmoLabel.Text = currentWeapon.ReserveAmmo.ToString();
            }
            else
            {
                _ammoCounterPanel.Visible = false;
            }
        }
        else
        {
            _ammoCounterPanel.Visible = false;
        }
    }
}
