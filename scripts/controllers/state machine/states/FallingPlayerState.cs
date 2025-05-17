using Godot;

public partial class FallingPlayerState : PlayerMovementState
{
    const float SPEED = 5.0f;
    const float ACCELERATION = 0.5f;
    const float DECELERATION = 0.25f;

    public override void Update(double delta)
    {
        if (!PlayerController.IsOnFloor())
        {
            AnimationController.AnimationTree.Set("is_in_air", true);
            AnimationController.AnimationTree.Set(AnimationController.JumpBlendPos, -1.0f);
        }

        if (PlayerController.IsOnFloor())
        {
            AnimationController.AnimationTree.Set("is_in_air", false);
            EmitSignal(AnimationController.Transition, "IdlePlayerState");
        }
    }

    public override void PhysicsUpdate(double delta)
    {
        PlayerController.UpdateInput(SPEED, ACCELERATION, DECELERATION);
        PlayerController.UpdateGravity((float)delta);
        PlayerController.UpdateVelocity();
        AnimationController.UpdateLeaning(false, 0.0f, 0.0f, 0.0f);
    }
}
