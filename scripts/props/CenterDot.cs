using Godot;
using System;

public partial class CenterDot : CenterContainer
{
    [Export]
    public float DotRadius = 1.0f;
    [Export]
    public Color DotColor = Colors.White;

    private Vector2 DotPosition = new Vector2(0,0);

    public override void _Ready()
    {
        QueueRedraw();
    }

    public override void _Draw()
    {
        DrawCircle(DotPosition, DotRadius, DotColor);
    }
}
