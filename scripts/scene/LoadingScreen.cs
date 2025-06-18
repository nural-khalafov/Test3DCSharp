using Godot;
using System;

public partial class LoadingScreen : Control
{
    [Export] private Label _loadingLabel;


    private Godot.Collections.Array _progress = new Godot.Collections.Array();
    private string _targetScenePath;
    private ResourceLoader.ThreadLoadStatus _loadStatus;
    private PackedScene _loadedSceneResource;
    public static string SceneToLoad { get; set; }

    public override void _Ready()
    {
        if(_loadingLabel == null)
        {
            GD.PrintErr("LoadingLabel is not assigned in the inspector.");
            GetTree().ChangeSceneToFile("res://scenes/main_menu.tscn");

            return;
        }

        _targetScenePath = SceneToLoad;

        if (string.IsNullOrEmpty(SceneToLoad))
        {
            GD.PrintErr("SceneToLoad is not set. Please set it before loading the scene.");
            GetTree().ChangeSceneToFile("res://scenes/main_menu.tscn");

            return;
        }
        StartLoadingScene();
    }

    public override void _Process(double delta)
    {
        if(string.IsNullOrEmpty(_targetScenePath))
        {
            GD.PrintErr("Target scene path is not set. Cannot process loading.");

            return;
        }

        _loadStatus = ResourceLoader.LoadThreadedGetStatus(_targetScenePath, _progress);

        switch (_loadStatus) 
        {
            case ResourceLoader.ThreadLoadStatus.InProgress:
                if(_progress.Count > 0)
                {
                    float progressValue = (float)_progress[0];
                    _loadingLabel.Text = $"LOADING: {progressValue * 100:F0}%";
                }
                break;
            case ResourceLoader.ThreadLoadStatus.Loaded:
                if(_loadingLabel != null)
                    _loadingLabel.Text = "LOADED!";

                GD.Print("Scene loaded successfully: " + _targetScenePath);
                var loadedScene = ResourceLoader.LoadThreadedGet(_targetScenePath) as PackedScene;
                if(loadedScene != null) 
                {
                    _targetScenePath = null;
                    GetTree().ChangeSceneToPacked(loadedScene);
                }
                else 
                {
                    GD.PrintErr($"Failed to load scene from path: {SceneToLoad}");
                    GetTree().ChangeSceneToFile("res://scenes/main_menu.tscn");
                }
                break;
            case ResourceLoader.ThreadLoadStatus.Failed:
            case ResourceLoader.ThreadLoadStatus.InvalidResource:
                GD.PrintErr($"Failed to load scene from path: '{SceneToLoad}'. Status: {_loadStatus}");
                GetTree().ChangeSceneToFile("res://scenes/main_menu.tscn");
                _targetScenePath = null;
                break;
        }
    }

    private void StartLoadingScene()
    {
        Error err = ResourceLoader.LoadThreadedRequest(_targetScenePath, "", false);
        if (err != Error.Ok)
        {
            GD.PrintErr($"Error starting load request for {_targetScenePath}: {err}");
            GetTree().ChangeSceneToFile("res://scenes/main_menu.tscn");
            _targetScenePath = null;
            return;
        }
        _loadStatus = ResourceLoader.LoadThreadedGetStatus(_targetScenePath);
    }
}