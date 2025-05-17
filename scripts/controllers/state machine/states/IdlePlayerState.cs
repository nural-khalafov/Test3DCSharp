using Godot;

public partial class IdlePlayerState : PlayerMovementState
{
    const float SPEED = 5.0f;
    const float ACCELERATION = 0.5f;
    const float DECELERATION = 0.25f;

    private float _timeSinceJump;

    public override void Enter(State previousState)
    {
        _timeSinceJump = 0f;
    }

    public override void Update(double delta)
    {
        AnimationController.AnimationTree.Set(AnimationController.WalkBlendPos, Vector2.Zero);

        if (PlayerController.Velocity.Length() > 0.0f && PlayerController.IsOnFloor())
        {
            EmitSignal(AnimationController.Transition, "WalkingPlayerState");
        }
        if (Input.IsActionPressed("crouch") && PlayerController.IsOnFloor())
        {
            EmitSignal(AnimationController.Transition, "CrouchingPlayerState");
        }
        if (Input.IsActionJustPressed("jump") && PlayerController.IsOnFloor())
        {
            EmitSignal(AnimationController.Transition, "JumpingPlayerState");
        }
        if (PlayerController.Velocity.Y < 0.0f && !PlayerController.IsOnFloor())
        {
            EmitSignal(AnimationController.Transition, "FallingPlayerState");
        }
    }

    public override void PhysicsUpdate(double delta)
    {
        PlayerController.UpdateGravity((float)delta);
        PlayerController.UpdateInput(SPEED, ACCELERATION, DECELERATION);
        PlayerController.UpdateVelocity();
        AnimationController.UpdateLeaning(true, (float)delta, -0.5f, 0.5f);
    }
}
