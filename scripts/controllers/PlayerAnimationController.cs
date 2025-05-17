using Godot;
using System;

public partial class PlayerAnimationController : Node3D
{
    [ExportCategory("Animation Components")]
    [Export] public AnimationPlayer animationPlayer { get; set; }
    [Export] public AnimationTree AnimationTree { get; set; }

    [ExportCategory("IK Components")]
    [Export] public Skeleton3D Skeleton;
    [Export] private SkeletonIK3D _hipsIK;
    [Export] private Marker3D _hipsTarget;

    [ExportCategory("Hands IK Components")]
    [Export] private SkeletonIK3D _rightHandIK;
    [Export] private SkeletonIK3D _leftHandIK;

    private float _leanAmount = 0.0f;

    public string Transition = "Transition";
    public string WalkBlendPos = "parameters/PlayerStateMachine/Standing/WalkBlendSpace2D/blend_position";
    public string CrouchBlendPos = "parameters/PlayerStateMachine/Crouched/CrouchingBlendSpace2D/blend_position";
    public string JumpBlendPos = "parameters/PlayerStateMachine/Standing/JumpBlendSpace1D/blend_position";
    public string SprintBlendPos = "parameters/PlayerStateMachine/Standing/SprintBlendSpace1D/blend_position";

    public override void _Ready()
    {
        _hipsIK.Start();
        //_rightHandIK.Start();
        _leftHandIK.Start();
    }

    public override void _Process(double delta)
    {
        UpdateHipsRotation();
    }

    public void UpdateHipsRotation()
    {
        _hipsTarget.Rotation = new Vector3(-FirstPersonController.CameraRef.Rotation.X,
            _hipsTarget.Rotation.Y, _leanAmount);
    }

    public void UpdateLeaning(bool canLean, float delta, float negValue, float posValue)
    {
        if (canLean) 
        {
            if(_hipsTarget != null) 
            {
                if (Input.IsActionPressed("lean_left"))
                {
                    _leanAmount = Mathf.LerpAngle(_hipsTarget.Rotation.Z, negValue, delta * 5);
                    _hipsTarget.Rotation = new Vector3(_hipsTarget.Rotation.X,
                        _hipsTarget.Rotation.Y,
                        _leanAmount);
                }
                else if (Input.IsActionPressed("lean_right"))
                {
                    _leanAmount = Mathf.LerpAngle(_hipsTarget.Rotation.Z, posValue, delta * 5);
                    _hipsTarget.Rotation = new Vector3(_hipsTarget.Rotation.X,
                        _hipsTarget.Rotation.Y,
                        _leanAmount);
                }
                else
                {
                    _leanAmount = Mathf.LerpAngle(_hipsTarget.Rotation.Z, 0.0f, delta * 5);
                    _hipsTarget.Rotation = new Vector3(_hipsTarget.Rotation.X,
                        _hipsTarget.Rotation.Y,
                        _leanAmount);
                }
            }
            else
            {
                GD.PrintErr("Hips target is null");
            }
        }
        else 
        {
            _hipsTarget.Rotation = new Vector3(_hipsTarget.Rotation.X,
                    _hipsTarget.Rotation.Y,
                    _hipsTarget.Rotation.Z);
        }
    }
}
