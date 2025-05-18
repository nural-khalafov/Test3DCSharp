using Godot;
using System;

public partial class Options : Panel
{
    [Export]
    private CheckButton _fullscreenSwitcher;

    public override void _EnterTree()
    {
        ServiceLocator.RegisterService(this);
    }

    public override void _Ready()
    {
        Visible = false;

        if(_fullscreenSwitcher != null) 
        {
            _fullscreenSwitcher.Toggled += OnFullScreenSwitcherToggled;
            _fullscreenSwitcher.ButtonPressed = DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen;
        }
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
            GD.Print("WINDOWED MODE ENABLED");
        }
    }
}
