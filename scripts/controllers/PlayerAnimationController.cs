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

    private Basis _initialHipsBasis;

    public string Transition = "Transition";
    public string WalkBlendPos = "parameters/PlayerStateMachine/Standing/WalkBlendSpace2D/blend_position";
    public string CrouchBlendPos = "parameters/PlayerStateMachine/Crouched/CrouchingBlendSpace2D/blend_position";
    public string JumpBlendPos = "parameters/PlayerStateMachine/Standing/JumpBlendSpace1D/blend_position";
    public string SprintBlendPos = "parameters/PlayerStateMachine/Standing/SprintBlendSpace1D/blend_position";

    public override void _Ready()
    {
        _hipsIK.Start();
    }

    public override void _Process(double delta)
    {
        _hipsTarget.Rotation = new Vector3(-FirstPersonController.CameraRef.Rotation.X, _hipsTarget.Rotation.Y, _hipsTarget.Rotation.Z);


    }

    //public void UpdateLeaning(bool canLean, float delta, float negX, float posX)
    //{
    //    if (canLean)
    //    {
    //        _leanIK.Start();

    //        if (Input.IsActionPressed("lean_left"))
    //        {
    //            _leanIK.Influence = 1.0f;
    //            _leanTarget.Rotation = new Vector3(_leanTarget.Rotation.X,
    //                _leanTarget.Rotation.Y,
    //                Mathf.LerpAngle(_leanTarget.Rotation.Z, negX, delta * 5));
    //            //_leanTarget.Position = new Vector3(
    //            //    Mathf.Lerp(_leanTarget.Position.X, -0.5f, delta * 5),
    //            //    _leanTarget.Position.Y,
    //            //    _leanTarget.Position.Z);
    //        }
    //        else if (Input.IsActionPressed("lean_right"))
    //        {
    //            _leanIK.Influence = 1.0f;
    //            _leanTarget.Rotation = new Vector3(_leanTarget.Rotation.X,
    //                _leanTarget.Rotation.Y,
    //                Mathf.LerpAngle(_leanTarget.Rotation.Z, posX, delta * 5));
    //            //_leanTarget.Position = new Vector3(
    //            //    Mathf.Lerp(_leanTarget.Position.X, 0.5f, delta * 5),
    //            //    _leanTarget.Position.Y,
    //            //    _leanTarget.Position.Z);
    //        }
    //        else
    //        {
    //            _leanTarget.Rotation = new Vector3(_leanTarget.Rotation.X,
    //               _leanTarget.Rotation.Y,
    //               Mathf.LerpAngle(_leanTarget.Rotation.Z, 0.0f, delta * 5));
    //            //_leanTarget.Position = new Vector3(
    //            //    Mathf.Lerp(_leanTarget.Position.X, 0.0f, delta * 5),
    //            //    _leanTarget.Position.Y,
    //            //    _leanTarget.Position.Z);
    //            _leanIK.Influence = Mathf.Lerp(_leanIK.Influence, 0.0f, delta * 5);
    //        }
    //    }
    //    else 
    //    {
    //        _leanIK.Stop();
    //        _leanIK.Influence = 0.0f;
    //    }
    //}
}
