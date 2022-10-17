using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FirstRogue.Gfx;

public class DrawableVoxelChunk
{
    public readonly IndexBuffer IndexBuffer;
    private readonly uint[] indices;
    public readonly VertexBuffer VertexBuffer;
    private readonly VertexPositionColorTexture[] vertices;
    public readonly VoxelChunk VoxelChunk;
    private readonly float[] aoBuffer = new float[4];
    private int indexCount;
    private int vertexCount;

    public DrawableVoxelChunk(GraphicsDevice graphicsDevice, int width, int height, int depth)
    {
        VoxelChunk = new VoxelChunk(width, height, depth);

        // Assuming that maximum faces in a chunk, without redundant faces, is Ceil(voxelCount/2) * 6.
        // Also account for 4 vertices per face and 6 indices per face.
        int maxFaces = (int)MathF.Ceiling(width * height * depth * 0.5f) * 6;
        int maxVertices = maxFaces * 4;
        int maxIndices = maxFaces * 6;

        vertices = new VertexPositionColorTexture[maxVertices];
        VertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColorTexture), maxVertices,
            BufferUsage.WriteOnly);

        indices = new uint[maxIndices];
        IndexBuffer = new IndexBuffer(graphicsDevice, typeof(uint), maxIndices, BufferUsage.WriteOnly);
    }

    public int PrimitiveCount { get; private set; }

    public void Update()
    {
        if (VoxelChunk.Changed)
        {
            GenerateMesh();
            VoxelChunk.UnmarkChanged();
        }
    }

    public void GenerateMesh()
    {
        vertexCount = 0;
        indexCount = 0;

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

                for (var ii = 0; ii < 6; ii++)
                {
                    indices[indexCount] = (uint)(CubeMesh.Indices[direction][ii] + vertexCount);
                    indexCount++;
                }

                for (var vi = 0; vi < 4; vi++)
                {
                    VertexPositionColorTexture vertex = CubeMesh.Vertices[direction][vi];
                    
                    VertexNeighbors neighbors = CheckVertexNeighbors(voxelPos, vertex.Position, direction);

                    int ao = CalculateAoLevel(neighbors);
                    aoBuffer[vi] = ao;
                    float aoLightValue = MathF.Min(ao / 3f + 0.1f, 1f);
                    vertex.Color = new Color(vertex.Color.ToVector3() * aoLightValue);

                    vertex.Position += voxelPos;
                    vertex.TextureCoordinate += CubeMesh.GetTexCoord(voxel);

                    vertices[vertexCount] = vertex;
                    vertexCount++;
                }

                OrientLastFace();
            }
        }

        VertexBuffer.SetData(vertices, 0, vertexCount);
        IndexBuffer.SetData(indices, 0, indexCount);
        PrimitiveCount = indexCount / 3;
    }

    private static int CalculateAoLevel(VertexNeighbors neighbors)
    {
        if (neighbors.Side1 && neighbors.Side2)
        {
            return 0;
        }
        
        var occupied = 0;

        if (neighbors.Side1) occupied++;

        if (neighbors.Side2) occupied++;

        if (neighbors.Corner) occupied++;

        return 3 - occupied;
    }

    private VertexNeighbors CheckVertexNeighbors(Vector3 voxelPos, Vector3 vertexPos, Directions direction)
    {
        Vector3 dir = vertexPos * 2;
        dir.X -= 1;
        dir.Y -= 1;
        dir.Z -= 1;

        int outwardComponent = direction.OutwardComponentI();
        Vector3 dirSide1 = dir;
        dirSide1.SetComponent((outwardComponent + 2) % 3, 0f);
        Vector3 dirSide2 = dir;
        dirSide2.SetComponent((outwardComponent + 1) % 3, 0f);

        return new VertexNeighbors
        {
            Side1 = VoxelChunk.GetVoxel(voxelPos + dirSide1) != Voxels.Air,
            Side2 = VoxelChunk.GetVoxel(voxelPos + dirSide2) != Voxels.Air,
            Corner = VoxelChunk.GetVoxel(voxelPos + dir) != Voxels.Air
        };
    }

    // Ensure that color interpolation will be correct for the most recent face.
    private void OrientLastFace()
    {
        int faceStart = vertexCount - 4;
        VertexPositionColorTexture v0 = vertices[faceStart];
        VertexPositionColorTexture v1 = vertices[faceStart + 1];
        VertexPositionColorTexture v2 = vertices[faceStart + 2];
        VertexPositionColorTexture v3 = vertices[faceStart + 3];

        if (!(aoBuffer[0] + aoBuffer[2] < aoBuffer[1] + aoBuffer[3])) return;
        
        vertices[faceStart] = v3;
        vertices[faceStart + 1] = v0;
        vertices[faceStart + 2] = v1;
        vertices[faceStart + 3] = v2;
    }

    private struct VertexNeighbors
    {
        public bool Side1;
        public bool Side2;
        public bool Corner;
    }
}