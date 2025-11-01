using Godot;
using GyroHelpers;

namespace GodotGyro;

public partial class Bootstrap : Node
{
    public override void _Ready()
    {
        Singleton<ControllerSettings>.Instance = new ControllerSettings();
        Singleton<FlickStick>.Instance = new FlickStick();
        Singleton<GodotGyro>.Instance = new GodotGyro();
        Singleton<GyroInput>.Instance = new GyroInput();
        Singleton<GyroProcessor>.Instance = new GyroProcessor();
        Singleton<GyroSettings>.Instance = new GyroSettings();
    }
}

public static class Singleton<T>
{
    public static T Instance;
}