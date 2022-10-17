using System;
using Microsoft.Xna.Framework;

namespace FirstRogue;

public static class VectorExtensions
{
    public static float GetComponent(this Vector3 vector, int i)
    {
        return i switch
        {
            0 => vector.X,
            1 => vector.Y,
            2 => vector.Z,
            _ => throw new ArgumentOutOfRangeException(nameof(vector), vector, null)
        };
    }

    public static void SetComponent(this ref Vector3 vector, int i, float val)
    {
        switch (i)
        {
            case 0:
                vector.X = val;
                break;
            case 1:
                vector.Y = val;
                break;
            case 2:
                vector.Z = val;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(vector), vector, null);
        }
    }
}