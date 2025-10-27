using GyroHelpers;

namespace GodotGyro;

public enum GyroActivationMode
{
    ALWAYS_ON,
    ALWAYS_OFF,
    WHEN_AIMING,
    TOGGLE,
}

public enum GyroRatchet
{
    TOUCHPAD,
    BUTTON,
}

public enum GyroOrientation
{
    YAW,
    ROLL,
    PLAYERSPACE,
    WORLDSPACE,
}

public class GyroSettings
{
    public float Sensitivity { get; set; } = 6f;
    public float VerticalSensMul { get; set; } = 0.67f;
    public GyroActivationMode ActivationMode { get; set; }  = GyroActivationMode.ALWAYS_ON;
    public GyroRatchet Ratchet { get; set; } =  GyroRatchet.TOUCHPAD;

    private float minThreshold = 0.36f;
    public float MinThreshold
    {
        get => minThreshold;
        set
        {
            minThreshold = value;
            Singleton<GyroProcessor>.Instance.TighteningThreshold = value;
        }
    }
    public float PrecisionThreshold { get; set; } = 0.75f;
    public bool FlickStickEnabled { get; set; }
}