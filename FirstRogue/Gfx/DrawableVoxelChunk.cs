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

    public DrawableVoxelChunk(int width, int height, int depth, int chunkX, int chunkY, int chunkZ)
    {
        this.chunkX = chunkX;
        this.chunkY = chunkY;
        this.chunkZ = chunkZ;
        
        noise = new float[(width + 2) * (height + 2) * (depth + 2)];

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

        for (var z = 0; z < voxelChunk.Depth; z++)
        for (var y = 0; y < voxelChunk.Height; y++)
        for (var x = 0; x < voxelChunk.Width; x++)
        {
            Voxels voxel = voxelChunk.voxels[x + y * voxelChunk.Width + z * voxelChunk.Width * voxelChunk.Height];

            if (voxel == Voxels.Air) continue;

            int worldX = x + chunkX * voxelChunk.Width;
            int worldY = y + chunkY * voxelChunk.Height;
            int worldZ = z + chunkZ * voxelChunk.Depth;
            IVector3 voxelWorldPos = new IVector3(worldX, worldY, worldZ);

            // for (var di = 0; di < 6; di++)
            // {
            //     var direction = (Directions)di;
            //
            //     if (world.GetVoxel(voxelWorldPos + direction.ToVec()) != Voxels.Air) continue;
            //
            //     AddFace(voxelWorldPos, voxel, direction);
            // }

            uint opaqueBitmask = world.GetVoxel(voxelWorldPos.X, voxelWorldPos.Y, voxelWorldPos.Z - 1) != Voxels.Air ? 1u : 0u;
            opaqueBitmask |= (world.GetVoxel(voxelWorldPos.X, voxelWorldPos.Y, voxelWorldPos.Z + 1) != Voxels.Air ? 1u : 0u) << 1;
            opaqueBitmask |= (world.GetVoxel(voxelWorldPos.X + 1, voxelWorldPos.Y, voxelWorldPos.Z) != Voxels.Air ? 1u : 0u) << 2;
            opaqueBitmask |= (world.GetVoxel(voxelWorldPos.X - 1, voxelWorldPos.Y, voxelWorldPos.Z) != Voxels.Air ? 1u : 0u) << 3;
            opaqueBitmask |= (world.GetVoxel(voxelWorldPos.X, voxelWorldPos.Y + 1, voxelWorldPos.Z) != Voxels.Air ? 1u : 0u) << 4;
            opaqueBitmask |= (world.GetVoxel(voxelWorldPos.X, voxelWorldPos.Y - 1, voxelWorldPos.Z) != Voxels.Air ? 1u : 0u) << 5;

            if (opaqueBitmask == 0) continue;

            if ((opaqueBitmask & 0b000001) == 0) AddFace(voxelWorldPos, voxel, Directions.Forward);
            if ((opaqueBitmask & 0b000010) == 0) AddFace(voxelWorldPos, voxel, Directions.Backward);
            if ((opaqueBitmask & 0b000100) == 0) AddFace(voxelWorldPos, voxel, Directions.Right);
            if ((opaqueBitmask & 0b001000) == 0) AddFace(voxelWorldPos, voxel, Directions.Left);
            if ((opaqueBitmask & 0b010000) == 0) AddFace(voxelWorldPos, voxel, Directions.Up);
            if ((opaqueBitmask & 0b100000) == 0) AddFace(voxelWorldPos, voxel, Directions.Down);
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

    private void AddFace(IVector3 voxelWorldPos, Voxels voxel, Directions direction)
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

            // int noiseI = (x + 1) + (y + 1) * voxelChunk.Width + (z + 1) * voxelChunk.Width * voxelChunk.Height;
            // float variance = noise[noiseI];
            // if (variance == 0f)
            // {
            //     variance = GameRandom.GradientNoise(worldX, worldY, worldZ) * 0.2f + NoiseMin;
            //     noise[noiseI] = variance;
            // }
            //
            // vertex.Color = new Color(vertex.Color.ToVector3() * variance);
            //
            // VertexNeighbors neighbors = CheckVertexNeighbors(world, voxelWorldPos, vertex.Position, direction);
            //
            // int ao = CalculateAoLevel(neighbors);
            // aoBuffer[vi] = ao;
            // float aoLightValue = MathF.Min(ao / 3f + 0.1f, 1f);
            // vertex.Color = new Color(vertex.Color.ToVector3() * aoLightValue);

            vertex.Position.X += voxelWorldPos.X;
            vertex.Position.Y += voxelWorldPos.Y;
            vertex.Position.Z += voxelWorldPos.Z;

            int voxelI = (int)voxel * 4;
            float u = CubeMesh.UnitX * (voxelI & (CubeMesh.TileSize - 1));
            float v = CubeMesh.UnitY * (voxelI >> CubeMesh.TileSizeShift * 2);

            vertex.TextureCoordinate.X += u;
            vertex.TextureCoordinate.Y += v;

            vertices.AddUnchecked(vertex);
        }

        // OrientLastFace();
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

    private VertexNeighbors CheckVertexNeighbors(World world, Vector3 voxelWorldPos, Vector3 vertexPos, Directions direction)
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
            Side1 = world.GetVoxel(voxelWorldPos + dirSide1) != Voxels.Air,
            Side2 = world.GetVoxel(voxelWorldPos + dirSide2) != Voxels.Air,
            Corner = world.GetVoxel(voxelWorldPos + dir) != Voxels.Air
        };
    }

    // Ensure that color interpolation will be correct for the most recent face.
    private void OrientLastFace()
    {
        int faceStart = vertices.Count - 4;
        VertexPositionColorTexture v0 = vertices[faceStart];
        VertexPositionColorTexture v1 = vertices[faceStart + 1];
        VertexPositionColorTexture v2 = vertices[faceStart + 2];
        VertexPositionColorTexture v3 = vertices[faceStart + 3];

        if (aoBuffer[0] + aoBuffer[2] > aoBuffer[1] + aoBuffer[3]) return;
        
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