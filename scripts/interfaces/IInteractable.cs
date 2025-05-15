using Godot;
using System;

public interface IInteractable
{
    void Interact();

    void Interact(Node interactor);
}
