using Godot;

public partial class PlayerMovementState : State
{
    public FirstPersonController PlayerController;
    public PlayerAnimationController AnimationController;

    public override async void _Ready()
    {
        await ToSignal(Owner, "ready");
        PlayerController = Owner as FirstPersonController;
        AnimationController = Owner.GetNode<PlayerAnimationController>("CharacterModel");
    }

    public override void _Process(double delta)
    {
        // NULL
    }
}
