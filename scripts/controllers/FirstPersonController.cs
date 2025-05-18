using Godot;
using System;

public partial class FirstPersonController : CharacterBody3D
{
    [ExportCategory("Player Camera Settings")]
    [Export] public float MouseSensitivity = 0.07f;
    [Export] public float CameraFollowSpeed = 10.0f;
    [Export] public float CameraSpringStiffness = 200.0f;
    [Export] public float CameraSpringDamping = 25.0f;

    [ExportCategory("Node Components")]
    [Export] public ShapeCast3D CrouchShapeCast;
    [Export] public CollisionShape3D CollisionShape3D;

    [ExportCategory("Camera Components")]
    [Export] public Camera3D Camera;
    [Export] public Marker3D HeadTarget;
    [Export] public Node3D AimNode;

    private float _tiltMinLimit = Mathf.DegToRad(-75.0f);
    private float _tiltMaxLimit = Mathf.DegToRad(75.0f);

    private bool _mouseInput = false;
    private float _rotationInput;
    private float _tiltInput;
    private Vector3 _mouseRotation;
    private Vector3 _playerRotation;
    private Vector3 _cameraRotation;
    private Vector3 _cameraVelocity = Vector3.Zero;

    private Vector2 _inputDirection;

    private float _gravity;

    public static Camera3D CameraRef;

    public override void _Ready()
    {
        _gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
        Input.MouseMode = Input.MouseModeEnum.Captured;
        ServiceLocator.RegisterService(this);
        CameraRef = Camera;
    }

    public override void _Process(double delta)
    {
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

    public void UpdateInput(float speed, float acceleration, float deceleration, bool sprintForwardOnly = false)
    {
        Vector2 rawInput = GetInputDirection();
        Vector3 inputVector;

        if (sprintForwardOnly) 
        {
            inputVector = new Vector3(0, 0, rawInput.Y < 0 ? rawInput.Y : 0);
        }
        else
        {
            inputVector = new Vector3(rawInput.X, 0, rawInput.Y);
        }

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

        Vector3 springForce = (desiredPosition - currentPosition) * CameraSpringStiffness;

        Vector3 dampingForce = _cameraVelocity * CameraSpringDamping;

        Vector3 netForce = springForce - dampingForce;

        _cameraVelocity += netForce * delta;

        Vector3 newPosition = currentPosition + _cameraVelocity * delta;

        Camera.GlobalTransform = new Transform3D(Camera.GlobalTransform.Basis, newPosition);
    }
}
