using Godot;

public partial class ButtonState : TextureRect
{
    [Export] private Texture2D textureActive, textureInactive;
    public void SetButtonState(bool state)
    {
        switch (state)
        {
            case true:
                Texture = textureActive;
                break;
            case false:
                Texture = textureInactive;
                break;
        }
    }
}
