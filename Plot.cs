using Godot;
using System;
using GodotGyro;

public partial class Plot : Control
{
    private static int resolution = 50;
    private Vector2[] points;

    public override void _Ready()
    {
        points =  new Vector2[resolution + 1];
        for (int i = 0; i <= resolution; i++)
        {
            points[i] = new Vector2(i, i) / resolution;
        }
        Update();
    }

    public override void _Draw()
    {
        DrawPolyline(ScaledPoints(), Colors.GreenYellow, 1f, true);
        QueueRedraw();
    }

    public void Update()
    {
        QueueRedraw();
    }

    private Vector2[] ScaledPoints()
    {
        var scaledPoints = new Vector2[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            var foo = points[i];
            foo = foo with { Y = Mathf.Ease(foo.Y, Singleton<ControllerSettings>.Instance.Curve) };
            foo = foo with { Y = 1f - foo.Y }; // Convert to 0,0 in bottom-left instead of top-left
            scaledPoints[i] = foo * new Vector2(320, 320);
        }
        return scaledPoints;
    }
}
