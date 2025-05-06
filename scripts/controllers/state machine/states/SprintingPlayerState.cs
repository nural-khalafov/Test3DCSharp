using Godot;

public partial class SprintingPlayerState : PlayerMovementState
{
    const float SPEED = 9.0f;
    const float ACCELERATION = 0.5f;
    const float DECELERATION = 0.25f;

    const string TRANSITION = "Transition";
    const string SPRINT_BLEND_POS = "parameters/PlayerStateMachine/Standing/SprintBlendSpace1D/blend_position";

    public override void Update(double delta)
    {

        // Animations setup
        PlayerController.AnimationTree.Set("is_sprinting", true);
        PlayerController.AnimationTree.Set(SPRINT_BLEND_POS, SPEED);

        if (PlayerController.Velocity.Length() == 0.0f && PlayerController.IsOnFloor())
        {
            PlayerController.AnimationTree.Set("is_sprinting", false);
            EmitSignal(TRANSITION, "IdlePlayerState");
        }
        if (Input.IsActionJustReleased("sprint") && PlayerController.IsOnFloor())
        {
            PlayerController.AnimationTree.Set("is_sprinting", false);
            EmitSignal(TRANSITION, "WalkingPlayerState");
        }
        if (Input.IsActionJustPressed("jump") && PlayerController.IsOnFloor())
        {
            PlayerController.AnimationTree.Set("is_sprinting", false);
            EmitSignal(TRANSITION, "JumpingPlayerState");
        }
        if (PlayerController.Velocity.Y < 0.0f && !PlayerController.IsOnFloor())
        {
            PlayerController.AnimationTree.Set("is_sprinting", false);
            EmitSignal(TRANSITION, "FallingPlayerState");
        }
    }

    public override void PhysicsUpdate(double delta)
    {
        PlayerController.UpdateInput(SPEED, ACCELERATION, DECELERATION);
        PlayerController.UpdateGravity((float)delta);
        PlayerController.UpdateVelocity();
    }
}
