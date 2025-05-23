using Godot;
using Test3DCSharp.Utilities;

[GlobalClass]
public partial class DebugDisplay : Node
{
    private Debug _debugPanel;
    private StateMachine _playerStateMachine;
    private UpperbodyStateMachine _playerUpperBodyStateMachine;
    private WeaponManager _weaponManager;
    private FirstPersonController _playerController;

    public override void _Ready()
    {
        _debugPanel = ServiceLocator.GetService<Debug>();
        _playerStateMachine = ServiceLocator.GetService<StateMachine>();
        _playerUpperBodyStateMachine = ServiceLocator.GetService<UpperbodyStateMachine>();
        _weaponManager = ServiceLocator.GetService<WeaponManager>();
        _playerController = ServiceLocator.GetService<FirstPersonController>();

        if (_debugPanel == null)
            GD.PrintErr("DebugSingleton: Debug panel not found.");
        if(_playerStateMachine == null)
            GD.PrintErr("DebugSingleton: Player state machine not found.");
        if(_weaponManager == null)
            GD.PrintErr("DebugSingleton: Weapon manager not found.");
        if(_playerController == null)
            GD.PrintErr("DebugSingleton: Player controller not found.");
    }

    public override void _Process(double delta)
    {
        if (_debugPanel == null || _playerStateMachine == null || _weaponManager == null || _playerController == null)
            return;

        if (!_debugPanel.Visible)
            return;

        _debugPanel.AddProperty("FPS", (1.0 / delta).ToString("F2"), 0);
        _debugPanel.AddProperty("Camera Mode", _playerController.CameraMode.ToString(),1);
        _debugPanel.AddProperty("Current Velocity", _playerController.Velocity.Length().ToString("F2"), 2);
        if (_playerStateMachine != null)
            _debugPanel.AddProperty("Current State", _playerStateMachine.CurrentState.Name.ToString(), 3);
        _debugPanel.AddProperty("Current Upperbody State", _playerUpperBodyStateMachine.CurrentState.Name.ToString(), 4);
        _debugPanel.AddProperty("Current Weapon Slot", _weaponManager.CurrentSlot.ToString(), 5);
        if(_weaponManager.CurrentSlot != WeaponSlot.None)
            _debugPanel.AddProperty("Current Weapon",
            _weaponManager.WeaponSlots[_weaponManager.CurrentSlot]
            .WeaponData.WeaponName.ToString(), 6);
        else
            _debugPanel.AddProperty("Current Weapon", "None", 6);
    }
}
