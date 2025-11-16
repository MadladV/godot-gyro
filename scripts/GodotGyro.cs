using System;
using Godot;
using GyroHelpers;
using SDL3;

namespace GodotGyro;

/// <summary>
/// Class meant to provide simple and easy to use abstractions for SDL and GyroHelpers
/// Instance of this class calls <see cref="SDL.Init">SDL.Init(InitFlags.Gamepad)</see> by default
/// </summary>
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
    public bool TouchpadHoldState { 
        get => touchpadHoldState;
        private set
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
    /// <summary>
    /// Updates the Gyro and Accelerator sensor values. This should be done every frame.
    /// Consumes <see cref="SDL.PollEvent"/> and calls <see cref="GyroInput.Begin"/> 
    /// </summary>
    public void Update()
    {
        GyroInput.Begin();
        // NOTE: Things go awry when more than a single SDL.PollEvent() instance is processed,
        // could be worth moving it to another dedicated class and call related functions from within it
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
    /// <summary>
    /// Evaluates GyroSettings (<see cref="GyroSettings.ActivationMode">Activation Mode</see> and <see cref="GyroSettings.Ratchet">Ratchet</see>) to determine whether the Gyro input is paused or not.
    /// </summary>
    /// <returns>True if the Gyroscope input is paused.</returns>
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

    /// <summary>
    /// Processes Gyro sensor data and applies user settings (Sensitivity, Tightening)
    /// </summary>
    /// <param name="delta">Delta Time</param>
    /// <returns>Rotation delta in Euler angles (radians)</returns>
    public Vector3 ProcessGyro(float delta)
    {
        System.Numerics.Vector2 angleDelta = GyroProcessor.Update(GyroInput.Gyro, delta);
        angleDelta = angleDelta with { Y = angleDelta.Y * GyroSettings.VerticalSensMul };
        angleDelta *= GyroSettings.Sensitivity;
        
        return (new Quaternion(Vector3.Up, angleDelta.Y) * new Quaternion(Vector3.Right, angleDelta.X)).GetEuler();
    }
    
    /// <summary>
    /// Parses data returned by Gamepad Sensor to System.Numerics.Vector3
    /// </summary>
    /// <param name="event">SDL.EventType.GamepadSensorUpdate</param>
    /// <returns>Sensor data (Accel, Gyro) as Vector3</returns>
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
