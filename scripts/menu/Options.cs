using Godot;
using System;

public partial class Options : Panel
{
    [Export]
    private CheckButton _fullscreenSwitcher;

    public override void _Ready()
    {
        MenuSingleton.Options = this;

        Visible = false;

        _fullscreenSwitcher.Toggled += OnFullScreenSwitcherToggled;
    }

    private void OnFullScreenSwitcherToggled(bool toggledOn) 
    {
        if (toggledOn)
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
            GD.Print("FULLSCREEN MODE ENABLED");
        }
        else
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
        }
    }
}
