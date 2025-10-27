using Godot;

namespace GodotGyro;
public partial class Plot : Control
{
    private const int Resolution = 50;
    private Vector2[] points;

    public override void _Ready()
    {
        points =  new Vector2[Resolution + 1];
        for (int i = 0; i <= Resolution; i++)
        {
            points[i] = new Vector2(i, i) / Resolution;
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
        Vector2[] scaledPoints = new Vector2[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            Vector2 foo = points[i];
            foo = foo with { Y = Mathf.Ease(foo.Y, Singleton<ControllerSettings>.Instance.Curve) };
            foo = foo with { Y = 1f - foo.Y }; // Convert to 0,0 in bottom-left instead of top-left
            scaledPoints[i] = foo * new Vector2(320, 320);
        }
        return scaledPoints;
    }
}
