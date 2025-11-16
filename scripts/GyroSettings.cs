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
    /// <summary>
    /// Gyro sensitivity multiplier in Real World Scale.
    /// Example: Value of 1f will translate 1° of controller motion to 1° in the game.
    /// </summary>
    public float Sensitivity { get; set; } = 6f;
    /// <summary>
    /// Vertical sensitivity multiplier applied on top of <see cref="GyroSettings.Sensitivity">Sensitivity</see>
    /// Example: Value of 0.5f will translate 10° of vertical rotation to 5°
    /// </summary>
    public float VerticalSensMul { get; set; } = 0.67f;
    public GyroActivationMode ActivationMode { get; set; }  = GyroActivationMode.ALWAYS_ON;
    public GyroRatchet Ratchet { get; set; } =  GyroRatchet.TOUCHPAD;
    private float tighteningThreshold = 0.75f;
    /// <summary>
    /// Gyro tightening value in accordance with <see href="http://gyrowiki.jibbsmart.com/blog:good-gyro-controls-part-1:the-gyro-is-a-mouse">Good Gyro Controls Part 1: The Gyro is a Mouse</see>
    /// </summary>
    public float TighteningThreshold
    {
        get => tighteningThreshold;
        set
        {
            tighteningThreshold = value;
            Singleton<GyroProcessor>.Instance.TighteningThreshold = value;
        }
    }
    public bool FlickStickEnabled { get; set; }
}