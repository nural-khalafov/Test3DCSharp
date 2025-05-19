using Godot;
using System;
using System.Threading.Tasks;

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
    [Export] public SkeletonIK3D _rightHandIK;
    [Export] public SkeletonIK3D LeftHandIK;

    private float _hipsUnarmedRotationY = -25.0f;
    private float _hipsArmedRotationY = -41.0f;
    private float _leanPositionAmount = 0.0f;
    private float _leanRotationAmount = 0.0f;

    private const string ANIM_TREE_BASE_PATH = "parameters/";

    private const string WEAPON_IDLE_BLENDPOS = "parameters/UpperbodyStateMachine/ArmedState/ArmedStates/WeaponIdleBS1D/blend_position";

    public const string IsArmedParam = "is_armed";
    public const string WeaponStanceParam = "WeaponStance";

    public string Transition = "Transition";
    public string WalkBlendPos = "parameters/PlayerStateMachine/Standing/WalkBlendSpace2D/blend_position";
    public string CrouchBlendPos = "parameters/PlayerStateMachine/Crouched/CrouchingBlendSpace2D/blend_position";
    public string JumpBlendPos = "parameters/PlayerStateMachine/Standing/JumpBlendSpace1D/blend_position";
    public string SprintBlendPos = "parameters/PlayerStateMachine/Standing/SprintBlendSpace1D/blend_position";
    public string UpperbodyUnarmedBlendPos = "parameters/UpperbodyStateMachine/UnarmedState/Movement/blend_position";

    public bool IsArmed => (bool)AnimationTree.Get(IsArmedParam);

    public override void _EnterTree()
    {
        ServiceLocator.RegisterService(this);
    }

    public override void _Ready()
    {
        if (AnimationTree == null)
            GD.PrintErr("AnimationTree is null.");

        _hipsIK.Start();

        SetArmedState(false, WeaponType.None);
    }

    public override void _Process(double delta)
    {
        UpdateHipsRotation();
    }

    public void UpdateHipsRotation()
    {
        if(FirstPersonController.CameraRef != null) 
        {
            if(IsArmed) 
            {
                _hipsTarget.Position = new Vector3(_leanPositionAmount,
                _hipsTarget.Position.Y, _hipsTarget.Position.Z);
                _hipsTarget.Rotation = new Vector3(-FirstPersonController.CameraRef.Rotation.X,
                Mathf.DegToRad(_hipsArmedRotationY), _leanRotationAmount);
            }
            else 
            {
                _hipsTarget.Position = new Vector3(_leanPositionAmount,
                _hipsTarget.Position.Y, _hipsTarget.Position.Z);
                _hipsTarget.Rotation = new Vector3(-FirstPersonController.CameraRef.Rotation.X,
                Mathf.DegToRad(_hipsUnarmedRotationY), _leanRotationAmount);
            }
        }
    }

    public void UpdateLeaning(bool canLean, float delta, float negValue, float posValue)
    {
        if (_hipsTarget == null)
            return;

        float targetLeanRotation = _leanRotationAmount;
        float targetLeanPosition = _leanPositionAmount;
        if (canLean)
        {
            if (Input.IsActionPressed("lean_left"))
            {
                targetLeanPosition = 0.2f;
                targetLeanRotation = negValue;
            }
            else if (Input.IsActionPressed("lean_right"))
            {
                targetLeanPosition = -0.2f;
                targetLeanRotation = posValue;
            }
            else 
            {
                targetLeanPosition = 0.0f;
                targetLeanRotation = 0.0f;
            }
        }
        else
        {
            targetLeanRotation = 0.0f;
        }
        _leanPositionAmount = Mathf.Lerp(_leanPositionAmount, targetLeanPosition, (float)delta * 7f);
        _leanRotationAmount = Mathf.LerpAngle(_leanRotationAmount, targetLeanRotation, (float)delta * 7f);
    }

    public void SetArmedState(bool isArmed, WeaponType weaponType) 
    {
        if (AnimationTree == null)
        {
            GD.PrintErr("SetArmedState: AnimationTree is null.");
            return;
        }

        AnimationTree.Set(IsArmedParam, isArmed);

        int stanceIndex = 0; // 0 - unarmed
        if (isArmed) 
        {
            switch(weaponType) 
            {
                case WeaponType.AssaultRifle:
                    stanceIndex = 1;
                    break;
                case WeaponType.Pistol:
                    stanceIndex = 2;
                    break;
                case WeaponType.Melee:
                    stanceIndex = 3;
                    break;
            }
        }
        AnimationTree.Set(WEAPON_IDLE_BLENDPOS, stanceIndex);
    }
}
