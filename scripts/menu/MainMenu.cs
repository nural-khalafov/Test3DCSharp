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

    private const string LoadingScreenScenePath = "res://scenes/loading_screen.tscn";

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
        LoadingScreen.SceneToLoad = "res://scenes/test_level.tscn";
        GetTree().ChangeSceneToFile(LoadingScreenScenePath);
    }

    private void OnMultiplayerButtonPressed()
    {
        LoadingScreen.SceneToLoad = "res://scenes/test_multiplayer.tscn";
        GetTree().ChangeSceneToFile(LoadingScreenScenePath);
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
