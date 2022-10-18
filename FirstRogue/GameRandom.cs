using System;
using Microsoft.Xna.Framework;

namespace FirstRogue;

public static class GameRandom
{
    public static float PseudoRandomFloat(int vx, int vy, int vz)
    {
        var smallPos = new Vector3(MathF.Sin(vx), MathF.Sin(vy), MathF.Sin(vz));

        float random = Vector3.Dot(smallPos, new Vector3(12.9898f, 78.233f, 37.719f));
        random = MathF.Sin(random) * 143758.5453f;
        random -= MathF.Floor(random);
        return random;
    }
    
    public static float GradientNoise(int x, int y, int z)
    {
        float total = PseudoRandomFloat(x, y, z);
        total += PseudoRandomFloat(x + 1, y, z);
        total += PseudoRandomFloat(x - 1, y, z);
        total += PseudoRandomFloat(x, y + 1, z);
        total += PseudoRandomFloat(x, y - 1, z);
        total += PseudoRandomFloat(x, y, z + 1);
        total += PseudoRandomFloat(x, y, z - 1);
        
        return total / 7f;
    }
}