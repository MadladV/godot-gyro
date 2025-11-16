using Godot;
using GyroHelpers;
using GyroHelpers.GyroSpaces;

namespace GodotGyro;
public partial class GyroSettingsMenu : PanelContainer
{
    [ExportGroup("Motion controls")]
    [Export] private SettingSlider sensitivity, vertSensitivity, tighteningThreshold;
    [Export] private OptionButton activationMode, gyroRatchet, gyroOrientation;
    [ExportGroup("Flick Stick")]
    [Export] private CheckButton flickStickEnabled;
    [Export] private VBoxContainer flickStickSettingsUI;
    [Export] private SettingSlider flickThreshold, flickTime;
    [Export] private OptionButton snapping;

    public override void _Ready()
    {
        sensitivity.OnValueChanged += (_, value) =>
        {
            Singleton<GyroSettings>.Instance.Sensitivity = value;
        };
        vertSensitivity.OnValueChanged += (_, value) =>
        {
            Singleton<GyroSettings>.Instance.VerticalSensMul = value;
        };
        activationMode.ItemSelected += (value) =>
        {
            Singleton<GyroSettings>.Instance.ActivationMode = (GyroActivationMode)value;
        };
        gyroRatchet.ItemSelected += (value) =>
        {
            Singleton<GyroSettings>.Instance.Ratchet = (GyroRatchet)value;
        };
        gyroOrientation.ItemSelected += (value) =>
        {
            switch ((GyroOrientation)value)
            {
                case GyroOrientation.YAW:
                    Singleton<GyroProcessor>.Instance.GyroSpace = new LocalGyroSpace(GyroAxis.Yaw);
                    break;
                case GyroOrientation.ROLL:
                    Singleton<GyroProcessor>.Instance.GyroSpace = new LocalGyroSpace(GyroAxis.Roll);
                    break;
                case GyroOrientation.PLAYERSPACE:
                    Singleton<GyroProcessor>.Instance.GyroSpace = new PlayerGyroSpace();
                    break;
                case GyroOrientation.WORLDSPACE:
                    Singleton<GyroProcessor>.Instance.GyroSpace = new WorldGyroSpace();
                    break;
            }
        };
        tighteningThreshold.OnValueChanged += (_, value) =>
        {
            Singleton<GyroSettings>.Instance.TighteningThreshold = value;
        };
        
        // Flick stick settings
        flickStickEnabled.Toggled += (value) =>
        {
            Singleton<GyroSettings>.Instance.FlickStickEnabled = value;
            flickStickSettingsUI.Visible = value;
        };
        flickThreshold.OnValueChanged += (_, value) =>
        {   
            Singleton<FlickStick>.Instance.FlickThreshold = value;
        };
        flickTime.OnValueChanged += (_, value) =>
        {
            Singleton<FlickStick>.Instance.FlickTime = value;
        };
        snapping.ItemSelected += (value) =>
        {
            Singleton<FlickStick>.Instance.Snapping = (FlickSnapping)value;
        };
    }
}
