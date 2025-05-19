using Godot;
using System;
using System.Collections.Generic;

public partial class StateMachine : Node
{
    [Export]
    public State CurrentState;

    private Dictionary<StringName, State> _states = new();

    public override void _EnterTree()
    {
        ServiceLocator.RegisterService(this);
    }

    public override async void _Ready()
    {
        foreach (Node child in GetChildren())
        {
            if(child is State state) 
            {
                _states[child.Name] = state;
                state.Transition += OnChildTransition;
            }
            else 
            {
                GD.PushWarning("State machine contains incompatible child node");
            }
        }

        await ToSignal(Owner, "ready");
        CurrentState.Enter(null);
    }

    public override void _Process(double delta)
    {
        CurrentState?.Update(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        CurrentState?.PhysicsUpdate(delta);
    }

    private void OnChildTransition(StringName newStateName) 
    {
        var new_state = _states[newStateName];
        if (new_state != null) 
        {
            if(new_state != CurrentState)
            {
                CurrentState?.Exit();
                new_state.Enter(CurrentState);
                CurrentState = new_state;
            }
        }
        else
        {
            GD.PushWarning($"State '{newStateName}' does not exist or is already active.");
        }
    }
}
