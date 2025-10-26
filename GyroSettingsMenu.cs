using Godot;
using GodotGyro;
using System;

public partial class GyroSettingsMenu : PanelContainer
{
    [ExportGroup("Motion controls")]
    [Export] private SettingSlider sensitivity, vertSensitivity, yawRollMix, minThreshold, precisionThreshold;
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
            Singleton<GyroSettings>.Instance.VerticalSensMul = value * 0.01f;
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
            Singleton<GyroSettings>.Instance.Orientation = (GyroOrientation)value;
        };
        yawRollMix.OnValueChanged += (_, value) =>
        {
            Singleton<GyroSettings>.Instance.YawRollMix = value;
        };
        minThreshold.OnValueChanged += (_, value) =>
        {
            Singleton<GyroSettings>.Instance.MinThreshold = value;
        };
        precisionThreshold.OnValueChanged += (_, value) =>
        {
            Singleton<GyroSettings>.Instance.PrecisionThreshold = value;
        };
        
        // Flick stick settings
        flickStickEnabled.Toggled += (value) =>
        {
            Singleton<GyroSettings>.Instance.FlickstickEnabled = value;
            flickStickSettingsUI.Visible = value;
        };
        flickThreshold.OnValueChanged += (_, value) => { };
        flickTime.OnValueChanged += (_, value) => { };
        snapping.ItemSelected += (value) => {
        };
    }
}
