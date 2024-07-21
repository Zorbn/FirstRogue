using System;
using Microsoft.Xna.Framework;

namespace FirstRogue;

public class World
{
    public readonly VoxelChunk[] Chunks;
    
    public const int ChunkDepth = 32;
    public const int ChunkHeight = 32;
    public const int ChunkWidth = 32;

    public const int ChunkDepthShift = 5;
    public const int ChunkHeightShift = 5;
    public const int ChunkWidthShift = 5;
    
    public const int XChunks = 2;
    public const int YChunks = 2;
    public const int ZChunks = 2;

    public const int Depth = ChunkDepth * ZChunks;
    public const int Height = ChunkHeight * YChunks;
    public const int Width = ChunkWidth * XChunks;

    public World()
    {
        Chunks = new VoxelChunk[XChunks * YChunks * ZChunks];

        for (var x = 0; x < XChunks; x++)
        for (var y = 0; y < YChunks; y++)
        for (var z = 0; z < ZChunks; z++)
        {
            var chunk = new VoxelChunk(ChunkWidth, ChunkHeight, ChunkDepth);
            Chunks[x + y * XChunks + z * XChunks * YChunks] = chunk;
        }
    }

    public void GenerateWorld(Random random)
    {
        for (var x = 0; x < XChunks; x++)
        for (var y = 0; y < YChunks; y++)
        for (var z = 0; z < ZChunks; z++)
            GetChunk(x, y, z).GenerateTerrain(random);
    }

    public VoxelChunk GetChunkUnchecked(int x, int y, int z)
    {
        int i = x + y * XChunks + z * XChunks * YChunks;

        return Chunks[i];
    }

    public VoxelChunk GetChunk(int x, int y, int z)
    {
        int i = x + y * XChunks + z * XChunks * YChunks;

        if (i < 0 || i >= XChunks * YChunks * ZChunks) return null;

        return Chunks[i];
    }

    public void SetVoxel(int x, int y, int z, Voxels voxel)
    {
        if (x < 0 || y < 0 || z < 0 || x >= Width || y >= Height || z >= Depth) return;

        int chunkX = x >> ChunkWidthShift;
        int chunkY = y >> ChunkHeightShift;
        int chunkZ = z >> ChunkDepthShift;
        int subChunkX = x & (ChunkWidth - 1);
        int subChunkY = y & (ChunkHeight - 1);
        int subChunkZ = z & (ChunkDepth - 1);

        VoxelChunk chunk = GetChunkUnchecked(chunkX, chunkY, chunkZ);
        chunk.SetVoxelUnchecked(subChunkX, subChunkY, subChunkZ, voxel);

        UpdateChunkBoundaries(chunkX, chunkY, chunkZ, subChunkX, subChunkY, subChunkZ);
    }

    private void UpdateChunkBoundaries(int chunkX, int chunkY, int chunkZ, int subChunkX, int subChunkY, int subChunkZ)
    {
        if (subChunkX == 0 && GetChunk(chunkX - 1, chunkY, chunkZ) is { } xNegNeighbor) xNegNeighbor.MarkChanged();

        if (subChunkX == ChunkWidth - 1 && GetChunk(chunkX + 1, chunkY, chunkZ) is { } xPosNeighbor)
            xPosNeighbor.MarkChanged();

        if (subChunkY == 0 && GetChunk(chunkX, chunkY - 1, chunkZ) is { } yNegNeighbor) yNegNeighbor.MarkChanged();

        if (subChunkY == ChunkHeight - 1 && GetChunk(chunkX, chunkY + 1, chunkZ) is { } yPosNeighbor)
            yPosNeighbor.MarkChanged();

        if (subChunkZ == 0 && GetChunk(chunkX, chunkY, chunkZ - 1) is { } zNegNeighbor) zNegNeighbor.MarkChanged();

        if (subChunkZ == ChunkDepth - 1 && GetChunk(chunkX, chunkY, chunkZ + 1) is { } zPosNeighbor)
            zPosNeighbor.MarkChanged();
    }

    public void SetVoxel(Vector3 pos, Voxels voxel)
    {
        var x = (int)MathF.Floor(pos.X);
        var y = (int)MathF.Floor(pos.Y);
        var z = (int)MathF.Floor(pos.Z);

        SetVoxel(x, y, z, voxel);
    }

    public Voxels GetVoxel(int x, int y, int z)
    {
        if (x < 0 || y < 0 || z < 0 || x >= Width || y >= Height || z >= Depth) return Voxels.Air;

        int chunkX = x >> ChunkWidthShift;
        int chunkY = y >> ChunkHeightShift;
        int chunkZ = z >> ChunkDepthShift;
        int subChunkX = x & (ChunkWidth - 1);
        int subChunkY = y & (ChunkHeight - 1);
        int subChunkZ = z & (ChunkDepth - 1);

        VoxelChunk chunk = GetChunkUnchecked(chunkX, chunkY, chunkZ);
        return chunk.GetVoxelUnchecked(subChunkX, subChunkY, subChunkZ);
    }

    public Voxels GetVoxel(Vector3 pos)
    {
        var x = (int)MathF.Floor(pos.X);
        var y = (int)MathF.Floor(pos.Y);
        var z = (int)MathF.Floor(pos.Z);

        return GetVoxel(x, y, z);
    }

    public Voxels GetVoxel(IVector3 pos)
    {
        return GetVoxel(pos.X, pos.Y, pos.Z);
    }
}