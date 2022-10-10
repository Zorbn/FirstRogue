using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FirstRogue;

public static class CubeMesh
{
    public const int TileSize = 16;
    public const float UnitX = 1f / TileSize;
    public const float UnitY = 1f / TileSize;
    
    public static readonly Dictionary<Directions, VertexPositionTexture[]> Vertices = new()
    {
        {
            Directions.Forward,
            new [] {
                new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(UnitX, UnitY)),
                new VertexPositionTexture(new Vector3(1, 0, 0), new Vector2(0, UnitY)),
                new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(UnitX, UnitY)),
                new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(0, 1, 0), new Vector2(UnitX, 0))
            }
        },
        {
            Directions.Backward,
            new [] {
                new VertexPositionTexture(new Vector3(0, 0, 1), new Vector2(0, UnitY * 2)),
                new VertexPositionTexture(new Vector3(1, 1, 1), new Vector2(UnitX, UnitY)),
                new VertexPositionTexture(new Vector3(1, 0, 1), new Vector2(UnitX, UnitY * 2)),
                new VertexPositionTexture(new Vector3(0, 0, 1), new Vector2(0, UnitY * 2)),
                new VertexPositionTexture(new Vector3(0, 1, 1), new Vector2(0, UnitY)),
                new VertexPositionTexture(new Vector3(1, 1, 1), new Vector2(UnitX, UnitY))
            }
        },
        {
            Directions.Right,
            new [] {
                new VertexPositionTexture(new Vector3(1, 0, 0), new Vector2(UnitX * 3, UnitY * 2)),
                new VertexPositionTexture(new Vector3(1, 0, 1), new Vector2(UnitX * 2, UnitY * 2)),
                new VertexPositionTexture(new Vector3(1, 1, 1), new Vector2(UnitX * 2, UnitY)),
                new VertexPositionTexture(new Vector3(1, 0, 0), new Vector2(UnitX * 3, UnitY * 2)),
                new VertexPositionTexture(new Vector3(1, 1, 1), new Vector2(UnitX * 2, UnitY)),
                new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(UnitX * 3, UnitY))
            }
        },
        {
            Directions.Left,
            new [] {
                new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(UnitX * 2, UnitY)),
                new VertexPositionTexture(new Vector3(0, 1, 1), new Vector2(UnitX * 3, 0)),
                new VertexPositionTexture(new Vector3(0, 0, 1), new Vector2(UnitX * 3, UnitY)),
                new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(UnitX * 2, UnitY)),
                new VertexPositionTexture(new Vector3(0, 1, 0), new Vector2(UnitX * 2, 0)),
                new VertexPositionTexture(new Vector3(0, 1, 1), new Vector2(UnitX * 3, 0))
            }
        },
        {
            Directions.Up,
            new [] {
                new VertexPositionTexture(new Vector3(0, 1, 0), new Vector2(UnitX, UnitY)),
                new VertexPositionTexture(new Vector3(1, 1, 1), new Vector2(UnitX * 2, 0)),
                new VertexPositionTexture(new Vector3(0, 1, 1), new Vector2(UnitX, 0)),
                new VertexPositionTexture(new Vector3(0, 1, 0), new Vector2(UnitX, UnitY)),
                new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(UnitX * 2, UnitY)),
                new VertexPositionTexture(new Vector3(1, 1, 1), new Vector2(UnitX * 2, 0))
            }
        },
        {
            Directions.Down,
            new [] {
                new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(UnitX, UnitY * 2)),
                new VertexPositionTexture(new Vector3(0, 0, 1), new Vector2(UnitX, UnitY)),
                new VertexPositionTexture(new Vector3(1, 0, 1), new Vector2(UnitX * 2, UnitY)),
                new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(UnitX, UnitY * 2)),
                new VertexPositionTexture(new Vector3(1, 0, 1), new Vector2(UnitX * 2, UnitY)),
                new VertexPositionTexture(new Vector3(1, 0, 0), new Vector2(UnitX * 2, UnitY * 2))
            }
        }
    };

    public static Vector2 GetTexCoord(Voxels voxel)
    {
        int i = (int)voxel * 4;
        return new Vector2(UnitX * (i % TileSize), UnitY * (i / TileSize * 2));
    }
}