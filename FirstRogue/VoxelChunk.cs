using System;

namespace FirstRogue;

public class VoxelChunk
{
    public readonly int Width;
    public readonly int Height;
    public readonly int Depth;

    private Voxels[] voxels;
    
    public VoxelChunk(int width, int height, int depth)
    {
        Width = width;
        Height = height;
        Depth = depth;
        voxels = new Voxels[width * height * depth];
    }
    
    public void GenerateTerrain(Random random)
    {
        for (var z = 0; z < Depth; z++)
        for (var y = 0; y < Height; y++)
        for (var x = 0; x < Width; x++)
        {
            SetVoxel(x, y, z, (Voxels)random.Next(0, 4));
        }
    }

    public void SetVoxel(int x, int y, int z, Voxels voxel)
    {
        if (x < 0 || y < 0 || z < 0 || x >= Width || y >= Height || z >= Depth) return;
        
        int vi = x + y * Width + z * Width * Height;
        voxels[vi] = voxel;
    }
    
    public Voxels GetVoxel(int x, int y, int z)
    {
        if (x < 0 || y < 0 || z < 0 || x >= Width || y >= Height || z >= Depth) return Voxels.Air;
        
        int vi = x + y * Width + z * Width * Height;
        return voxels[vi];
    }
}