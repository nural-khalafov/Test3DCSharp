using Godot;

public partial class WalkingPlayerState : PlayerMovementState
{
    const float SPEED = 5.0f;
    const float ACCELERATION = 0.1f;
    const float DECELERATION = 0.25f;

    const string TRANSITION = "Transition";
    const string WALK_BLEND_POS = "parameters/PlayerStateMachine/Standing/WalkBlendSpace2D/blend_position";

    public override void Update(double delta)
    {
        PlayerController.UpdateInput(SPEED, DECELERATION, ACCELERATION);

        PlayerController.AnimationTree.Set(WALK_BLEND_POS, 
            new Vector2(PlayerController.GetInputDirection().X,
            -PlayerController.GetInputDirection().Y));

        if (PlayerController.Velocity.Length() == 0.0 && PlayerController.IsOnFloor())
        {
            EmitSignal(TRANSITION, "IdlePlayerState");
        }

        if (Input.IsActionPressed("sprint") && PlayerController.IsOnFloor())
        {
            EmitSignal(TRANSITION, "SprintingPlayerState");
        }

        if (Input.IsActionPressed("crouch") && PlayerController.IsOnFloor())
        {
            EmitSignal(TRANSITION, "CrouchingPlayerState");
        }

        if (Input.IsActionJustPressed("jump") && PlayerController.IsOnFloor())
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
