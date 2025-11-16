using Godot;
using GyroHelpers;

namespace GodotGyro.scripts.ui;
public partial class SettingsInterface : Control
{
    [Export] private Button calibrationButton;
    [Export] private Control stickSettings, gyroSettings, videoSettings;
    [Export] private TabContainer tabContainer;
    [Export] private TextureRect escRect, escOutlineRect;
    [Export] private Label calibrationLabel;
    
    public override void _Ready()
    {
        calibrationButton.Pressed += CalibrateGyro;
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            tabContainer.Visible = !tabContainer.Visible;
            escRect.Visible = !tabContainer.Visible;
            escOutlineRect.Visible = tabContainer.Visible;
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
