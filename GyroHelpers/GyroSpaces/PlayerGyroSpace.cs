using System.Numerics;

namespace GyroHelpers.GyroSpaces;

public class PlayerGyroSpace : IGyroSpace
{
    /// <summary>
    /// Player Space implementation from <see href="http://gyrowiki.jibbsmart.com/blog:player-space-gyro-and-alternatives-explained">GyroWiki</see>
    /// Uses local pitch.
    /// Combines yaw and roll to get rotation around whatever axis the player is actually using.
    /// </summary>
    public Vector2 Transform(GyroState gyro)
    {
        float worldYaw = gyro.Gyro.Y * gyro.Gravity.Y + gyro.Gyro.Z * gyro.Gravity.Z;
        float yawRelaxFactor = 1.41f;
        
        float yaw = float.Sign(worldYaw) * float.Min(float.Abs(worldYaw) * yawRelaxFactor, new Vector2(gyro.Gyro.Y, gyro.Gyro.Z).Length());
        float pitch = gyro.Gyro.X;
        
        return new Vector2(pitch, -yaw);
    }
}