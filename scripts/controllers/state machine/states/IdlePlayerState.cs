using Godot;
using System;

public partial class IdlePlayerState : PlayerMovementState
{
    const float SPEED = 5.0f;
    const float ACCELERATION = 0.1f;
    const float DECELERATION = 0.25f;

    const string WALK_BLEND_POS = "parameters/PlayerStateMachine/Standing/WalkBlendSpace2D/blend_position";

    public override void Update(double delta)
    {
        PlayerController.UpdateInput(SPEED, ACCELERATION, DECELERATION);

        if (PlayerController.Velocity.Length() > 0.0f && PlayerController.IsOnFloor())
        {
            EmitSignal("Transition", "WalkingPlayerState");
        }
    }

    public override void PhysicsUpdate(double delta)
    {
        PlayerController.UpdateGravity((float)delta);
        PlayerController.UpdateVelocity();
    }
}
