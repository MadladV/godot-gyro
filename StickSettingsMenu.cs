using Godot;
using System;
using GodotGyro;

public partial class StickSettingsMenu : PanelContainer
{
    [Export] private SettingSlider turnrate, deadzone, axialDeadzone, verticalSensitivity, curve;
    [Export] private Plot plot;

    public override void _Ready()
    {
        turnrate.OnValueChanged += (_, value) =>
        {
            Singleton<ControllerSettings>.Instance.Turnrate = value;
        };
        verticalSensitivity.OnValueChanged += (_, value) =>
        {
            Singleton<ControllerSettings>.Instance.VertSensitivity = value;
        };
        deadzone.OnValueChanged += (_, value) =>
        {
            Singleton<ControllerSettings>.Instance.Deadzone = value;
        };
        axialDeadzone.OnValueChanged += (_, value) =>
        {
            Singleton<ControllerSettings>.Instance.Deadzone = value;
        };
        curve.OnValueChanged += (_, value) =>
        {
            Singleton<ControllerSettings>.Instance.Curve = value;
            plot.Update();
            //Mathf.Ease((0f - 1f), value);
        };
    }
}
