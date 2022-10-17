using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FirstRogue.Gfx;

public class DrawableVoxelChunk
{
    public readonly VertexBuffer VertexBuffer;
    public readonly IndexBuffer IndexBuffer;
    public readonly VoxelChunk VoxelChunk;
    private int vertexCount;
    private int indexCount;
    private readonly VertexPositionColorTexture[] vertices;
    private readonly uint[] indices;
    private float[] aoBuffer = new float[4];

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

                    /*
                     * Experimental AO
                     */

                    Vector3 dir = vertex.Position * 2;
                    dir.X -= 1;
                    dir.Y -= 1;
                    dir.Z -= 1;

                    Vector3 off1, off2;
                    
                    switch (direction)
                    {
                        case Directions.Forward:
                            off1 = new Vector3(dir.X, 0, dir.Z);
                            off2 = new Vector3(0, dir.Y, dir.Z);
                            break;
                        case Directions.Backward:
                            off1 = new Vector3(dir.X, 0, dir.Z);
                            off2 = new Vector3(0, dir.Y, dir.Z);
                            break;
                        case Directions.Left:
                            off1 = new Vector3(dir.X, dir.Y, 0);
                            off2 = new Vector3(dir.X, 0, dir.Z);
                            break;
                        case Directions.Right:
                            off1 = new Vector3(dir.X, dir.Y, 0);
                            off2 = new Vector3(dir.X, 0, dir.Z);
                            break;
                        case Directions.Up:
                            off1 = new Vector3(dir.X, dir.Y, 0);
                            off2 = new Vector3(0, dir.Y, dir.Z);
                            break;
                        case Directions.Down:
                            off1 = new Vector3(dir.X, dir.Y, 0);
                            off2 = new Vector3(0, dir.Y, dir.Z);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    var ao = 0;

                    bool side1 = VoxelChunk.GetVoxel(new Vector3(x, y, z) + off1) != Voxels.Air;
                    bool side2 = VoxelChunk.GetVoxel(new Vector3(x, y, z) + off2) != Voxels.Air;
                    bool corner = VoxelChunk.GetVoxel(new Vector3(x, y, z) + dir) != Voxels.Air;

                    if (side1 && side2)
                    {
                        ao = 0;
                    }
                    else
                    {
                        if (side1)
                        {
                            ao++;
                        }
                    
                        if (side2)
                        {
                            ao++;
                        }
                    
                        if (corner)
                        {
                            ao++;
                        }

                        ao = 3 - ao;
                    }

                    aoBuffer[vi] = ao;

                    float aoLightValue = ao / 3f;
                    var aoColor = new Vector3(aoLightValue, aoLightValue, aoLightValue);
                    var original = vertex.Color.ToVector3();
                    vertex.Color = new Color(original * aoColor);
                    
                    /*
                     * End Experimental AO
                     */

                    vertex.Position += voxelPos;
                    vertex.TextureCoordinate += CubeMesh.GetTexCoord(voxel);
                    
                    vertices[vertexCount] = vertex;
                    vertexCount++;
                }

                int faceStart = vertexCount - 4;
                VertexPositionColorTexture v0 = vertices[faceStart];
                VertexPositionColorTexture v1 = vertices[faceStart + 1];
                VertexPositionColorTexture v2 = vertices[faceStart + 2];
                VertexPositionColorTexture v3 = vertices[faceStart + 3];
            
                if (aoBuffer[0] + aoBuffer[2] < aoBuffer[1] + aoBuffer[3])
                {
                    vertices[faceStart] = v3;
                    vertices[faceStart + 1] = v0;
                    vertices[faceStart + 2] = v1;
                    vertices[faceStart + 3] = v2;
                }
            }
        }

        VertexBuffer.SetData(vertices, 0, vertexCount);
        IndexBuffer.SetData(indices, 0, indexCount);
        PrimitiveCount = indexCount / 3;
    }
}