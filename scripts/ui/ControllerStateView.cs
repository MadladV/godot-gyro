using Godot;

namespace GodotGyro;

public partial class ControllerStateView : Control
{
    [Export] private ButtonState triangleState, squareState, circleState, crossState, touchpadState;
    [Export] private Label gyroState;
    private GodotGyro GodotGyro => Singleton<GodotGyro>.Instance;
    
    public override void _Process(double delta)
    {
        touchpadState.SetButtonState(GodotGyro.TouchpadHoldState);
        triangleState.SetButtonState(Input.IsJoyButtonPressed(0, JoyButton.Y));
        squareState.SetButtonState(Input.IsJoyButtonPressed(0, JoyButton.X));
        circleState.SetButtonState(Input.IsJoyButtonPressed(0, JoyButton.B));
        crossState.SetButtonState(Input.IsJoyButtonPressed(0, JoyButton.A));
        gyroState.Text = GodotGyro.IsPaused() ? "Paused" : "Active";
    }
}
