using System;
using Microsoft.Xna.Framework;

namespace FirstRogue.Gfx;

public class Sprite
{
    public Vector3 Pos { get; private set; }

    public Sprite(Vector3 pos)
    {
        Pos = pos;
    }
    
    public Matrix GetModelMatrix(Vector3 facingPos)
    {
        float angle = MathF.Atan2(facingPos.X - Pos.X, facingPos.Z - Pos.Z);

        Matrix model = Matrix.CreateScale(1f) * Matrix.CreateRotationY(MathHelper.WrapAngle(angle)) *
                       Matrix.CreateTranslation(Pos);
        
        return model;
    }
}