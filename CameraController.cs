using System;
using Godot;
using GodotGyro;
using GyroHelpers;
using GyroHelpers.GyroSpaces;
using SDL3;
public partial class CameraController : Camera3D
{
    [Export] private Button calibrationButton;
    [Export] private Label calibrationLabel;
    private readonly GyroInput gyroInput = new();
    private readonly GyroProcessor gyroProcessor = new();

    private bool IsTouchpadDown { get; set; } = false;

    public override void _Ready()
    {
        SDL.Init(SDL.InitFlags.Gamepad);
        gyroProcessor.GyroSpace = new WorldGyroSpace();

        // TODO: Make this prettier.
        calibrationButton.Pressed += () =>
        {
            calibrationLabel.Text = "Put the controller down on a stable surface and wait for the process to finish.";
            GetTree().CreateTimer(3f).Timeout += () =>
            {
                gyroInput.Calibrating = true;
                calibrationLabel.Text = "Calibrating...";
            };
            
            GetTree().CreateTimer(5f).Timeout += () =>
            {
                gyroInput.Calibrating = false;
                calibrationLabel.Text = "Calibration finished!";
            };
            
            GetTree().CreateTimer(3f).Timeout += () =>
            {
                calibrationLabel.Text = "Please calibrate the controller if you are experiencing drift.";
            };
        };
    }
    
    public override void _Process(double delta)
    {
        gyroInput.Begin();
        while (SDL.PollEvent(out var @event))
        {
            switch ((SDL.EventType)@event.Type)
            {
                case SDL.EventType.GamepadAdded:
                    // TODO: Support multiple devices
                    var gamepad = SDL.OpenGamepad(@event.GDevice.Which);
                    GD.Print($"Added gamepad: {SDL.GetGamepadName(gamepad)} : {gamepad}");
                    SDL.SetGamepadSensorEnabled(gamepad, SDL.SensorType.Gyro, true);
                    SDL.SetGamepadSensorEnabled(gamepad, SDL.SensorType.Accel, true);
                    break;
                case SDL.EventType.GamepadSensorUpdate:
                    var timestamp = @event.GSensor.Timestamp;
                    var data = ParseGSensorData(@event.GSensor);
                    
                    switch ((SDL.SensorType)@event.GSensor.Sensor)
                    {
                        case SDL.SensorType.Accel:
                            gyroInput.AddAccelerometerSample(data, timestamp);
                            break;
                        case SDL.SensorType.Gyro:
                            gyroInput.AddGyroSample(data, timestamp);
                            break;
                    }
                    break;
                case SDL.EventType.GamepadTouchpadDown:
                    IsTouchpadDown = true;
                    break;
                case SDL.EventType.GamepadTouchpadUp:
                    IsTouchpadDown = false;
                    break;
            }
        }
        
        // Gyro input
        var angleDelta = gyroProcessor.Update(gyroInput.Gyro, (float)delta);
        angleDelta = angleDelta with { Y = angleDelta.Y * Singleton<GyroSettings>.Instance.VerticalSensMul };
        angleDelta *= Singleton<GyroSettings>.Instance.Sensitivity;
        // TODO: Gyro precision logic
        
        if (!IsTouchpadDown)
        {
            // Gyro velocity in radians per second
            var gyroSpeed = angleDelta.Length() / (float)delta; 
            
            // threshold for inputs to be ignored in radians per second (default 0.36°/s)
            var minThreshold = Singleton<GyroSettings>.Instance.MinThreshold * Mathf.Pi / 180f; 
            
            // threshold for inputs to be slowed down in radians per second (default 0.75°/s)
            var precisionThreshold = Singleton<GyroSettings>.Instance.PrecisionThreshold * Mathf.Pi / 180f; 
            
            if (gyroSpeed < minThreshold) return; 
            if (gyroSpeed < precisionThreshold)
            {
                // linear interpolation for gyro velocities between deadzone and precision threshold
                var precision = Mathf.Remap(gyroSpeed, minThreshold, precisionThreshold, 0f, 1f); 
                angleDelta *= precision;
            }
            Rotation += (new Quaternion(Vector3.Up, angleDelta.Y) *
                         new Quaternion(Vector3.Right, angleDelta.X)).GetEuler();
        }
        
        // TODO: Add an orientation gizmo
        
        // Stick camera controller
        var rightStick = new Vector2(Input.GetJoyAxis(0, JoyAxis.RightX), Input.GetJoyAxis(0, JoyAxis.RightY));
        
        if (rightStick.Length() >= Singleton<ControllerSettings>.Instance.Deadzone && !Singleton<GyroSettings>.Instance.FlickstickEnabled)
        {
            // Axial deadzone
            if (Math.Abs(rightStick.X) < Singleton<ControllerSettings>.Instance.AxialDeadzone ||
                Math.Abs(rightStick.Y) < Singleton<ControllerSettings>.Instance.AxialDeadzone) return;
            
            var direction = rightStick.Normalized();
            var length = rightStick.Length();
            length = Mathf.Remap(length, Singleton<ControllerSettings>.Instance.Deadzone, 1f, 0f, 1f);
            length *= Mathf.Pi / 180f * (float)delta; // Convert to degrees per second
            var output = direction * length;
            output *= Singleton<ControllerSettings>.Instance.Turnrate * new Vector2(1f, Singleton<ControllerSettings>.Instance.VertSensitivity); // Apply sensitivity
            
            // Power curve
            Rotation -= (new Quaternion(Vector3.Up, output.X) * new Quaternion(Vector3.Right, output.Y)).GetEuler();
        }
        
        // Flick Stick
        if (Singleton<GyroSettings>.Instance.FlickstickEnabled)
        {
            Rotation = Rotation with { Y = Rotation.Y + Singleton<FlickStick>.Instance.Update(new System.Numerics.Vector2(rightStick.X, rightStick.Y), (float)delta) };
        }
    }

    private static unsafe System.Numerics.Vector3 ParseGSensorData(SDL.GamepadSensorEvent @event)
    {
        var result = new System.Numerics.Vector3(
            @event.Data[0],
            @event.Data[1],
            @event.Data[2]
            );
        return result;
    }
}


