using Godot;
using System;

public partial class MainMenu : CanvasLayer
{
    [ExportCategory("Main Menu Controls")]
    [Export] private Control _mainMenuControl;
    [Export] private Control _optionsControl;

    [ExportCategory("Main Menu Buttons")]
    [Export] private Button _singleplayerButton;
    [Export] private Button _multiplayerButton;
    [Export] private Button _optionButton;
    [Export] private Button _exitButton;


    public override void _EnterTree()
    {
        ServiceLocator.RegisterService(this);
    }

    public override void _Ready()
    {
        _singleplayerButton.Pressed += OnSingleplayerButtonPressed;
        _multiplayerButton.Pressed += OnMultiplayerButtonPressed;
        _optionButton.Pressed += OnOptionsButtonPressed;
        _exitButton.Pressed += OnExitButtonPressed;
    }

    private void OnSingleplayerButtonPressed() 
    {
        throw new NotImplementedException();
    }

    private void OnMultiplayerButtonPressed()
    {
        throw new NotImplementedException();
    }

    private void OnOptionsButtonPressed()
    {
        _mainMenuControl.Visible = false;
        _optionsControl.Visible = true;
    }

    private void OnExitButtonPressed()
    {
        GetTree().Quit();
    }
}
