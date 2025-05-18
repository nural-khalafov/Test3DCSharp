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

    public bool IsMenuOpen { get; private set; } = false;

    public override void _EnterTree()
    {
        MenuSingleton.Menu = this;
        ServiceLocator.RegisterService(this);
    }

    public override void _Ready()
    {
        _resumeButton.Pressed += OnResumeButtonPressed;
        _optionsButton.Pressed += OnOptionsButtonPressed;
        _exitButton.Pressed += OnExitButtonPressed;

        Visible = false;
        IsMenuOpen = false;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("exit"))
        {
            var optionsPanel = ServiceLocator.GetService<Options>();
            if (!IsMenuOpen)
            {
                this.Visible = true;
                IsMenuOpen = true;
                Input.MouseMode = Input.MouseModeEnum.Visible;
                GetTree().Paused = true;
            }
            else
            {
                this.Visible = false;
                if (optionsPanel != null)
                    optionsPanel.Visible = false;
                IsMenuOpen = false;
                Input.MouseMode = Input.MouseModeEnum.Captured;
                GetTree().Paused = false;
            }
        }
    }

    private void OnResumeButtonPressed()
    {
        this.Visible = false;
        IsMenuOpen = false;
        Input.MouseMode = Input.MouseModeEnum.Captured;
        GetTree().Paused = false;
    }

    private void OnOptionsButtonPressed()
    {
        var optionsPanel = ServiceLocator.GetService<Options>();
        if (optionsPanel != null)
        {
            this.Visible = false;
            optionsPanel.Visible = true;
        }
        else
        {
            GD.PrintErr("Menu: Options panel was not found in ServiceLocator");
        }
    }

    private void OnExitButtonPressed()
    {
        GetTree().Quit();
    }
}
