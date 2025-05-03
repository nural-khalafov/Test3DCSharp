public partial class PlayerMovementState : State
{
    public FirstPersonController PlayerController;

    public override async void _Ready()
    {
        await ToSignal(Owner, "ready");
        PlayerController = Owner as FirstPersonController;
    }

    public override void _Process(double delta)
    {
        // NULL
    }
}
