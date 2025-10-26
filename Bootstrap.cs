using Godot;
using GyroHelpers;

namespace GodotGyro;

public partial class Bootstrap : Node
{
    public override void _Ready()
    {
        Singleton<FlickStick>.Instance = new FlickStick();
        Singleton<GyroSettings>.Instance = new GyroSettings();
        Singleton<ControllerSettings>.Instance = new ControllerSettings();
    }
}

public static class Singleton<T>
{
    public static T Instance;
}