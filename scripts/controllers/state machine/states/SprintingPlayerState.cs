using Godot;

public partial class SprintingPlayerState : PlayerMovementState
{
    const float SPEED = 9.0f;
    const float ACCELERATION = 0.5f;
    const float DECELERATION = 0.25f;

    public override void Update(double delta)
    {

        // Animations setup
        AnimationController.AnimationTree.Set("is_sprinting", true);
        AnimationController.AnimationTree.Set(AnimationController.SprintBlendPos, 2.0f);

        if (PlayerController.Velocity.Length() == 0.0f && PlayerController.IsOnFloor())
        {
            AnimationController.AnimationTree.Set("is_sprinting", false);
            EmitSignal(AnimationController.Transition, "IdlePlayerState");
        }
        if (Input.IsActionJustReleased("sprint") && PlayerController.IsOnFloor())
        {
            AnimationController.AnimationTree.Set("is_sprinting", false);
            EmitSignal(AnimationController.Transition, "WalkingPlayerState");
        }
        if (Input.IsActionJustPressed("jump") && PlayerController.IsOnFloor())
        {
            AnimationController.AnimationTree.Set("is_sprinting", false);
            EmitSignal(AnimationController.Transition, "JumpingPlayerState");
        }
        if (PlayerController.Velocity.Y < 0.0f && !PlayerController.IsOnFloor())
        {
            AnimationController.AnimationTree.Set("is_sprinting", false);
            EmitSignal(AnimationController.Transition, "FallingPlayerState");
        }
    }

    public override void PhysicsUpdate(double delta)
    {
        PlayerController.UpdateInput(SPEED, ACCELERATION, DECELERATION);
        PlayerController.UpdateGravity((float)delta);
        PlayerController.UpdateVelocity();
    }
}
