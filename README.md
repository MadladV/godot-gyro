# Motion Controls for Godot

Demo of Gyro Aim (Motion Controls) in Godot 4.5 based on SDL3 and work by Jibb Smart & NaokoAF.

The main goal of this project is to serve as an example of well-implemented Motion Controls in videogames, including the basic functionality input handling via [Simple DirectMedia Layer](https://libsdl.org/), and the supporting suite of necessary settings (based on [Jibb Smart's GamepadMotionHelpers](https://github.com/JibbSmart/GamepadMotionHelpers)).

# Example

This project is meant to offer a simple and easy to use abstraction layer for SDL and GamepadMotionHelpers, allowing Godot developers to implement Gyro Aim with ease:

```csharp
    public override void _Process(double delta)
    {
        GodotGyro.Update();
        GodotGyro.IsAiming = Player.IsAiming();
        if (!GodotGyro.IsPaused())
        {
            Camera.Rotation += GodotGyro.ProcessGyro((float)delta);
        }
    }
```

# Referenced work

[Simple DirectMedia Layer](https://libsdl.org/)

[Jibb Smart](https://www.jibbsmart.com/)

[NaokoAF (Nao) · GitHub](https://github.com/NaokoAF)

[Kenney](https://kenney.nl/)
