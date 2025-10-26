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
    MIXED,
    PLAYERSPACE,
    WORLDSPACE,
}

public class GyroSettings
{
    public float Sensitivity { get; set; } = 6f;
    public float VerticalSensMul { get; set; } = 0.67f;
    public GyroActivationMode ActivationMode { get; set; }  = GyroActivationMode.ALWAYS_ON;
    public GyroRatchet Ratchet { get; set; } =  GyroRatchet.TOUCHPAD;
    public float MinThreshold { get; set; } = 0.36f;
    public float PrecisionThreshold { get; set; } = 0.75f;
    public GyroOrientation Orientation { get; set; } = GyroOrientation.WORLDSPACE;
    public float YawRollMix { get; set; } = 0.5f;
    public bool FlickstickEnabled { get; set; } = false;
}