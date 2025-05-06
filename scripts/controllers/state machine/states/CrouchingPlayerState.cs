using Godot;
using System;

public partial class CrouchingPlayerState : PlayerMovementState
{
    const float SPEED = 3.0f;
    const float ACCELERATION = 0.5f;
    const float DECELERATION = 0.25f;

    const float DEFAULT_HEIGHT = 2.15f;
    const float CROUCH_HEIGHT = 1.6f;

    const string TRANSITION = "Transition";
    const string CROUCH_BLEND_POS = "parameters/PlayerStateMachine/Crouched/CrouchingBlendSpace2D/blend_position";
    const string WALK_BLEND_POS = "parameters/PlayerStateMachine/Standing/WalkBlendSpace2D/blend_position";

    public override void Update(double delta)
    {

        PlayerController.AnimationTree.Set("is_crouched", true);
        SetCameraCollision(true);

        PlayerController.AnimationTree.Set(CROUCH_BLEND_POS,
            new Vector2(PlayerController.GetInputDirection().X,
            -PlayerController.GetInputDirection().Y));

        if (Input.IsActionJustReleased("crouch"))
        {
            Uncrouch();
        }

        if (!PlayerController.IsOnFloor() && PlayerController.Velocity.Y < 0.0f)
        {
            EmitSignal(TRANSITION, "FallingPlayerState");
        }

    }

    public override void PhysicsUpdate(double delta)
    {
        PlayerController.UpdateGravity((float)delta);
        PlayerController.UpdateInput(SPEED, ACCELERATION, DECELERATION);
        PlayerController.UpdateVelocity();
    }

    private async void Uncrouch()
    {
        if (!PlayerController.CrouchShapeCast.IsColliding() &&
            Input.IsActionPressed("crouch") == false)
        {
            PlayerController.AnimationTree.Set("is_crouched", false);
            PlayerController.AnimationTree.Set(WALK_BLEND_POS, Vector2.Zero);
            SetCameraCollision(false);
            EmitSignal(TRANSITION, "IdlePlayerState");
        }
        else if (PlayerController.CrouchShapeCast.IsColliding())
        {
            await ToSignal(GetTree().CreateTimer(0.1), SceneTreeTimer.SignalName.Timeout);
            Uncrouch();
        }
    }

    private void SetCameraCollision(bool is_crouching)
    {
        if (!is_crouching)
        {
            PlayerController.CollisionShape3D.Shape.Set("height", DEFAULT_HEIGHT);
            PlayerController.CollisionShape3D.Position = new Vector3(0.0f,
                DEFAULT_HEIGHT / 2.0f, 0.0f);
        }
        else
        {
            PlayerController.CollisionShape3D.Shape.Set("height", CROUCH_HEIGHT);
            PlayerController.CollisionShape3D.Position = new Vector3(0.0f,
                CROUCH_HEIGHT / 2.0f, 0.0f);
        }
    }
}
