using Godot;

public partial class SettingsInterface : Control
{
    [Export] private Button stickTab, gyroTab, videoTab;
    [Export] private Control stickSettings, gyroSettings, videoSettings;
    [Export] private VBoxContainer settingsContainer;
    [Export] private TextureRect escRect, escOutlineRect;
    
    
    public GyroSettings GyroSettings = new();

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
}
