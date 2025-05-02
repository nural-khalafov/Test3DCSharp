using Godot;
using System;

public partial class FirstPersonController : CharacterBody3D
{
    [ExportCategory("Player Camera Settings")]
    [Export] public float MouseSensitivity = 0.25f;
    [Export] public float CameraFollowSpeed = 20.0f;

    [ExportCategory("Node Components")]
    [Export] public ShapeCast3D CrouchShapeCast;
    [Export] public CollisionShape3D CollisionShape3D;

    private float _tiltMinLimit = Mathf.DegToRad(-75.0f);
    private float _tiltMaxLimit = Mathf.DegToRad(75.0f);

    private float leanBlendTarget = 1.0f;
    private string leanBlendPosition = "parameters/LeanBlendSpace1D/blend_position";

    private float aimIdleFOV = 90.0f;
    private Vector3 aimIdlePosition = new Vector3(0.192f, -0.236f, 0.151f);
    private Vector3 aimFiringPosition = new Vector3(0.037f, -0.191f, 0.221f);
    private float aimFiringFOV = 60.0f;

    private bool mouseInput = false;
    private float rotationInput;
    private float tiltInput;
    private Vector3 mouseRotation;
    private Vector3 playerRotation;
    private Vector3 cameraRotation;

    private bool menuToggled = false;
    private Vector2 inputDirection;

    [Export] public AnimationTree animationTree;
    [Export] public AnimationPlayer animationPlayer;
    [Export] private Skeleton3D skeleton;
    [Export] public Camera3D camera;

    [Export] private Marker3D headTarget;

    [Export] private SkeletonIK3D rightHandIK;
    [Export] private SkeletonIK3D leftHandIK;
    [Export] private Node3D aimTarget;
    [Export] private Node3D gripL;
    [Export] private Node3D smoothedGripL;

    private float _gravity;

    public override void _Ready()
    {
        _gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
        Input.MouseMode = Input.MouseModeEnum.Captured;
        // Global.Player = this;
        DebugSingleton.Player = this;
    }

    public override void _Process(double delta)
    {
        // rightHandIK.Start();
        // leftHandIK.Start();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        mouseInput = @event is InputEventMouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured;
        if (mouseInput)
        {
            var mouseMotion = @event as InputEventMouseMotion;
            rotationInput = -mouseMotion.Relative.X * MouseSensitivity;
            tiltInput = -mouseMotion.Relative.Y * MouseSensitivity;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        UpdateCamera((float)delta);
        UpdateCameraFollow((float)delta);
        // UpdateAiming((float)delta);
        // UpdateLeftArmGrip((float)delta);
        CrouchShapeCast.AddException(this);
    }

    private void UpdateCamera(float delta)
    {
        mouseRotation.X += tiltInput * delta;
        mouseRotation.X = Mathf.Clamp(mouseRotation.X, _tiltMinLimit, _tiltMaxLimit);
        mouseRotation.Y += rotationInput * delta;

        playerRotation = new Vector3(0, mouseRotation.Y, 0);
        cameraRotation = new Vector3(mouseRotation.X, 0, 0);

        camera.Transform = new Transform3D(Basis.FromEuler(cameraRotation), camera.Transform.Origin);
        GlobalTransform = new Transform3D(Basis.FromEuler(playerRotation), GlobalTransform.Origin);

        camera.Rotation = new Vector3(camera.Rotation.X, camera.Rotation.Y, 0);

        rotationInput = 0.0f;
        tiltInput = 0.0f;
    }

    private Vector2 GetInputDirection()
    {
        inputDirection = Input.GetVector("left", "right", "up", "down");
        return inputDirection;
    }

    public void UpdateInput(float speed, float acceleration, float deceleration)
    {
        var direction = (Transform.Basis * new Vector3(GetInputDirection().X, 0, GetInputDirection().Y)).Normalized();

        if (direction != Vector3.Zero)
        {
            Velocity = new Vector3(
                Mathf.Lerp(Velocity.X, direction.X * speed, acceleration),
                Velocity.Y,
                Mathf.Lerp(Velocity.Z, direction.Z * speed, acceleration)
            );
        }
        else
        {
            Velocity = new Vector3(
                Mathf.MoveToward(Velocity.X, 0, deceleration),
                Velocity.Y,
                Mathf.MoveToward(Velocity.Z, 0, deceleration)
            );
        }
    }

    public void UpdateVelocity()
    {
        MoveAndSlide();
    }

    public void UpdateGravity(float delta)
    {
        Velocity = new Vector3(Velocity.X, Velocity.Y - _gravity * delta, Velocity.Z);
    }

    //public void UpdateLeaning(bool canLean, float delta, float negX, float posX)
    //{
    //    if (canLean)
    //    {
    //        if (Input.IsActionPressed("lean_left"))
    //            ikEffector.Rotation = new Vector3(ikEffector.Rotation.X, ikEffector.Rotation.Y, Mathf.Lerp(ikEffector.Rotation.Z, negX, delta * 5));
    //        else if (Input.IsActionPressed("lean_right"))
    //            ikEffector.Rotation = new Vector3(ikEffector.Rotation.X, ikEffector.Rotation.Y, Mathf.Lerp(ikEffector.Rotation.Z, posX, delta * 5));
    //        else
    //            ikEffector.Rotation = new Vector3(ikEffector.Rotation.X, ikEffector.Rotation.Y, Mathf.Lerp(ikEffector.Rotation.Z, 0, delta * 5));
    //    }
    //}

    private void UpdateCameraFollow(float delta)
    {
        var desiredPosition = headTarget.GlobalTransform.Origin;
        var currentPosition = camera.GlobalTransform.Origin;
        camera.GlobalTransform = new Transform3D(camera.GlobalTransform.Basis, currentPosition.Lerp(desiredPosition, CameraFollowSpeed * delta));
    }

    //private void UpdateLeftArmGrip(float delta)
    //{
    //    var targetPos = gripL.GlobalTransform.Origin;
    //    var currentPos = smoothedGripL.GlobalTransform.Origin;
    //    smoothedGripL.GlobalTransform = new Transform3D(smoothedGripL.GlobalTransform.Basis, currentPos.Lerp(targetPos, delta * 30f));
    //    smoothedGripL.Rotation = Vector3.Zero;
    //}

    //private void UpdateAiming(float delta)
    //{
    //    if (Input.IsActionPressed("aim"))
    //    {
    //        aimTarget.Position = aimTarget.Position.Lerp(aimFiringPosition, delta * 8);
    //        camera.Fov = aimFiringFOV;
    //    }
    //    if (Input.IsActionJustReleased("aim"))
    //    {
    //        aimTarget.Position = aimIdlePosition;
    //        camera.Fov = aimIdleFOV;
    //    }
    //}
}
