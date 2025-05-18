using Godot;
using Test3DCSharp.Utilities;

[GlobalClass]
public partial class DebugSingleton : Node
{
    public static Debug Debug;
    public static FirstPersonController Player;
    public static StateMachine PlayerStateMachine;

    public override void _Process(double delta)
    {
        Debug.AddProperty("FPS", (1.0 / delta).ToString("F2"), 0);
        Debug.AddProperty("Current Velocity", Player.Velocity.Length().ToString("F2"), 1);
        Debug.AddProperty("Current State", PlayerStateMachine.CurrentState.Name.ToString(), 2);
        Debug.AddProperty("Current Weapon Slot", GlobalSingleton.WeaponManager.CurrentSlot.ToString(), 3);
        if(GlobalSingleton.WeaponManager.CurrentSlot != WeaponSlot.None)
            Debug.AddProperty("Current Weapon",
            GlobalSingleton.WeaponManager.WeaponSlots[GlobalSingleton.WeaponManager.CurrentSlot]
            .WeaponData.WeaponName.ToString(), 4);
    }
}
