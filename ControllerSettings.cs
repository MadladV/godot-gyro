namespace GodotGyro;

public class ControllerSettings
{
    private float deadzone = 0.1f;
    public float Deadzone
    {
        get => deadzone; set => deadzone = value; 
    }

    private float axialDeadzone = 0f;

    public float AxialDeadzone
    {
        get => axialDeadzone;
        set => axialDeadzone = value;
    }

    private float turnrate = 360f;
    public float Turnrate
    {
        get => turnrate; 
        set => turnrate = value;
    }

    private float vertSensitivity = 0.67f;
    public float VertSensitivity 
    {
        get => vertSensitivity; 
        set => vertSensitivity = value;
    }

    private float curve = 3f; // https://byteatatime.dev/posts/easings/
    public float Curve 
    {
        get => curve;
        set => curve = value;
    }
}