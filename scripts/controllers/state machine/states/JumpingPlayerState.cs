using Godot;

public partial class JumpingPlayerState : PlayerMovementState
{
    const float SPEED = 7.0f;
    const float ACCELERATION = 0.5f;
    const float DECELERATION = 0.15f;
    const float JUMP_VELOCITY = 5.5f;
    const float INPUT_MULTIPLIER = 0.85f;

    private float _timeSinceJump = 0.0f;
    private const float GRACE_TIME = 0.1f;
    private bool _justJumped = false;

    public override void Enter(State previousState)
    {
        PlayerController.Velocity = new Vector3(PlayerController.Velocity.X,
            PlayerController.Velocity.Y + JUMP_VELOCITY,
            PlayerController.Velocity.Z);
        AnimationController.AnimationTree.Set("is_in_air", true);
        AnimationController.AnimationTree.Set(AnimationController.JumpBlendPos, 1.0f);

        _timeSinceJump = 0.0f;
    }

    public override void Update(double delta)
    {
        if (Input.IsActionJustReleased("jump"))
        {
            if (PlayerController.Velocity.Y > 0)
            {
                var velocity = PlayerController.Velocity;
                velocity.Y /= 2.0f;
                PlayerController.Velocity = velocity;
            }
        }

        if (PlayerController.IsOnFloor())
        {
            AnimationController.AnimationTree.Set("is_in_air", false);
            EmitSignal(AnimationController.Transition, "IdlePlayerState");
        }
    }

    public override void PhysicsUpdate(double delta)
    {
        PlayerController.UpdateGravity((float)delta);
        PlayerController.UpdateInput(SPEED * INPUT_MULTIPLIER, ACCELERATION, DECELERATION);
        PlayerController.UpdateVelocity();
    }
}
