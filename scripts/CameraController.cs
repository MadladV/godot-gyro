using System;
using Godot;
using GyroHelpers;

namespace GodotGyro;

public partial class CameraController : Camera3D
{
    private ControllerSettings ControllerSettings => Singleton<ControllerSettings>.Instance;
    private FlickStick FlickStick => Singleton<FlickStick>.Instance;
    private GodotGyro GodotGyro => Singleton<GodotGyro>.Instance;
    private GyroSettings GyroSettings => Singleton<GyroSettings>.Instance;
    
    public override void _Process(double delta)
    {
        GodotGyro.Update();
        GodotGyro.IsAiming = AdsAlpha > 0f;
        if (!GodotGyro.IsPaused())
        {
            Rotation += GodotGyro.ProcessGyro((float)delta) * AimSensitivityMultiplier(Fov);
        }
        
        // TODO: Grab device ID from active controller?
        Vector2 rightStick = new (Input.GetJoyAxis(0, JoyAxis.RightX), Input.GetJoyAxis(0, JoyAxis.RightY));
        switch (GyroSettings.FlickStickEnabled)
        {
            case true:
                Rotation = Rotation with { Y = Rotation.Y + FlickStick.Update(rightStick, (float)delta) };
                break;
            case false:
                Rotation -= ProcessRightStick(rightStick, (float)delta) * AimSensitivityMultiplier(Fov);
                break;
        }

        Rotation = Rotation with { X = float.Clamp(Rotation.X, -1.5f, 1.5f) };
        ProcessZoom((float)delta);
    }
    
    private float adsAlpha;
    private float AdsAlpha
    {
        get => adsAlpha;
        set => adsAlpha = Math.Clamp(value, 0f, 1f);
    }
    private float hipFov = 90f;
    private float aimFov = 60f;
    private void ProcessZoom(float delta)
    {
        AdsAlpha += delta * (Input.IsActionPressed("aim") ? 3f : -3f);
        Fov = float.Lerp(90f, 60f, AdsAlpha);
    }
    /// <summary>
    /// Implementation of <see href="https://www.ea.com/en/games/battlefield/battlefield-2042/news/dev-notes-uniform-soldier-aiming">Uniform Aim</see> meant to maintain consistent input-to-monitor-distance ratio.
    /// </summary>
    /// <param name="currentFov">Field of View of currently used camera.</param>
    /// <returns>Aim sensitivity scalar</returns>
    private float AimSensitivityMultiplier(float currentFov)
    {
        float hip = Mathf.DegToRad(hipFov * 0.5f);
        float current = Mathf.DegToRad(currentFov * 0.5f);

        return (float)Math.Pow(Mathf.Tan(current) / Mathf.Tan(hip), 1.78f); // Uniform Aiming for 16:9 aspect ratio
    }
    /// <summary>
    /// Handles stick input, including Deadzones (<see cref="ControllerSettings.Deadzone">Circular</see> and <see cref="ControllerSettings.AxialDeadzone">Axial</see>), and <see cref="ControllerSettings.Curve">Power Curve</see>
    /// </summary>
    /// <param name="rightStick">Stick deflection as Vector2</param>
    /// <param name="delta">Delta Time</param>
    /// <returns>Camera rotation vector</returns>
    private Vector3 ProcessRightStick(Vector2 rightStick, float delta)
    {
        // Deadzone
        if (rightStick.Length() <= ControllerSettings.Deadzone)
        {
            return Vector3.Zero;
        }
        
        // Axial deadzone
        if (Math.Abs(rightStick.X) < ControllerSettings.AxialDeadzone || Math.Abs(rightStick.Y) < ControllerSettings.AxialDeadzone)
        {
            return Vector3.Zero;
        }
            
        Vector2 direction = rightStick.Normalized();
        float length = rightStick.Length();
        
        length = Mathf.Remap(length, ControllerSettings.Deadzone, 1f, 0f, 1f);
        length = Mathf.Ease(length, ControllerSettings.Curve); // Apply power curve
        length *= Mathf.Pi / 180f * delta; // Convert to degrees per second
        Vector2 output = direction * length; 
        output *= ControllerSettings.TurnRate * new Vector2(1f, ControllerSettings.VerticalSensitivity); // Apply sensitivity
        
        return (new Quaternion(Vector3.Up, output.X) * new Quaternion(Vector3.Right, output.Y)).GetEuler();
    }
}