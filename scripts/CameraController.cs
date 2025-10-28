using System;
using Godot;
using GyroHelpers;
using GyroHelpers.GyroSpaces;
using SDL3;

namespace GodotGyro;
public partial class CameraController : Camera3D
{
    public event EventHandler TouchpadStateChanged;
    [Export] private Button calibrationButton;
    [Export] private Label calibrationLabel;
    private readonly GyroInput gyroInput = new();
    private GyroProcessor GyroProcessor => Singleton<GyroProcessor>.Instance;

    private bool isTouchpadTouched;
    private bool IsTouchpadDown
    {
        get => isTouchpadTouched;
        set
        {
            if (isTouchpadTouched == value) return;
            isTouchpadTouched = value;
            if (IsTouchpadDown) TouchpadStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public override void _Ready()
    {
        SDL.Init(SDL.InitFlags.Gamepad);
        GyroProcessor.GyroSpace = new WorldGyroSpace();

        // TODO: Make this prettier.
        calibrationButton.Pressed += () =>
        {
            calibrationLabel.Text = "Put the controller down on a stable surface and wait for the process to finish.";
            GetTree().CreateTimer(3f).Timeout += () =>
            {
                gyroInput.Calibrating = true;
                calibrationLabel.Text = "Calibrating...";
                GetTree().CreateTimer(5f).Timeout += () =>
                {
                    gyroInput.Calibrating = false;
                    calibrationLabel.Text = "Calibration finished!";
                    GetTree().CreateTimer(3f).Timeout += () =>
                    {
                        calibrationLabel.Text = "Please calibrate the controller if you are experiencing drift.";
                    };
                };
            };
        };
        TouchpadStateChanged += (_, _) =>
        {
            if (Singleton<GyroSettings>.Instance.Ratchet == GyroRatchet.TOUCHPAD)
            {
                gyroToggleState = !gyroToggleState;
            }
        };
    }

    private float adsAlpha;

    private float AdsAlpha
    {
        get => adsAlpha;
        set => adsAlpha = Math.Clamp(value, 0f, 1f);
    }

    private float hipFov = 90f;
    private float aimFov = 60f;
    private bool gyroToggleState;
    private bool isGyroActive = true;
    public override void _Process(double delta)
    {
        gyroInput.Begin();
        while (SDL.PollEvent(out SDL.Event @event))
        {
            switch ((SDL.EventType)@event.Type)
            {
                case SDL.EventType.GamepadAdded:
                    // TODO: Support multiple devices
                    IntPtr gamepad = SDL.OpenGamepad(@event.GDevice.Which);
                    GD.Print($"Added gamepad: {SDL.GetGamepadName(gamepad)} : {gamepad}");
                    SDL.SetGamepadSensorEnabled(gamepad, SDL.SensorType.Gyro, true);
                    SDL.SetGamepadSensorEnabled(gamepad, SDL.SensorType.Accel, true);
                    break;
                case SDL.EventType.GamepadSensorUpdate:
                    ulong timestamp = @event.GSensor.Timestamp;
                    System.Numerics.Vector3 data = ParseGSensorData(@event.GSensor);
                    
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
        System.Numerics.Vector2 angleDelta = GyroProcessor.Update(gyroInput.Gyro, (float)delta);
        angleDelta = angleDelta with { Y = angleDelta.Y * Singleton<GyroSettings>.Instance.VerticalSensMul };
        angleDelta *= Singleton<GyroSettings>.Instance.Sensitivity;
        // TODO: Gyro precision logic
        
        // Gyro Activation + Ratchet modes
        if (Singleton<GyroSettings>.Instance.Ratchet == GyroRatchet.BUTTON && Input.IsActionJustPressed("pauseGyro"))
        {
            gyroToggleState = !gyroToggleState;
        }
        
        switch (Singleton<GyroSettings>.Instance.ActivationMode)
        {
            case GyroActivationMode.ALWAYS_ON:
                switch (Singleton<GyroSettings>.Instance.Ratchet)
                {
                    case GyroRatchet.TOUCHPAD:
                        isGyroActive = !IsTouchpadDown;
                        break;
                    case GyroRatchet.BUTTON:
                        isGyroActive = !Input.IsActionPressed("pauseGyro");
                        break;
                }
                break;
            case GyroActivationMode.ALWAYS_OFF:
                switch (Singleton<GyroSettings>.Instance.Ratchet)
                {
                    case GyroRatchet.TOUCHPAD:
                        isGyroActive = IsTouchpadDown;
                        break;
                    case GyroRatchet.BUTTON:
                        isGyroActive = Input.IsActionPressed("pauseGyro");
                        break;
                }
                break;
            case GyroActivationMode.WHEN_AIMING:
                isGyroActive = Input.IsActionPressed("aim");
                break;
            case GyroActivationMode.TOGGLE:
                isGyroActive = gyroToggleState;
                break;
        }
        
        if (isGyroActive)
        {
            // Gyro velocity in radians per second
            float gyroSpeed = angleDelta.Length() / (float)delta; 
            
            // threshold for inputs to be ignored in radians per second (default 0.36°/s)
            float minThreshold = Singleton<GyroSettings>.Instance.MinThreshold * Mathf.Pi / 180f; 
            
            // threshold for inputs to be slowed down in radians per second (default 0.75°/s)
            float precisionThreshold = Singleton<GyroSettings>.Instance.PrecisionThreshold * Mathf.Pi / 180f; 
            
            if (gyroSpeed < precisionThreshold)
            {
                // linear interpolation for gyro velocities between deadzone and precision threshold
                float precision = Mathf.Remap(gyroSpeed, minThreshold, precisionThreshold, 0f, 1f); 
                angleDelta *= precision;
            }
            Rotation += (new Quaternion(Vector3.Up, angleDelta.Y) *
                         new Quaternion(Vector3.Right, angleDelta.X)).GetEuler() * AimSensitivityMultiplier(Fov);
        }
        
        // TODO: Add an orientation gizmo
        
        // Stick camera controller
        Vector2 rightStick = new Vector2(Input.GetJoyAxis(0, JoyAxis.RightX), Input.GetJoyAxis(0, JoyAxis.RightY));
        
        if (rightStick.Length() >= Singleton<ControllerSettings>.Instance.Deadzone && !Singleton<GyroSettings>.Instance.FlickStickEnabled)
        {
            // Axial deadzone
            if (Math.Abs(rightStick.X) < Singleton<ControllerSettings>.Instance.AxialDeadzone ||
                Math.Abs(rightStick.Y) < Singleton<ControllerSettings>.Instance.AxialDeadzone) return;
            
            Vector2 direction = rightStick.Normalized();
            float length = rightStick.Length();
            length = Mathf.Remap(length, Singleton<ControllerSettings>.Instance.Deadzone, 1f, 0f, 1f);
            length *= Mathf.Pi / 180f * (float)delta; // Convert to degrees per second
            Vector2 output = direction * length;
            output *= Singleton<ControllerSettings>.Instance.TurnRate * new Vector2(1f, Singleton<ControllerSettings>.Instance.VerticalSensitivity); // Apply sensitivity
            
            // Power curve
            Rotation -= (new Quaternion(Vector3.Up, output.X) * new Quaternion(Vector3.Right, output.Y)).GetEuler() * AimSensitivityMultiplier(Fov);
        }
        
        // Flick Stick
        if (Singleton<GyroSettings>.Instance.FlickStickEnabled)
        {
            Rotation = Rotation with { Y = Rotation.Y + Singleton<FlickStick>.Instance.Update(new System.Numerics.Vector2(rightStick.X, rightStick.Y), (float)delta) };
        }

        AdsAlpha += (float)delta * (Input.IsActionPressed("aim") ? 3f : -3f);
        Fov = float.Lerp(90f, 60f, AdsAlpha);
    }

    private static unsafe System.Numerics.Vector3 ParseGSensorData(SDL.GamepadSensorEvent @event)
    {
        System.Numerics.Vector3 result = new System.Numerics.Vector3(
            @event.Data[0],
            @event.Data[1],
            @event.Data[2]
            );
        return result;
    }

    private float AimSensitivityMultiplier(float currentFov)
    {
        float hip = Mathf.DegToRad(hipFov * 0.5f);
        float current = Mathf.DegToRad(currentFov * 0.5f);

        return (float)Math.Pow(Mathf.Tan(current) / Mathf.Tan(hip), 1.78f); // Uniform Aiming for 16:9 aspect ratio
    }
}


