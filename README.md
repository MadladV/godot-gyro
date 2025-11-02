# Motion Controls for Godot

Demo of Gyro Aim (Motion Controls) in Godot 4.5 based on SDL3 and work by Jibb Smart & NaokoAF.

# Example

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
