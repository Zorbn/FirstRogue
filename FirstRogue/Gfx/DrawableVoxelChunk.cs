using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FirstRogue.Gfx;

public class DrawableVoxelChunk
{
    public IndexBuffer IndexBuffer;
    private readonly ArrayList<uint> indices;
    public VertexBuffer VertexBuffer;
    private readonly ArrayList<VertexPositionColorTexture> vertices;
    private readonly float[] aoBuffer = new float[4];
    private readonly int chunkX, chunkY, chunkZ;

    private const float NoiseMin = 0.8f;
    private readonly float[] noise;

    public DrawableVoxelChunk(int chunkX, int chunkY, int chunkZ)
    {
        this.chunkX = chunkX;
        this.chunkY = chunkY;
        this.chunkZ = chunkZ;
        
        noise = new float[VoxelChunk.Width * VoxelChunk.Height * VoxelChunk.Depth];

        int chunkWorldX = chunkX * VoxelChunk.Width;
        int chunkWorldY = chunkY * VoxelChunk.Height;
        int chunkWorldZ = chunkZ * VoxelChunk.Depth;

        for (int z = 0; z < VoxelChunk.Depth; z++)
        for (int y = 0; y < VoxelChunk.Height; y++)
        for (int x = 0; x < VoxelChunk.Width; x++)
        {
            int worldX = x + chunkWorldX;
            int worldY = y + chunkWorldY;
            int worldZ = z + chunkWorldZ;

            int noiseI = x + y * VoxelChunk.Width + z * VoxelChunk.Width * VoxelChunk.Height;

            noise[noiseI] = GameRandom.GradientNoise(worldX, worldY, worldZ) * 0.2f + NoiseMin;
        }

        vertices = new ArrayList<VertexPositionColorTexture>();
        indices = new ArrayList<uint>();
    }

    public int PrimitiveCount { get; private set; }

    public void Update(World world, GraphicsDevice graphicsDevice)
    {
        VoxelChunk voxelChunk = world.GetChunk(chunkX, chunkY, chunkZ);
        
        if (voxelChunk.Changed)
        {
            var sw = new Stopwatch();
            sw.Start();
            GenerateMesh(world, voxelChunk, graphicsDevice);
            Console.WriteLine(sw.Elapsed.TotalMilliseconds);
            voxelChunk.UnmarkChanged();
        }
    }

