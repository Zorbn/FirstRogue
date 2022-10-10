using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FirstRogue;

public class DrawableVoxelChunk
{
    public readonly VoxelChunk VoxelChunk;
    
    public readonly VertexBuffer VertexBuffer;
    public int PrimitiveCount { get; private set; }
    private VertexPositionTexture[] vertices;
    private int vertexCount;

    public DrawableVoxelChunk(GraphicsDevice graphicsDevice, int width, int height, int depth)
    {
        VoxelChunk = new VoxelChunk(width, height, depth);

        // Assuming that maximum faces in a chunk, without redundant faces, is Ceil(voxelCount/2) * 6.
        // Also account for 6 vertices per face.
        int maxVertices = (int)MathF.Ceiling(width * height * depth * 0.5f) * 6 * 6;
        vertices = new VertexPositionTexture[maxVertices];
        VertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionTexture), maxVertices, BufferUsage.WriteOnly);
    }
    
    public void GenerateMesh()
    {
        vertexCount = 0;
        
        for (var z = 0; z < VoxelChunk.Depth; z++)
        for (var y = 0; y < VoxelChunk.Height; y++)
        for (var x = 0; x < VoxelChunk.Width; x++)
        {
            Voxels voxel = VoxelChunk.GetVoxel(x, y, z);

            if (voxel == Voxels.Air) continue;

            var voxelPos = new Vector3(x, y, z);

            for (var di = 0; di < 6; di++)
            {
                var direction = (Directions)di;
                Vector3 dVec = direction.ToVec();
                
                if (VoxelChunk.GetVoxel(x + (int)dVec.X, y + (int)dVec.Y, z + (int)dVec.Z) != Voxels.Air) continue;

                for (var vi = 0; vi < 6; vi++)
                {
                    VertexPositionTexture vertex = CubeMesh.Vertices[direction][vi];
                    vertex.Position += voxelPos;
                    vertex.TextureCoordinate += CubeMesh.GetTexCoord(voxel);

                    vertices[vertexCount] = vertex;
                    vertexCount++;
                }
            }
        }
        
        VertexBuffer.SetData(vertices, 0, vertexCount);
        PrimitiveCount = vertexCount / 3;
    }
}