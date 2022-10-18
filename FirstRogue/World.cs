using System;
using Microsoft.Xna.Framework;

namespace FirstRogue;

public class World
{
    public readonly int XChunks;
    public readonly int YChunks;
    public readonly int ZChunks;
    public readonly int ChunkWidth;
    public readonly int ChunkHeight;
    public readonly int ChunkDepth;
    public readonly int Width;
    public readonly int Height;
    public readonly int Depth;
    public readonly VoxelChunk[] Chunks;

    public World(int xChunks, int yChunks, int zChunks, int chunkWidth, int chunkHeight, int chunkDepth)
    {
        XChunks = xChunks;
        YChunks = yChunks;
        ZChunks = zChunks;
        ChunkWidth = chunkWidth;
        ChunkHeight = chunkHeight;
        ChunkDepth = chunkDepth;
        Width = XChunks * ChunkWidth;
        Height = YChunks * ChunkHeight;
        Depth = ZChunks * ChunkDepth;
        
        Chunks = new VoxelChunk[XChunks * YChunks * ZChunks];
        
        for (int x = 0; x < XChunks; x++)
        {
            for (int y = 0; y < YChunks; y++)
            {
                for (int z = 0; z < ZChunks; z++)
                {
                    var chunk = new VoxelChunk(ChunkWidth, ChunkHeight, ChunkDepth);
                    Chunks[x + y * XChunks + z * XChunks * YChunks] = chunk;
                }
            }
        }
    }

    public void GenerateWorld(Random random)
    {
        for (int x = 0; x < XChunks; x++)
        {
            for (int y = 0; y < YChunks; y++)
            {
                for (int z = 0; z < ZChunks; z++)
                {
                    GetChunk(x, y, z).GenerateTerrain(random);
                }
            }
        }
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

        int chunkX = x / ChunkWidth;
        int chunkY = y / ChunkHeight;
        int chunkZ = z / ChunkDepth;
        int subChunkX = x % ChunkWidth;
        int subChunkY = y % ChunkHeight;
        int subChunkZ = z % ChunkDepth;
        
        VoxelChunk chunk = GetChunk(chunkX, chunkY, chunkZ);
        chunk.SetVoxel(subChunkX, subChunkY, subChunkZ, voxel);

        UpdateChunkBoundaries(chunkX, chunkY, chunkZ, subChunkX, subChunkY, subChunkZ);
    }

    private void UpdateChunkBoundaries(int chunkX, int chunkY, int chunkZ, int subChunkX, int subChunkY, int subChunkZ)
    {
        if (subChunkX == 0 && GetChunk(chunkX - 1, chunkY, chunkZ) is { } xNegNeighbor)
        {
            xNegNeighbor.MarkChanged();
        }

        if (subChunkX == ChunkWidth - 1 && GetChunk(chunkX + 1, chunkY, chunkZ) is { } xPosNeighbor)
        {
            xPosNeighbor.MarkChanged();
        }
        
        if (subChunkY == 0 && GetChunk(chunkX, chunkY - 1, chunkZ) is { } yNegNeighbor)
        {
            yNegNeighbor.MarkChanged();
        }
        
        if (subChunkY == ChunkHeight - 1 && GetChunk(chunkX, chunkY + 1, chunkZ) is { } yPosNeighbor)
        {
            yPosNeighbor.MarkChanged();
        }
        
        if (subChunkZ == 0 && GetChunk(chunkX, chunkY, chunkZ - 1) is { } zNegNeighbor)
        {
            zNegNeighbor.MarkChanged();
        }
        
        if (subChunkZ == ChunkDepth - 1 && GetChunk(chunkX, chunkY, chunkZ + 1) is { } zPosNeighbor)
        {
            zPosNeighbor.MarkChanged();
        }
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

        int chunkX = x / ChunkWidth;
        int chunkY = y / ChunkHeight;
        int chunkZ = z / ChunkDepth;
        int subChunkX = x % ChunkWidth;
        int subChunkY = y % ChunkHeight;
        int subChunkZ = z % ChunkDepth;
        
        VoxelChunk chunk = GetChunk(chunkX, chunkY, chunkZ);
        return chunk.GetVoxel(subChunkX, subChunkY, subChunkZ);
    }

    public Voxels GetVoxel(Vector3 pos)
    {
        var x = (int)MathF.Floor(pos.X);
        var y = (int)MathF.Floor(pos.Y);
        var z = (int)MathF.Floor(pos.Z);

        return GetVoxel(x, y, z);
    }
}