using Godot;
using System;

public partial class ContextComponent : CenterContainer
{
    [Export] public TextureRect TextureIcon { get; set; }
    [Export] public Label ContextLabel { get; set; }
    [Export] public Texture2D DefaultIcon { get; set; }

    public override void _EnterTree()
    {
        ServiceLocator.RegisterService(this);
    }

    public override void _Ready()
    {
        Reset();
    }

    public void Reset()
    {
        TextureIcon.Texture = null;
        ContextLabel.Text = string.Empty;
    }

    public void UpdateIcon(Texture2D image, bool isOverride) 
    {
        if(isOverride) 
        {
            TextureIcon.Texture = image;
        }
        else 
        {
            TextureIcon.Texture = DefaultIcon;
        }
    }

    public void UpdateLabel(string text) 
    {
        ContextLabel.Text = text;
    }
}
