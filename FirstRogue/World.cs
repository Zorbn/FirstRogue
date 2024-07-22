using System;
using Microsoft.Xna.Framework;

namespace FirstRogue;

public class World
{
    public readonly VoxelChunk[] Chunks;
    
    public const int XChunks = 20;
    public const int YChunks = 2;
    public const int ZChunks = 20;

    public const int Depth = VoxelChunk.Depth * ZChunks;
    public const int Height = VoxelChunk.Height * YChunks;
    public const int Width = VoxelChunk.Width * XChunks;

    public World()
    {
        Chunks = new VoxelChunk[XChunks * YChunks * ZChunks];

        for (var x = 0; x < XChunks; x++)
        for (var y = 0; y < YChunks; y++)
        for (var z = 0; z < ZChunks; z++)
        {
            var chunk = new VoxelChunk();
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

        int chunkX = x >> VoxelChunk.WidthShift;
        int chunkY = y >> VoxelChunk.HeightShift;
        int chunkZ = z >> VoxelChunk.DepthShift;
        int subChunkX = x & (VoxelChunk.Width - 1);
        int subChunkY = y & (VoxelChunk.Height - 1);
        int subChunkZ = z & (VoxelChunk.Depth - 1);

        VoxelChunk chunk = GetChunkUnchecked(chunkX, chunkY, chunkZ);
        chunk.SetVoxelUnchecked(subChunkX, subChunkY, subChunkZ, voxel);

        UpdateChunkBoundaries(chunkX, chunkY, chunkZ, subChunkX, subChunkY, subChunkZ);
    }

    private void UpdateChunkBoundaries(int chunkX, int chunkY, int chunkZ, int subChunkX, int subChunkY, int subChunkZ)
    {
        if (subChunkX == 0 && GetChunk(chunkX - 1, chunkY, chunkZ) is { } xNegNeighbor) xNegNeighbor.MarkChanged();

        if (subChunkX == VoxelChunk.Width - 1 && GetChunk(chunkX + 1, chunkY, chunkZ) is { } xPosNeighbor)
            xPosNeighbor.MarkChanged();

        if (subChunkY == 0 && GetChunk(chunkX, chunkY - 1, chunkZ) is { } yNegNeighbor) yNegNeighbor.MarkChanged();

        if (subChunkY == VoxelChunk.Height - 1 && GetChunk(chunkX, chunkY + 1, chunkZ) is { } yPosNeighbor)
            yPosNeighbor.MarkChanged();

        if (subChunkZ == 0 && GetChunk(chunkX, chunkY, chunkZ - 1) is { } zNegNeighbor) zNegNeighbor.MarkChanged();

        if (subChunkZ == VoxelChunk.Depth - 1 && GetChunk(chunkX, chunkY, chunkZ + 1) is { } zPosNeighbor)
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

        int chunkX = x >> VoxelChunk.WidthShift;
        int chunkY = y >> VoxelChunk.HeightShift;
        int chunkZ = z >> VoxelChunk.DepthShift;
        int subChunkX = x & (VoxelChunk.Width - 1);
        int subChunkY = y & (VoxelChunk.Height - 1);
        int subChunkZ = z & (VoxelChunk.Depth - 1);

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