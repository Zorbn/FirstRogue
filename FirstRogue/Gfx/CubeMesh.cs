using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FirstRogue.Gfx;

public static class CubeMesh
{
    public const int TileSize = 16;
    public const float UnitX = 1f / TileSize;
    public const float UnitY = 1f / TileSize;

    public static readonly Dictionary<Directions, VertexPositionColorTexture[]> Vertices = new()
    {
        {
            Directions.Forward,
            new[]
            {
                new VertexPositionColorTexture(new Vector3(0, 0, 0), GameColors.Lighting5, new Vector2(UnitX, UnitY)),
                new VertexPositionColorTexture(new Vector3(0, 1, 0), GameColors.Lighting5, new Vector2(UnitX, 0)),
                new VertexPositionColorTexture(new Vector3(1, 1, 0), GameColors.Lighting5, new Vector2(0, 0)),
                new VertexPositionColorTexture(new Vector3(1, 0, 0), GameColors.Lighting5, new Vector2(0, UnitY))
            }
        },
        {
            Directions.Backward,
            new[]
            {
                new VertexPositionColorTexture(new Vector3(0, 0, 1), GameColors.Lighting2, new Vector2(0, UnitY * 2)),
                new VertexPositionColorTexture(new Vector3(0, 1, 1), GameColors.Lighting2, new Vector2(0, UnitY)),
                new VertexPositionColorTexture(new Vector3(1, 1, 1), GameColors.Lighting2, new Vector2(UnitX, UnitY)),
                new VertexPositionColorTexture(new Vector3(1, 0, 1), GameColors.Lighting2,
                    new Vector2(UnitX, UnitY * 2))
            }
        },
        {
            Directions.Right,
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
            }
        },
        {
            Directions.Left,
            new[]
            {
                new VertexPositionColorTexture(new Vector3(0, 0, 0), GameColors.Lighting3,
                    new Vector2(UnitX * 2, UnitY)),
                new VertexPositionColorTexture(new Vector3(0, 0, 1), GameColors.Lighting3,
                    new Vector2(UnitX * 3, UnitY)),
                new VertexPositionColorTexture(new Vector3(0, 1, 1), GameColors.Lighting3, new Vector2(UnitX * 3, 0)),
                new VertexPositionColorTexture(new Vector3(0, 1, 0), GameColors.Lighting3, new Vector2(UnitX * 2, 0)),
            }
        },
        {
            Directions.Up,
            new[]
            {
                new VertexPositionColorTexture(new Vector3(0, 1, 0), GameColors.Lighting6, new Vector2(UnitX, UnitY)),
                new VertexPositionColorTexture(new Vector3(0, 1, 1), GameColors.Lighting6, new Vector2(UnitX, 0)),
                new VertexPositionColorTexture(new Vector3(1, 1, 1), GameColors.Lighting6, new Vector2(UnitX * 2, 0)),
                new VertexPositionColorTexture(new Vector3(1, 1, 0), GameColors.Lighting6,
                    new Vector2(UnitX * 2, UnitY)),
            }
        },
        {
            Directions.Down,
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
        }
    };
    
    public static readonly Dictionary<Directions, uint[]> Indices = new()
    {
        {
            Directions.Forward,
            new uint[] { 0, 2, 1, 0, 3, 2 }
        },
        {
            Directions.Backward,
            new uint[] { 0, 1, 2, 0, 2, 3 }
        },
        {
            Directions.Right,
            new uint[] { 0, 1, 2, 0, 2, 3 }
        },
        {
            Directions.Left,
            new uint[] { 0, 2, 1, 0, 3, 2 }
        },
        {
            Directions.Up,
            new uint[] { 0, 2, 1, 0, 3, 2 }
        },
        {
            Directions.Down,
            new uint[] { 0, 1, 2, 0, 2, 3 }
        }
    };

    public static Vector2 GetTexCoord(Voxels voxel)
    {
        int i = (int)voxel * 4;
        return new Vector2(UnitX * (i % TileSize), UnitY * (i / TileSize * 2));
    }
}