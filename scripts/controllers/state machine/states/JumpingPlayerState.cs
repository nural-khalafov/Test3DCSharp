using Godot;

public partial class JumpingPlayerState : PlayerMovementState
{
    const float SPEED = 7.0f;
    const float ACCELERATION = 0.015f;
    const float DECELERATION = 0.1f;
    const float JUMP_VELOCITY = 5.5f;
    const float INPUT_MULTIPLIER = 0.85f;

    const string TRANSITION = "Transition";
    const string JUMP_BLEND_POS = "parameters/PlayerStateMachine/Standing/JumpBlendSpace1D/blend_position";

    public override void Enter(State previousState)
    {
        PlayerController.Velocity = new Vector3(PlayerController.Velocity.X,
            PlayerController.Velocity.Y + JUMP_VELOCITY, PlayerController.Velocity.Z);
        PlayerController.AnimationTree.Set("is_in_air", true);
        PlayerController.AnimationTree.Set(JUMP_BLEND_POS, 1.0f);
    }

    public override void Update(double delta)
    {
        PlayerController.UpdateGravity((float)delta);
        PlayerController.UpdateInput(SPEED * INPUT_MULTIPLIER, ACCELERATION, DECELERATION);
        PlayerController.UpdateVelocity();

        if (Input.IsActionJustPressed("jump"))
        {
            if (PlayerController.Velocity.Y > 0)
            {
                PlayerController.Velocity = new Vector3(PlayerController.Velocity.X,
                    PlayerController.Velocity.Y / 2.0f, PlayerController.Velocity.Z);
            }
        }

        if (PlayerController.IsOnFloor())
        {
            PlayerController.AnimationTree.Set("is_in_air", false);
            EmitSignal(TRANSITION, "IdlePlayerState");
        }
    }
}
