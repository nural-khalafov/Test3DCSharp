using Godot;

public partial class FallingPlayerState : PlayerMovementState
{
    const float SPEED = 5.0f;
    const float ACCELERATION = 0.1f;
    const float DECELERATION = 0.25f;

    const string TRANSITION = "Transition";
    const string JUMP_BLEND_POS = "parameters/PlayerStateMachine/Standing/JumpBlendSpace1D/blend_position";

    public override void Update(double delta)
    {
        PlayerController.UpdateGravity((float)delta);
        PlayerController.UpdateInput(SPEED, ACCELERATION, DECELERATION);
        PlayerController.UpdateVelocity();

        if (!PlayerController.IsOnFloor())
        {
            PlayerController.AnimationTree.Set("is_in_air", true);
            PlayerController.AnimationTree.Set(JUMP_BLEND_POS, -1.0f);
        }

        if (PlayerController.IsOnFloor())
        {
            PlayerController.AnimationTree.Set("is_in_air", false);
            EmitSignal(TRANSITION, "IdlePlayerState");
        }
    }
}
