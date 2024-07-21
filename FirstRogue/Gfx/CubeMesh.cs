using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FirstRogue.Gfx;

public static class CubeMesh
{
    public const int TileSize = 16;
    public const int TileSizeShift = 4;
    public const float UnitX = 1f / TileSize;
    public const float UnitY = 1f / TileSize;

    public static readonly VertexPositionColorTexture[][] Vertices =
    {
        new[]
        {
            new VertexPositionColorTexture(new Vector3(0, 0, 0), GameColors.Lighting5, new Vector2(UnitX, UnitY)),
            new VertexPositionColorTexture(new Vector3(0, 1, 0), GameColors.Lighting5, new Vector2(UnitX, 0)),
            new VertexPositionColorTexture(new Vector3(1, 1, 0), GameColors.Lighting5, new Vector2(0, 0)),
            new VertexPositionColorTexture(new Vector3(1, 0, 0), GameColors.Lighting5, new Vector2(0, UnitY))
        },
        new[]
        {
            new VertexPositionColorTexture(new Vector3(0, 0, 1), GameColors.Lighting2, new Vector2(0, UnitY * 2)),
            new VertexPositionColorTexture(new Vector3(0, 1, 1), GameColors.Lighting2, new Vector2(0, UnitY)),
            new VertexPositionColorTexture(new Vector3(1, 1, 1), GameColors.Lighting2, new Vector2(UnitX, UnitY)),
            new VertexPositionColorTexture(new Vector3(1, 0, 1), GameColors.Lighting2,
                new Vector2(UnitX, UnitY * 2))
        },
        new[]
        {
            new VertexPositionColorTexture(new Vector3(1, 0, 0), GameColors.Lighting4,
                new Vector2(UnitX * 3, UnitY * 2)),
            new VertexPositionColorTexture(new Vector3(1, 0, 1), GameColors.Lighting4,
                new Vector2(UnitX * 2, UnitY * 2)),
            new VertexPositionColorTexture(new Vector3(1, 1, 1), GameColors.Lighting4,
                new Vector2(UnitX * 2, UnitY)),
            new VertexPositionColorTexture(new Vector3(1, 1, 0), GameColors.Lighting4,
                new Vector2(UnitX * 3, UnitY))
        },
        new[]
        {
            new VertexPositionColorTexture(new Vector3(0, 0, 0), GameColors.Lighting3,
                new Vector2(UnitX * 2, UnitY)),
            new VertexPositionColorTexture(new Vector3(0, 0, 1), GameColors.Lighting3,
                new Vector2(UnitX * 3, UnitY)),
            new VertexPositionColorTexture(new Vector3(0, 1, 1), GameColors.Lighting3, new Vector2(UnitX * 3, 0)),
            new VertexPositionColorTexture(new Vector3(0, 1, 0), GameColors.Lighting3, new Vector2(UnitX * 2, 0))
        },
        new[]
        {
            new VertexPositionColorTexture(new Vector3(0, 1, 0), GameColors.Lighting6, new Vector2(UnitX, UnitY)),
            new VertexPositionColorTexture(new Vector3(0, 1, 1), GameColors.Lighting6, new Vector2(UnitX, 0)),
            new VertexPositionColorTexture(new Vector3(1, 1, 1), GameColors.Lighting6, new Vector2(UnitX * 2, 0)),
            new VertexPositionColorTexture(new Vector3(1, 1, 0), GameColors.Lighting6,
                new Vector2(UnitX * 2, UnitY))
        },
        new[]
        {
            new VertexPositionColorTexture(new Vector3(0, 0, 0), GameColors.Lighting1,
                new Vector2(UnitX, UnitY * 2)),
            new VertexPositionColorTexture(new Vector3(0, 0, 1), GameColors.Lighting1, new Vector2(UnitX, UnitY)),
            new VertexPositionColorTexture(new Vector3(1, 0, 1), GameColors.Lighting1,
                new Vector2(UnitX * 2, UnitY)),
            new VertexPositionColorTexture(new Vector3(1, 0, 0), GameColors.Lighting1,
                new Vector2(UnitX * 2, UnitY * 2))
        }
    };

    public static readonly uint[][] Indices = {
        new uint[] { 0, 2, 1, 0, 3, 2 },
        new uint[] { 0, 1, 2, 0, 2, 3 },
        new uint[] { 0, 1, 2, 0, 2, 3 },
        new uint[] { 0, 2, 1, 0, 3, 2 },
        new uint[] { 0, 2, 1, 0, 3, 2 },
        new uint[] { 0, 1, 2, 0, 2, 3 }
    };
}