using Godot;

public partial class WalkingPlayerState : PlayerMovementState
{
    const float SPEED = 5.0f;
    const float ACCELERATION = 0.5f;
    const float DECELERATION = 0.25f;

    public override void Update(double delta)
    {
        AnimationController.AnimationTree.Set(AnimationController.WalkBlendPos, 
            new Vector2(PlayerController.GetInputDirection().X,
            -PlayerController.GetInputDirection().Y));

        if (!AnimationController.IsArmed)
        {
            AnimationController.AnimationTree.Set(AnimationController.UpperbodyUnarmedBlendPos,
                new Vector2(PlayerController.GetInputDirection().X,
            -PlayerController.GetInputDirection().Y) / 2f);
        }

        if (PlayerController.Velocity.Length() == 0.0 && PlayerController.IsOnFloor())
        {
            EmitSignal(AnimationController.Transition, "IdlePlayerState");
        }

        if (Input.IsActionPressed("sprint") &&
            Input.IsActionPressed("up") &&
            PlayerController.IsOnFloor())
        {
            EmitSignal(AnimationController.Transition, "SprintingPlayerState");
        }

        if (Input.IsActionPressed("crouch") && PlayerController.IsOnFloor())
        {
            EmitSignal(AnimationController.Transition, "CrouchingPlayerState");
        }

        if (Input.IsActionJustPressed("jump") && PlayerController.IsOnFloor())
        {
            EmitSignal(AnimationController.Transition, "JumpingPlayerState");
        }

        if (PlayerController.Velocity.Y < -3.0f && !PlayerController.IsOnFloor())
        {
            EmitSignal(AnimationController.Transition, "FallingPlayerState");
        }
    }

    public override void PhysicsUpdate(double delta)
    {
        PlayerController.UpdateGravity((float)delta);
        PlayerController.UpdateInput(SPEED, DECELERATION, ACCELERATION);
        PlayerController.UpdateVelocity();
        AnimationController.UpdateLeaning(true, (float)delta, -0.5f, 0.5f);

    }
}
