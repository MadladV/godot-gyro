using Godot;
using GyroHelpers;

namespace GodotGyro.scripts.ui;
public partial class SettingsInterface : Control
{
    [Export] private Button stickTab, gyroTab, videoTab, calibrationButton;
    [Export] private Control stickSettings, gyroSettings, videoSettings;
    [Export] private VBoxContainer settingsContainer;
    [Export] private TextureRect escRect, escOutlineRect;
    [Export] private OptionButton connectedGamepadsDropdown;
    [Export] private Label calibrationLabel;
    
    public override void _Ready()
    {
        stickTab.Pressed += () =>
        {
            stickSettings.Visible = true;
            gyroSettings.Visible = false;
            videoSettings.Visible = false;
        };
        
        gyroTab.Pressed += () =>
        {
            stickSettings.Visible = false;
            gyroSettings.Visible = true;
            videoSettings.Visible = false;
        };
        
        videoTab.Pressed += () =>
        {
            stickSettings.Visible = false;
            gyroSettings.Visible = false;
            videoSettings.Visible = true;
        };

        calibrationButton.Pressed += CalibrateGyro;
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            settingsContainer.Visible = !settingsContainer.Visible;
            escRect.Visible = !settingsContainer.Visible;
            escOutlineRect.Visible = settingsContainer.Visible;
        }
    }

    private void CalibrateGyro()
    {
        calibrationLabel.Text = "Put the controller down on a stable surface and wait for the process to finish.";
        GetTree().CreateTimer(3f).Timeout += () =>
        {
            Singleton<GyroInput>.Instance.Calibrating = true;
            calibrationLabel.Text = "Calibrating...";
            GetTree().CreateTimer(5f).Timeout += () =>
            {
                Singleton<GyroInput>.Instance.Calibrating = false;
                calibrationLabel.Text = "Calibration finished!";
                GetTree().CreateTimer(3f).Timeout += () =>
                {
                    calibrationLabel.Text = "Please calibrate the controller if you are experiencing drift.";
                };
            };
        };
    }
}
