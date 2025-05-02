using Godot;
using System;
using System.Collections.Generic;

public partial class StateMachine : Node
{
    [Export]
    public State CurrentState;

    private Dictionary<StringName, State> _states = new();


    public override void _Ready()
    {
        DebugSingleton.PlayerStateMachine = this;

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
        CallDeferred(nameof(EnterInitialState));
    }

    private void EnterInitialState()
    {
        if (CurrentState != null) 
        {
            CurrentState.Enter(null);
        }
        else 
        {
            GD.PushWarning("CurrentState is not set up");
        }
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
        StringName key = newStateName;
        if (_states.TryGetValue(key, out State newState) && newState != CurrentState) 
        {
            CurrentState?.Exit();
            newState.Enter(CurrentState);
            CurrentState = newState;
        }
        else
        {
            GD.PushWarning($"State '{newStateName}' does not exist or is already active.");
        }
    }
}
