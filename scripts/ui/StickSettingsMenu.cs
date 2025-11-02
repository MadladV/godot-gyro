using Godot;

namespace GodotGyro;

public partial class StickSettingsMenu : PanelContainer
{
    [Export] private SettingSlider turnRate, deadzone, axialDeadzone, verticalSensitivity, curve;
    [Export] private Plot plot;

    public override void _Ready()
    {
        turnRate.OnValueChanged += (_, value) =>
        {
            Singleton<ControllerSettings>.Instance.TurnRate = value;
        };
        verticalSensitivity.OnValueChanged += (_, value) =>
        {
            Singleton<ControllerSettings>.Instance.VerticalSensitivity = value;
        };
        deadzone.OnValueChanged += (_, value) =>
        {
            Singleton<ControllerSettings>.Instance.Deadzone = value;
        };
        axialDeadzone.OnValueChanged += (_, value) =>
        {
            Singleton<ControllerSettings>.Instance.AxialDeadzone = value;
        };
        curve.OnValueChanged += (_, value) =>
        {
            Singleton<ControllerSettings>.Instance.Curve = value;
            plot.Update();
        };
    }
}
