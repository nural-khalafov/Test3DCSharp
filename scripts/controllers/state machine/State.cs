using Godot;
using System;

public partial class State : Node
{
    [Signal]
    public delegate void TransitionEventHandler(StringName newStateName);

    public virtual void Enter(State previousState) { }
    public virtual void Exit() { }
    public virtual void Update(double delta) { }
    public virtual void PhysicsUpdate(double delta) { }
}
