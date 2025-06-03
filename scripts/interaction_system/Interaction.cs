using Godot;
using System;

public partial class Interaction : RayCast3D
{
    [ExportCategory("Interaction")]
    [Export] private string _context;
    [Export] private Texture2D _textureIcon;
    [Export] private bool _isOverride = false;

    public float InteractionDistance = 2.0f;
    public Node Interactor;

    private ContextComponent _contextComponent;
    private IInteractable _interactTarget;
    private GodotObject _currentCastResult;

    public override void _Ready()
    {
        _contextComponent = ServiceLocator.GetService<ContextComponent>();
    }

    public override void _PhysicsProcess(double delta)
    {
        CheckCastResult();

        if (Input.IsActionJustPressed("interact")) 
        {
            InteractionCast();
        }
    }

    private void InteractionCast() 
    {
        _currentCastResult = GetCollider();

        if (_currentCastResult != null)
        {
            GD.Print("Collision detected: " + _currentCastResult);

            Node colliderNode = _currentCastResult as Node;

            if(colliderNode is IInteractable interactable) 
            {
                _interactTarget = interactable;
                _interactTarget?.Interact();
                _interactTarget?.Interact(colliderNode);
            }
            else 
            {
                GD.Print("There is no Interactable target");
                _interactTarget = null;
            }
        }
        else 
        {
            _interactTarget = null;
        }
    }

    private void CheckCastResult()
    {
        if (IsColliding())
        {
            _currentCastResult = GetCollider();

            if(_currentCastResult != null) 
            {
                Node colliderNode = _currentCastResult as Node;

                if(colliderNode is IInteractable interactable)
                {
                    _interactTarget = interactable;
                    _contextComponent.Visible = true;
                    _contextComponent.UpdateLabel($"Pick up '{colliderNode.Name.ToString()}'");
                    _contextComponent.UpdateIcon(_textureIcon, _isOverride);
                }
                else
                {
                    _interactTarget = null;
                    _contextComponent.Visible = false;
                    _contextComponent.Reset();
                }
            }
        }
        else
        {
            _currentCastResult = null;
            _contextComponent.Visible = false;
            _contextComponent.Reset();
            _interactTarget = null;
        }
    }
}
