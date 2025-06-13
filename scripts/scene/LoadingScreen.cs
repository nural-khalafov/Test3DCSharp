using Godot;
using System;

public partial class LoadingScreen : Control
{
    [Export] private ProgressBar _progressBar;
    private string _targetScenePath;
    private ResourceLoader.ThreadLoadStatus _loadStatus;
    private PackedScene _loadedScene;

    public static string SceneToLoad { get; set; }

    public override void _Ready()
    {
        if(_progressBar == null) 
        {
            GD.PrintErr("ProgressBar not assigned in LoadingScreen.");

            GetTree().ChangeSceneToFile("res://scenes/main_menu.tscn");
            return;
        }
        _progressBar.Value = 0.0f;

        _targetScenePath = SceneToLoad;

        if (string.IsNullOrEmpty(_targetScenePath))
        {
            GD.PrintErr("No scene specified to load.");

            GetTree().ChangeSceneToFile("res://scenes/main_menu.tscn");
            return;
        }

        StartLoading();
    }

    public override void _Process(double delta)
    {
        if(string.IsNullOrEmpty(_targetScenePath)) 
        {
            return;
        }

        if(_loadStatus == ResourceLoader.ThreadLoadStatus.InProgress) 
        {
            Godot.Collections.Array progressArray = new Godot.Collections.Array();

            _loadStatus = ResourceLoader.LoadThreadedGetStatus(_targetScenePath, progressArray);

            if(progressArray.Count > 0 && _progressBar != null) 
            {
                // Assuming the first element is the progress percentage
                _progressBar.Value = (float)progressArray[0] * 100;
            }
        }

        if(_loadStatus == ResourceLoader.ThreadLoadStatus.Loaded) 
        {
            if (_progressBar != null)
                _progressBar.Value = 100.0f;

            _loadedScene = (PackedScene)ResourceLoader.LoadThreadedGet(_targetScenePath);
            if(_loadedScene != null) 
            {
                GD.Print($"Successfully loaded scene: {_targetScenePath}");
                GetTree().Root.AddChild(_loadedScene.Instantiate());
                QueueFree();
            }
            else 
            {
                GD.PrintErr($"Failed to load scene: {_targetScenePath}. The loaded scene is null.");
                GetTree().ChangeSceneToFile("res://scenes/main_menu.tscn");
            }
            _targetScenePath = null; // Clear the target scene path to prevent reloading
        }
        else if(_loadStatus == ResourceLoader.ThreadLoadStatus.Failed ||
            _loadStatus == ResourceLoader.ThreadLoadStatus.InvalidResource) 
        {
            GD.PrintErr($"Failed to load scene: {_targetScenePath}. Status: {_loadStatus}");
            GetTree().ChangeSceneToFile("res://scenes/main_menu.tscn");
            _targetScenePath = null; // Clear the target scene path to prevent reloading
        }
    }

    private void StartLoading() 
    {
        Error err = ResourceLoader.LoadThreadedRequest(_targetScenePath, "", true);
        if(err != Error.Ok) 
        {
            GD.PrintErr($"Failed to start loading scene: {_targetScenePath}. Error: {err}");
            GetTree().ChangeSceneToFile("res://scenes/main_menu.tscn");
            _targetScenePath = null;
            return;
        }
        _loadStatus = ResourceLoader.LoadThreadedGetStatus(_targetScenePath);
    }
}
