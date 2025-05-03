using Godot;

public partial class IdlePlayerState : PlayerMovementState
{
    const float SPEED = 5.0f;
    const float ACCELERATION = 0.1f;
    const float DECELERATION = 0.25f;

    const string TRANSITION = "Transition";
    const string WALK_BLEND_POS = "parameters/PlayerStateMachine/Standing/WalkBlendSpace2D/blend_position";

    public override void Update(double delta)
    {
        PlayerController.UpdateInput(SPEED, ACCELERATION, DECELERATION);

        PlayerController.AnimationTree.Set(WALK_BLEND_POS, Vector2.Zero);

        if (PlayerController.Velocity.Length() > 0.0f && PlayerController.IsOnFloor())
        {
            EmitSignal(TRANSITION, "WalkingPlayerState");
        }
        if (Input.IsActionPressed("crouch") && PlayerController.IsOnFloor())
        {
            EmitSignal(TRANSITION, "CrouchingPlayerState");
        }
        if (Input.IsActionPressed("jump") && PlayerController.IsOnFloor())
        {
            EmitSignal(TRANSITION, "JumpingPlayerState");
        }
        if (PlayerController.Velocity.Y > -3.0 && !PlayerController.IsOnFloor())
        {
            EmitSignal(TRANSITION, "FallingPlayerState");
        }
    }

    public override void PhysicsUpdate(double delta)
    {
        PlayerController.UpdateGravity((float)delta);
        PlayerController.UpdateVelocity();
    }
}
