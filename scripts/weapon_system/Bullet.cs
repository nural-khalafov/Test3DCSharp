using Godot;
using System;
using System.Collections.Generic;

public partial class Bullet : RigidBody3D
{
    [ExportCategory("Bullet Data")]
    [Export] public StringName Caliber { get; set; } = "null";
    [Export] public float Speed { get; set; } = 100.0f;
    [Export] public float PenetrationRate { get; set; } = 1.0f;
    [Export] public float LifeTime { get; set; } = 5.0f;
    [Export] public float Damage { get; set; } = 10.0f;

    [ExportGroup("Bullet Debug Trail")]
    [Export] public bool DebugTrailEnabled { get; set; } = true;
    [Export] public Color DebugTrailColor { get; set; } = new Color(1.0f, 0.0f, 0.0f, 0.5f);
    [Export] public int MaxTrailPoints { get; set; } = 200;

    private SceneTreeTimer _lifetimeTimer;
    private MeshInstance3D _trailMeshInstance;
    private ImmediateMesh _trailGeometry;
    private StandardMaterial3D _trailMaterial;
    private List<Vector3> _trailPoints = new List<Vector3>();
    public void Initialize(float damage, StringName caliber)
    {
        Damage = damage; // Set the bullet's damage
        Caliber = caliber; // Set the bullet's caliber

        GD.Print($"Bullet initialized with damage: {Damage}, caliber: {caliber}");
    }

    public override void _Ready()
    {
        _lifetimeTimer = GetTree().CreateTimer(LifeTime);
        _lifetimeTimer.Timeout += OnLifetimeTimeout;

        if (DebugTrailEnabled)
        {
            _trailMeshInstance = new MeshInstance3D();
            _trailGeometry = new ImmediateMesh();
            _trailMeshInstance.Mesh = _trailGeometry;

            _trailMaterial = new StandardMaterial3D
            {
                AlbedoColor = DebugTrailColor,
                ShadingMode = StandardMaterial3D.ShadingModeEnum.Unshaded,
                Transparency = StandardMaterial3D.TransparencyEnum.Alpha,
                NoDepthTest = true,
            };
            _trailMeshInstance.MaterialOverride = _trailMaterial;

            GetTree().Root.AddChild(_trailMeshInstance);
            _trailMeshInstance.GlobalTransform = Transform3D.Identity; // Set the initial position of the trail mesh

            CallDeferred(nameof(AddInitialTrailPoint));
        }

        //this.BodyEntered += OnBodyEntered;
    }

    public override void _PhysicsProcess(double delta)
    {
        if(DebugTrailEnabled && _trailMeshInstance != null && IsInstanceValid(_trailMeshInstance) && _trailGeometry != null) 
        {
            _trailPoints.Add(GlobalTransform.Origin);

            while(MaxTrailPoints > 0 && _trailPoints.Count > MaxTrailPoints) 
            {
                _trailPoints.RemoveAt(0); // Remove the oldest point if we exceed the max trail points
            }
            UpdateTrailMesh();
        }
    }

    public void Fire() 
    {
        ApplyCentralImpulse(-GlobalTransform.Basis.Z.Normalized() * Speed);
    }

    private void AddInitialTrailPoint()
    {
        if (DebugTrailEnabled && IsInsideTree())
        {
            _trailPoints.Add(GlobalTransform.Origin);
        }
    }

    private void UpdateTrailMesh()
    {
        if (_trailPoints.Count < 2 || _trailGeometry == null)
            return;

        _trailGeometry.ClearSurfaces();

        _trailGeometry.SurfaceBegin(Mesh.PrimitiveType.LineStrip);

        for(int i = 0; i < _trailPoints.Count; i++)
        {
            _trailGeometry.SurfaceAddVertex(_trailPoints[i]);
        }
        _trailGeometry.SurfaceEnd();
    }

    private void OnLifetimeTimeout()
    {
        QueueFree(); // Remove the bullet after its lifetime expires
    }

    public override void _Notification(int what)
    {
        base._Notification(what);

        if(what == NotificationExitTree)
        {
            if(_lifetimeTimer != null) 
            {
                if(_lifetimeTimer.IsConnected(SceneTreeTimer.SignalName.Timeout, Callable.From(OnLifetimeTimeout))) 
                {
                    _lifetimeTimer.Timeout -= OnLifetimeTimeout;
                }
            }

            if (_trailMeshInstance != null && IsInstanceValid(_trailMeshInstance)) 
            {
                _trailMeshInstance.QueueFree(); // Free the trail mesh instance when the bullet is removed
                _trailMeshInstance = null;
            }
            _trailGeometry = null; // Clear the trail geometry
        }
    }
}
