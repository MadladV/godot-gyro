using System;
using Godot;

public partial class SettingSlider : Control
{
    public event EventHandler<float> OnValueChanged;
    [Export] private HSlider hslider;
    [Export] private Label label;
    [Export(PropertyHint.Enum, " ,x,%,°/s,°,s,ms")] private string suffix;
    [Export(PropertyHint.Enum, "0:0,0.0:1,0.00:2")] private int precision = 1;
    public override void _Ready()
    {
        hslider.ValueChanged += (value) =>
        {
            OnValueChanged.Invoke(this, (float)value);
            switch (precision)
            {
                case 0:
                    label.Text = $"{value:F0}"+suffix;
                    break;
                case 1:
                    label.Text = $"{value:F1}"+suffix;
                    break;
                case 2:
                    label.Text = $"{value:F2}"+suffix;
                    break;
            }
        };
    }
}