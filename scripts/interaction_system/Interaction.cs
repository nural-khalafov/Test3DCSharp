using Godot;
using System;

public partial class Interaction : RayCast3D
{
    public float InteractionDistance = 2.0f;

    public Node Interactor;

    private IInteractable _interactTarget;
    private GodotObject _currentCastResult;

    public override void _PhysicsProcess(double delta)
    {
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
}
