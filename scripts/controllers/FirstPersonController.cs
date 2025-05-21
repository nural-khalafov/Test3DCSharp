using Godot;
using System;


public enum CameraMode
{
    FirstPerson,
    FreeFlow
}
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

    [ExportCategory("Free Flow Camera Settings")]
    [Export] public CameraMode CameraMode = CameraMode.FirstPerson;
    [Export] public float FreeFlowMovementSpeed { get; set; } = 5.0f;
    [Export] public float FreeFlowMouseSensitivity { get; set; } = 0.002f;
    [Export] public float FreeFlowShiftSpeedMultiplier { get; set; } = 2.5f;

    private float _tiltMinLimit = Mathf.DegToRad(-75.0f);
    private float _tiltMaxLimit = Mathf.DegToRad(75.0f);

    private bool _mouseInput = false;
    private float _rotationInput;
    private float _tiltInput;
    private Vector3 _mouseRotation;
    private Vector3 _playerRotation;
    private Vector3 _cameraRotation;
    private Vector3 _cameraVelocity = Vector3.Zero;

    private Vector2 _freeFlowMouseDelta;

    private Vector2 _inputDirection;

    private float _gravity;

    public static Camera3D CameraRef;
    public static CameraMode CameraModeRef;


    public override void _EnterTree()
    {
        ServiceLocator.RegisterService(this);
    }

    public override void _Ready()
    {
        _gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
        Input.MouseMode = Input.MouseModeEnum.Captured;
        CameraRef = Camera;
    }

    public override void _Process(double delta)
    {
        CameraModeRef = CameraMode;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("debug_camera"))
        {
            if (CameraMode == CameraMode.FirstPerson)
            {
                CameraMode = CameraMode.FreeFlow;
            }
            else
            {
                CameraMode = CameraMode.FirstPerson;
                _mouseRotation.X = Camera.Rotation.X;
                _mouseRotation.Y = GlobalRotation.Y;
            }
            GD.Print($"Camera mode switched to: {CameraMode}");
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        _mouseInput = @event is InputEventMouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured;
        if (_mouseInput)
        {
            var mouseMotion = @event as InputEventMouseMotion;

            if (CameraMode == CameraMode.FirstPerson)
            {
                _rotationInput = -mouseMotion.Relative.X * MouseSensitivity;
                _tiltInput = -mouseMotion.Relative.Y * MouseSensitivity;
            }
            else
            {
                _freeFlowMouseDelta = mouseMotion.Relative;
            }
        }
        else
        {
            _freeFlowMouseDelta = Vector2.Zero;
            if (CameraMode == CameraMode.FirstPerson)
            {
                _rotationInput = 0;
                _tiltInput = 0;
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (CameraMode == CameraMode.FirstPerson)
        {
            UpdateCamera((float)delta);
            UpdateCameraFollow((float)delta);
        }
        else
        {
            UpdateFreeFlowCamera((float)delta);
            Velocity = Vector3.Zero;
        }

        CrouchShapeCast.AddException(this);
    }

    private void UpdateFreeFlowCamera(float delta)
    {
        if (_freeFlowMouseDelta.LengthSquared() > 0 && Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            Camera.RotateY(-_freeFlowMouseDelta.X * FreeFlowMouseSensitivity);
            Camera.RotateObjectLocal(Vector3.Right, -_freeFlowMouseDelta.Y * FreeFlowMouseSensitivity);

            Vector3 currentRotation = Camera.RotationDegrees;
            currentRotation.X = Mathf.Clamp(currentRotation.X, -89.9f, 89.9f);
            Camera.RotationDegrees = currentRotation;
        }
        _freeFlowMouseDelta = Vector2.Zero;

        Vector3 inputDir = Vector3.Zero;
        if (Input.IsActionPressed("up"))
            inputDir.Z -= 1;
        if (Input.IsActionPressed("down"))
            inputDir.Z += 1;
        if (Input.IsActionPressed("left"))
            inputDir.X -= 1;
        if (Input.IsActionPressed("right"))
            inputDir.X += 1;
        if (Input.IsActionPressed("camera_up"))
            inputDir.Y += 1;
        if (Input.IsActionPressed("camera_down"))
            inputDir.Y -= 1;

        float currentSpeed = FreeFlowMovementSpeed;
        if (Input.IsActionPressed("camera_sprint"))
        {
            currentSpeed *= FreeFlowShiftSpeedMultiplier;
        }

        if (inputDir != Vector3.Zero)
        {
            Vector3 direction = (Camera.GlobalTransform.Basis * inputDir.Normalized().Normalized());
            Camera.GlobalTranslate(direction * currentSpeed * delta);
        }
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
        if (CameraMode == CameraMode.FreeFlow)
        {
            return Vector2.Zero;
        }

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
        if (CameraMode == CameraMode.FirstPerson)
            Velocity = new Vector3(Velocity.X, Velocity.Y - _gravity * delta, Velocity.Z);
    }

    private void UpdateCameraFollow(float delta)
    {
        if (CameraMode == CameraMode.FirstPerson)
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
        else
        {
            _cameraVelocity = Vector3.Zero;
        }
    }
}
