using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FirstRogue.Gfx;

public static class SpriteMesh
{
    public const int TileSize = 16;
    public const float UnitX = 1f / TileSize;
    public const float UnitY = 1f / TileSize;
    public const int PrimitiveCount = 2;

    public static VertexPositionColorTexture[] Mesh =
    {
        new(new Vector3(-0.5f, 0, 0), GameColors.Lighting6, new Vector2(0, UnitY)),
        new(new Vector3(0.5f, 1, 0), GameColors.Lighting6, new Vector2(UnitX, 0)),
        new(new Vector3(0.5f, 0, 0), GameColors.Lighting6, new Vector2(UnitX, UnitY)),
        new(new Vector3(-0.5f, 0, 0), GameColors.Lighting6, new Vector2(0, UnitY)),
        new(new Vector3(-0.5f, 1, 0), GameColors.Lighting6, new Vector2(0, 0)),
        new(new Vector3(0.5f, 1, 0), GameColors.Lighting6, new Vector2(UnitX, 0))
    };
}