    public void GenerateMesh(World world, VoxelChunk voxelChunk, GraphicsDevice graphicsDevice)
    {
        vertices.Clear();
        indices.Clear();

        for (int z = 0; z < VoxelChunk.Depth; z++)
        for (int y = 0; y < VoxelChunk.Height; y++)
        for (int x = 0; x < VoxelChunk.Width; x++)
        {
            Voxels voxel = voxelChunk.voxels[x + y * VoxelChunk.Width + z * VoxelChunk.Width * VoxelChunk.Height];

            if (voxel == Voxels.Air) continue;

            int worldX = x + chunkX * VoxelChunk.Width;
            int worldY = y + chunkY * VoxelChunk.Height;
            int worldZ = z + chunkZ * VoxelChunk.Depth;

            // for (var di = 0; di < 6; di++)
            // {
            //     var direction = (Directions)di;
            //
            //     if (world.GetVoxel(voxelWorldPos + direction.ToVec()) != Voxels.Air) continue;
            //
            //     AddFace(voxelWorldPos, voxel, direction);
            // }

            uint opaqueBitmask = world.GetVoxel(worldX, worldY, worldZ - 1) != Voxels.Air ? 1u : 0u;
            opaqueBitmask |= (world.GetVoxel(worldX, worldY, worldZ + 1) != Voxels.Air ? 1u : 0u) << 1;
            opaqueBitmask |= (world.GetVoxel(worldX + 1, worldY, worldZ) != Voxels.Air ? 1u : 0u) << 2;
            opaqueBitmask |= (world.GetVoxel(worldX - 1, worldY, worldZ) != Voxels.Air ? 1u : 0u) << 3;
            opaqueBitmask |= (world.GetVoxel(worldX, worldY + 1, worldZ) != Voxels.Air ? 1u : 0u) << 4;
            opaqueBitmask |= (world.GetVoxel(worldX, worldY - 1, worldZ) != Voxels.Air ? 1u : 0u) << 5;

            if (opaqueBitmask == 0) continue;

            if ((opaqueBitmask & 0b000001) == 0) AddFace(world, x, y, z, worldX, worldY, worldZ, voxel, Directions.Forward);
            if ((opaqueBitmask & 0b000010) == 0) AddFace(world, x, y, z, worldX, worldY, worldZ, voxel, Directions.Backward);
            if ((opaqueBitmask & 0b000100) == 0) AddFace(world, x, y, z, worldX, worldY, worldZ, voxel, Directions.Right);
            if ((opaqueBitmask & 0b001000) == 0) AddFace(world, x, y, z, worldX, worldY, worldZ, voxel, Directions.Left);
            if ((opaqueBitmask & 0b010000) == 0) AddFace(world, x, y, z, worldX, worldY, worldZ, voxel, Directions.Up);
            if ((opaqueBitmask & 0b100000) == 0) AddFace(world, x, y, z, worldX, worldY, worldZ, voxel, Directions.Down);
        }

        if (VertexBuffer is null || VertexBuffer.VertexCount < vertices.Count)
        {
            VertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColorTexture), vertices.Array.Length,
                BufferUsage.WriteOnly);
        }

        if (IndexBuffer is null || IndexBuffer.IndexCount < indices.Count)
        {
            IndexBuffer = new IndexBuffer(graphicsDevice, typeof(uint), indices.Array.Length, BufferUsage.WriteOnly);
        }

        VertexBuffer.SetData(vertices.Array, 0, vertices.Count);
        IndexBuffer.SetData(indices.Array, 0, indices.Count);
        PrimitiveCount = indices.Count / 3;
    }

    private void AddFace(World world, int x, int y, int z, int worldX, int worldY, int worldZ, Voxels voxel, Directions direction)
    {
        var directionI = (int)direction;

        indices.Reserve(6);

        for (var ii = 0; ii < 6; ii++)
        {
            indices.AddUnchecked((uint)(CubeMesh.Indices[directionI][ii] + vertices.Count));
        }

        vertices.Reserve(4);

        for (var vi = 0; vi < 4; vi++)
        {
            VertexPositionColorTexture vertex = CubeMesh.Vertices[directionI][vi];

            int noiseI = x + y * VoxelChunk.Width + z * VoxelChunk.Width * VoxelChunk.Height;
            float variance = noise[noiseI];
            int ao = CheckVertexNeighbors(world, worldX, worldY, worldZ, CubeMesh.VertexAoDirections[directionI][vi], direction);

            aoBuffer[vi] = ao;
            float aoLightValue = ao * 0.33f;
            float colorLightness = variance * aoLightValue;
            vertex.Color = new Color((byte)(colorLightness * vertex.Color.R), (byte)(colorLightness * vertex.Color.G), (byte)(colorLightness * vertex.Color.B));

            vertex.Position = new Vector3(vertex.Position.X + worldX, vertex.Position.Y + worldY, vertex.Position.Z + worldZ);

            int voxelI = (int)voxel * 4;
            float u = CubeMesh.UnitX * (voxelI & (CubeMesh.TileSize - 1));
            float v = CubeMesh.UnitY * (voxelI >> CubeMesh.TileSizeShift * 2);

            vertex.TextureCoordinate = new Vector2(vertex.TextureCoordinate.X + u, vertex.TextureCoordinate.Y + v);

            vertices.AddUnchecked(vertex);
        }

        OrientLastFace();
    }

    private int CheckVertexNeighbors(World world, int worldX, int worldY, int worldZ, IVector3 vertexAoDirections, Directions direction)
    {
        bool side1;
        bool side2;
        bool corner;

        switch (direction)
        {
            case Directions.Forward:
            case Directions.Backward:
                side1 = world.GetVoxel(worldX + vertexAoDirections.X, worldY, worldZ + vertexAoDirections.Z) !=
                        Voxels.Air;
                side2 = world.GetVoxel(worldX, worldY + vertexAoDirections.Y, worldZ + vertexAoDirections.Z) !=
                        Voxels.Air;
                corner = world.GetVoxel(worldX + vertexAoDirections.X, worldY + vertexAoDirections.Y,
                    worldZ + vertexAoDirections.Z) != Voxels.Air;
                break;
            case Directions.Right:
            case Directions.Left:
                side1 = world.GetVoxel(worldX + vertexAoDirections.X, worldY + vertexAoDirections.Y, worldZ) !=
                        Voxels.Air;
                side2 = world.GetVoxel(worldX + vertexAoDirections.X, worldY, worldZ + vertexAoDirections.Z) !=
                        Voxels.Air;
                corner = world.GetVoxel(worldX + vertexAoDirections.X, worldY + vertexAoDirections.Y,
                    worldZ + vertexAoDirections.Z) != Voxels.Air;
                break;
            default:
                side1 = world.GetVoxel(worldX, worldY + vertexAoDirections.Y, worldZ + vertexAoDirections.Z) !=
                        Voxels.Air;
                side2 = world.GetVoxel(worldX + vertexAoDirections.X, worldY + vertexAoDirections.Y, worldZ) !=
                        Voxels.Air;
                corner = world.GetVoxel(worldX + vertexAoDirections.X, worldY + vertexAoDirections.Y,
                    worldZ + vertexAoDirections.Z) != Voxels.Air;
                break;
        }

        if (side1 && side2)
        {
            return 0;
        }

        var occupied = 0;

        if (side1) ++occupied;

        if (side2) ++occupied;

        if (corner) ++occupied;

        return 3 - occupied;
    }

    // Ensure that color interpolation will be correct for the most recent face.
    private void OrientLastFace()
    {
        if (aoBuffer[0] + aoBuffer[2] > aoBuffer[1] + aoBuffer[3]) return;

        int faceStart = vertices.Count - 4;
        VertexPositionColorTexture v0 = vertices[faceStart];
        VertexPositionColorTexture v1 = vertices[faceStart + 1];
        VertexPositionColorTexture v2 = vertices[faceStart + 2];
        VertexPositionColorTexture v3 = vertices[faceStart + 3];
        
        vertices[faceStart] = v3;
        vertices[faceStart + 1] = v0;
        vertices[faceStart + 2] = v1;
        vertices[faceStart + 3] = v2;
    }
}