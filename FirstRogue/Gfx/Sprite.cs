using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FirstRogue.Gfx;

public class Sprite
{
    public readonly float Scale;
    public readonly VertexBuffer VertexBuffer;

    public Sprite(GraphicsDevice graphicsDevice, Vector3 pos, int texX, int texY, int texW, int texH, float scale)
    {
        Pos = pos;
        Scale = scale;

        VertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColorTexture), 6, BufferUsage.WriteOnly);

        var vertexData = new VertexPositionColorTexture[SpriteMesh.Mesh.Length];

        for (var i = 0; i < SpriteMesh.Mesh.Length; i++)
        {
            vertexData[i] = SpriteMesh.Mesh[i];
            vertexData[i].TextureCoordinate.X *= texW;
            vertexData[i].TextureCoordinate.Y *= texH;
            vertexData[i].TextureCoordinate.X += SpriteMesh.UnitX * texX;
            vertexData[i].TextureCoordinate.Y += SpriteMesh.UnitY * texY;
        }

        VertexBuffer.SetData(vertexData);
    }

    public Vector3 Pos { get; }

    public Matrix GetModelMatrix(Vector3 facingPos)
    {
        float angle = MathF.Atan2(facingPos.X - Pos.X, facingPos.Z - Pos.Z);

        Matrix model = Matrix.CreateScale(Scale) * Matrix.CreateRotationY(MathHelper.WrapAngle(angle)) *
                       Matrix.CreateTranslation(Pos);

        return model;
    }
}