namespace GodotGyro;

public class ControllerSettings
{
    public float Deadzone { get; set; } = 0.1f;
    public float AxialDeadzone { get; set; }
    public float TurnRate { get; set; } = 360f;
    public float VerticalSensitivity { get; set; } = 0.67f;
    public float Curve { get; set; } = 3f;
}