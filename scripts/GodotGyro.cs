using System;
using Godot;
using GyroHelpers;
using SDL3;

namespace GodotGyro;

public class GodotGyro
{
    private GyroInput GyroInput => Singleton<GyroInput>.Instance;
    private GyroProcessor GyroProcessor => Singleton<GyroProcessor>.Instance;
    private GyroSettings GyroSettings => Singleton<GyroSettings>.Instance;
    public GodotGyro()
    {
        SDL.Init(SDL.InitFlags.Gamepad);
    }

    private bool touchpadToggleState = false;
    private bool touchpadHoldState = false;
    private bool TouchpadHoldState { 
        get => touchpadHoldState;
        set
        {
            if (touchpadHoldState == value) return;
            touchpadHoldState = value;
            if (value)
            {
                touchpadToggleState = !touchpadToggleState;
            }
        }
    }
    private bool buttonToggleState = false;
    public void Update()
    {
        GyroInput.Begin();
        while (SDL.PollEvent(out SDL.Event @event))
        {
            switch ((SDL.EventType)@event.Type)
            {
                case SDL.EventType.GamepadAdded:
                    IntPtr gamepadPtr = SDL.OpenGamepad(@event.GDevice.Which);
                    GD.Print($"Added gamepad: {SDL.GetGamepadName(gamepadPtr)}");
                    SDL.SetGamepadSensorEnabled(gamepadPtr, SDL.SensorType.Gyro, true);
                    SDL.SetGamepadSensorEnabled(gamepadPtr, SDL.SensorType.Accel, true);
                    break;
                case SDL.EventType.GamepadRemoved:
                    break;
                case SDL.EventType.GamepadSensorUpdate:
                    // TODO: Only process gyro for the currently active gamepad
                    ulong timestamp = @event.GSensor.Timestamp;
                    System.Numerics.Vector3 data = ParseGSensorData(@event.GSensor);
                    switch ((SDL.SensorType)@event.GSensor.Sensor)
                    {
                        case SDL.SensorType.Accel:
                            GyroInput.AddAccelerometerSample(data, timestamp);
                            break;
                        case SDL.SensorType.Gyro:
                            GyroInput.AddGyroSample(data, timestamp);
                            break;
                    }
                    break;
                case SDL.EventType.GamepadTouchpadDown:
                    TouchpadHoldState = true;
                    break;
                case SDL.EventType.GamepadTouchpadUp:
                    TouchpadHoldState = false;
                    break;
            }
        }
    }
    
    public bool IsAiming;
    public bool IsPaused()
    {
        if (Input.IsActionJustPressed("pauseGyro"))
        {
            buttonToggleState = !buttonToggleState;
        }
        
        switch (GyroSettings.ActivationMode)
        {
            case GyroActivationMode.ALWAYS_ON:
                switch (GyroSettings.Ratchet)
                {
                    case GyroRatchet.BUTTON:
                        return Input.IsActionPressed("pauseGyro");
                    case GyroRatchet.TOUCHPAD:
                        return TouchpadHoldState;
                }
                break;
            case GyroActivationMode.ALWAYS_OFF:
                switch (GyroSettings.Ratchet)
                {
                    case GyroRatchet.BUTTON:
                        return !Input.IsActionPressed("pauseGyro");
                    case GyroRatchet.TOUCHPAD:
                        return !TouchpadHoldState;
                }
                break;
            case GyroActivationMode.WHEN_AIMING:
                return !IsAiming;
            case GyroActivationMode.TOGGLE:
                switch (GyroSettings.Ratchet)
                {
                    case GyroRatchet.BUTTON:
                        return buttonToggleState;
                    case GyroRatchet.TOUCHPAD:
                        return touchpadToggleState;
                }
                break;
        }
        return false;
    }

    public Vector3 ProcessGyro(float delta)
    {
        System.Numerics.Vector2 angleDelta = GyroProcessor.Update(GyroInput.Gyro, delta);
        angleDelta = angleDelta with { Y = angleDelta.Y * GyroSettings.VerticalSensMul };
        angleDelta *= GyroSettings.Sensitivity;
        
        // Gyro velocity in radians per second
        float gyroSpeed = angleDelta.Length() / delta; 
            
        // Threshold for inputs to be ignored in radians per second (default 0.36°/s)
        // Motion below this threshold is discarded by GyroProcessor.Tightening
        float minThreshold = GyroSettings.MinThreshold * Mathf.Pi / 180f; 
        
        // threshold for inputs to be slowed down in radians per second (default 0.75°/s)
        float precisionThreshold = GyroSettings.PrecisionThreshold * Mathf.Pi / 180f; 
            
        if (gyroSpeed < precisionThreshold)
        {
            // linear interpolation for gyro velocities between deadzone and precision threshold
            float precision = Mathf.Remap(gyroSpeed, minThreshold, precisionThreshold, 0f, 1f); 
            angleDelta *= precision;
        }
        
        return (new Quaternion(Vector3.Up, angleDelta.Y) * new Quaternion(Vector3.Right, angleDelta.X)).GetEuler();
    }
    
    private static unsafe System.Numerics.Vector3 ParseGSensorData(SDL.GamepadSensorEvent @event)
    {
        System.Numerics.Vector3 result = new (
            @event.Data[0],
            @event.Data[1],
            @event.Data[2]
        );
        return result;
    }
}
