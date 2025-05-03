using Godot;
using System;

public partial class Menu : Panel
{
    [Export]
    private TextureButton _resumeButton;
    [Export]
    private TextureButton _optionsButton;
    [Export]
    private TextureButton _exitButton;
    public override void _Ready()
    {
        MenuSingleton.Menu = this;

        _resumeButton.Pressed += OnResumeButtonPressed;
        _optionsButton.Pressed += OnOptionsButtonPressed;
        _exitButton.Pressed += OnExitButtonPressed;

        Visible = false;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("exit")) 
        {
            if (!MenuSingleton.MenuToggled) 
            {
                MenuSingleton.Menu.Visible = true;
                MenuSingleton.MenuToggled = true;
                Input.MouseMode = Input.MouseModeEnum.Visible;
                GetTree().Paused = true;
            }
            else if (MenuSingleton.MenuToggled)
            {
                MenuSingleton.Menu.Visible = false;
                MenuSingleton.Options.Visible = false;
                MenuSingleton.MenuToggled = false;
                Input.MouseMode = Input.MouseModeEnum.Captured;
                GetTree().Paused = false;
            }
        }
    }

    private void OnResumeButtonPressed() 
    {
        Visible = false;
        Input.MouseMode = Input.MouseModeEnum.Captured;
        MenuSingleton.MenuToggled = false;
        GetTree().Paused = false;
    }

    private void OnOptionsButtonPressed() 
    {
        Visible = false;
        MenuSingleton.Options.Visible = true;
    }

    private void OnExitButtonPressed()
    {
        GetTree().Quit();
    }
}
