using FirstRogue.Gfx;
using Microsoft.Xna.Framework.Graphics;

namespace FirstRogue;

public class DrawableWorld
{
    private readonly DrawableVoxelChunk[] chunks;
    public readonly World World;

    public DrawableWorld(GraphicsDevice graphicsDevice, int xChunks, int yChunks, int zChunks, int chunkWidth,
        int chunkHeight, int chunkDepth)
    {
        World = new World(xChunks, yChunks, zChunks, chunkWidth, chunkHeight, chunkDepth);
        chunks = new DrawableVoxelChunk[xChunks * yChunks * zChunks];

        for (var x = 0; x < xChunks; x++)
        for (var y = 0; y < yChunks; y++)
        for (var z = 0; z < zChunks; z++)
            chunks[x + y * xChunks + z * xChunks * yChunks] =
                new DrawableVoxelChunk(graphicsDevice, chunkWidth, chunkHeight, chunkDepth, x, y, z);
    }

    public void Update()
    {
        for (var x = 0; x < World.XChunks; x++)
        for (var y = 0; y < World.YChunks; y++)
        for (var z = 0; z < World.ZChunks; z++)
            GetDrawableChunk(x, y, z).Update(World);
    }

    public void Draw(GraphicsDevice graphicsDevice, BasicEffect voxelEffect)
    {
        foreach (EffectPass currentTechniquePass in voxelEffect.CurrentTechnique.Passes)
        {
            currentTechniquePass.Apply();

            for (var x = 0; x < World.XChunks; x++)
            for (var y = 0; y < World.YChunks; y++)
            for (var z = 0; z < World.ZChunks; z++)
            {
                DrawableVoxelChunk chunk = GetDrawableChunk(x, y, z);

                graphicsDevice.SetVertexBuffer(chunk.VertexBuffer);
                graphicsDevice.Indices = chunk.IndexBuffer;

                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, chunk.PrimitiveCount);
            }
        }
    }

    public DrawableVoxelChunk GetDrawableChunk(int x, int y, int z)
    {
        return chunks[x + y * World.XChunks + z * World.XChunks * World.YChunks];
    }
}