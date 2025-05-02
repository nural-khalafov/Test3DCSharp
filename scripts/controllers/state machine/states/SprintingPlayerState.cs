using Godot;
using System;

public partial class SprintingPlayerState : PlayerMovementState
{
    const float SPEED = 9.0f;
    const float ACCELERATION = 0.1f;
    const float DECELERATION = 0.25f;

    const string TRANSITION = "Transition";
    const string SPRINT_BLEND_POS = "parameters/PlayerStateMachine/Standing/SprintBlendSpace1D/blend_position";

    public override void Update(double delta)
    {
        PlayerController.UpdateInput(SPEED, ACCELERATION, DECELERATION);
        PlayerController.animationTree.Set("is_sprinting", true);

        if (PlayerController.Velocity.Length() == 0.0f && PlayerController.IsOnFloor())
        {
            PlayerController.animationTree.Set("is_sprinting", false);
            EmitSignal(TRANSITION, "IdlePlayerState");
        }

        if (Input.IsActionJustReleased("sprint") && PlayerController.IsOnFloor()) 
        {
            PlayerController.animationTree.Set("is_sprinting", false);
            EmitSignal(TRANSITION, "WalkingPlayerState");
        }

    }

    public override void PhysicsUpdate(double delta)
    {
        PlayerController.UpdateGravity((float)delta);
        PlayerController.UpdateVelocity();
    }
}
