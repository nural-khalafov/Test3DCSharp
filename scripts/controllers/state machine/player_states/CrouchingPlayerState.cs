using Godot;
using System;

public partial class CrouchingPlayerState : PlayerMovementState
{
    const float SPEED = 2f;
    const float ACCELERATION = 0.5f;
    const float DECELERATION = 0.25f;

    const float DEFAULT_HEIGHT = 1.75f;
    const float CROUCH_HEIGHT = 1.2f;

    public override void Update(double delta)
    {

        AnimationController.AnimationTree.Set("is_crouched", true);
        SetCameraCollision(true);

        AnimationController.AnimationTree.Set(AnimationController.CrouchBlendPos,
            new Vector2(PlayerController.GetInputDirection().X,
            -PlayerController.GetInputDirection().Y));

        if (!AnimationController.IsArmed)
        {
            AnimationController.AnimationTree.Set(AnimationController.UpperbodyUnarmedBlendPos,
                new Vector2(PlayerController.GetInputDirection().X,
            -PlayerController.GetInputDirection().Y) / 2.5f);
        }

        if (Input.IsActionJustReleased("crouch"))
        {
            Uncrouch();
        }

        if (!PlayerController.IsOnFloor() && PlayerController.Velocity.Y < 0.0f)
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

    private async void Uncrouch()
    {
        if (!PlayerController.CrouchShapeCast.IsColliding() &&
            Input.IsActionPressed("crouch") == false)
        {
            AnimationController.AnimationTree.Set("is_crouched", false);
            AnimationController.AnimationTree.Set(AnimationController.WalkBlendPos, Vector2.Zero);
            SetCameraCollision(false);
            EmitSignal(AnimationController.Transition, "IdlePlayerState");
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
