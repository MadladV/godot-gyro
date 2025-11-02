using Godot;

namespace GodotGyro;
public partial class VideoSettingsMenu : PanelContainer
{
    [Export] private SettingSlider resolutionScale;
    [Export] private OptionButton windowResolution, displayMode;
    [Export] private Button applyButton;

    private Vector2I[] resolutions = [new (1280, 720), new (1920, 1080), new (2560, 1440), new (3840, 2160)];
    private float newResolutionScale;
    private Vector2I newResolution;
    private DisplayServer.WindowMode newDisplayMode;
    
    public override void _Ready()
    {
        resolutionScale.OnValueChanged += (_, value) =>
        {
            newResolutionScale = value;
        };
        windowResolution.ItemSelected += (value) =>
        {
            newResolution = resolutions[value];
        };
        displayMode.ItemSelected += (value) =>
        {
            newDisplayMode = (DisplayServer.WindowMode)value;
        };
        applyButton.Pressed += () =>
        {
            DisplayServer.WindowSetSize(newResolution);
            DisplayServer.WindowSetMode(newDisplayMode);
            GetViewport().Scaling3DScale = newResolutionScale;
        };
    }
}
