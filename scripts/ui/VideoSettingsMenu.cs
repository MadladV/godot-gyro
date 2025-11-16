using Godot;

namespace GodotGyro;
public partial class VideoSettingsMenu : PanelContainer
{
    [Export] private SettingSlider resolutionScale;
    [Export] private OptionButton windowResolution, displayMode;
    [Export] private Button applyButton;
    [Export] private CanvasLayer buttonPromptCanvas;

    private readonly Vector2I[] resolutions = [new (1280, 720), new (1920, 1080), new (2560, 1440), new (3840, 2160)];
    private float newResolutionScale = 1.0f;
    private Vector2I newResolution = new (1280, 720);
    private DisplayServer.WindowMode newDisplayMode =  DisplayServer.WindowMode.Windowed;
    private Vector2I defaultResolution = new (2560, 1440);
    
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
            switch (value)
            {
                case 0:
                    newDisplayMode = DisplayServer.WindowMode.Windowed;
                    break;
                case 1:
                    newDisplayMode = DisplayServer.WindowMode.Fullscreen;
                    break;
            }
        };
        applyButton.Pressed += () =>
        {
            DisplayServer.WindowSetSize(newResolution);
            DisplayServer.WindowSetMode(newDisplayMode);
            GetViewport().Scaling3DScale = newResolutionScale;
        };

        GetViewport().SizeChanged += () =>
        {
            Vector2 scale = GetViewportRect().Size / defaultResolution;
            GD.Print(scale);
            buttonPromptCanvas.Transform = new Transform2D(
                new Vector2(scale.X, 0f), 
                new Vector2(0f, scale.X), 
                new Vector2(GetViewportRect().Size.X * (1f - scale.X), GetViewportRect().Size.Y * (1f - scale.Y))
            );
        };
    }
}
