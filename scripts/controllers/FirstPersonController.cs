using Godot;

public partial class FirstPersonController : CharacterBody3D
{
    [ExportCategory("Player Camera Settings")]
    [Export] public float MouseSensitivity = 0.07f;
    [Export] public float CameraFollowSpeed = 20.0f;

    [ExportCategory("Node Components")]
    [Export] public ShapeCast3D CrouchShapeCast;
    [Export] public CollisionShape3D CollisionShape3D;

    [ExportCategory("Camera Components")]
    [Export] public Camera3D Camera;
    [Export] public Marker3D HeadTarget;

    private float _tiltMinLimit = Mathf.DegToRad(-75.0f);
    private float _tiltMaxLimit = Mathf.DegToRad(75.0f);

    private bool _mouseInput = false;
    private float _rotationInput;
    private float _tiltInput;
    private Vector3 _mouseRotation;
    private Vector3 _playerRotation;
    private Vector3 _cameraRotation;

    private bool _menuToggled = false;
    private Vector2 _inputDirection;

    private float _gravity;

    public static Camera3D CameraRef;
    public static Vector3 MouseRotation;

    public override void _Ready()
    {
        _gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
        Input.MouseMode = Input.MouseModeEnum.Captured;
        DebugSingleton.Player = this;
        CameraRef = Camera;
    }

    public override void _Process(double delta)
    {
        // rightHandIK.Start();
        // leftHandIK.Start();
        MouseRotation.X += _tiltInput * (float)delta;
        MouseRotation.X = Mathf.Clamp(MouseRotation.X, _tiltMinLimit, _tiltMaxLimit);
        MouseRotation.Y += _rotationInput * (float)delta;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        _mouseInput = @event is InputEventMouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured;
        if (_mouseInput)
        {
            var mouseMotion = @event as InputEventMouseMotion;
            _rotationInput = -mouseMotion.Relative.X * MouseSensitivity;
            _tiltInput = -mouseMotion.Relative.Y * MouseSensitivity;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        UpdateCamera((float)delta);
        UpdateCameraFollow((float)delta);
        CrouchShapeCast.AddException(this);
    }

    private void UpdateCamera(float delta)
    {
        _mouseRotation.X += _tiltInput * delta;
        _mouseRotation.X = Mathf.Clamp(_mouseRotation.X, _tiltMinLimit, _tiltMaxLimit);
        _mouseRotation.Y += _rotationInput * delta;

        _playerRotation = new Vector3(0, _mouseRotation.Y, 0);
        _cameraRotation = new Vector3(_mouseRotation.X, 0, 0);

        Camera.Transform = new Transform3D(Basis.FromEuler(_cameraRotation), Camera.Transform.Origin);
        GlobalTransform = new Transform3D(Basis.FromEuler(_playerRotation), GlobalTransform.Origin);

        Camera.Rotation = new Vector3(Camera.Rotation.X, Camera.Rotation.Y, 0);

        _rotationInput = 0.0f;
        _tiltInput = 0.0f;
    }

    public Vector2 GetInputDirection()
    {
        _inputDirection = Input.GetVector("left", "right", "up", "down");
        return _inputDirection;
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

    private void UpdateCameraFollow(float delta)
    {
        var desiredPosition = HeadTarget.GlobalTransform.Origin;
        var currentPosition = Camera.GlobalTransform.Origin;
        Camera.GlobalTransform = new Transform3D(Camera.GlobalTransform.Basis, currentPosition.Lerp(desiredPosition, CameraFollowSpeed * delta));
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
