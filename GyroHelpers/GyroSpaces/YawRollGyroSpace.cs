using System.Numerics;
using System.Runtime.CompilerServices;

namespace GyroHelpers.GyroSpaces;

public class YawRollGyroSpace : IGyroSpace
{
    private float yawRollMix = 0.5f;

    public float YawRollMix
    {
        get => yawRollMix;
        set => yawRollMix = float.Clamp(value, 0f, 1f);
    }
    
    public YawRollGyroSpace(float yawRollMix)
    {
        YawRollMix = yawRollMix;
    }

    public YawRollGyroSpace()
    {
        
    }

    public Vector2 Transform(GyroState gyro)
    {
        var blendedAxis = float.Lerp(GetAxis(gyro.Gyro, GyroAxis.Yaw), GetAxis(gyro.Gyro, GyroAxis.Roll), yawRollMix);
        return new Vector2(gyro.Gyro.X, blendedAxis);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static float GetAxis(Vector3 vector, GyroAxis axis)
    {
#if NET7_0_OR_GREATER
        return vector[(int)axis];
#else
		switch (axis)
		{
			case GyroAxis.Pitch: return vector.X;
			case GyroAxis.Yaw: return vector.Y;
			case GyroAxis.Roll: return vector.Z;
			default: throw new ArgumentOutOfRangeException(nameof(axis));
		}
#endif
    }
}