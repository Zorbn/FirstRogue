using System;
using Microsoft.Xna.Framework;

namespace FirstRogue;

// TODO: Add sub-chunks for faster updating.
// Sub-chunks store their own meshes, on each update, only one sub-chunk mesh is regenerated.
// Then, the chunk mesh is created by appending the sub-chunk meshes to each other.
public class VoxelChunk
{
    public readonly int Depth;
    public readonly int Height;
    public readonly int Width;

    private readonly Voxels[] voxels;

    public VoxelChunk(int width, int height, int depth)
    {
        Width = width;
        Height = height;
        Depth = depth;
        voxels = new Voxels[width * height * depth];
    }

    public bool Changed { get; private set; }

    public void GenerateTerrain(Random random)
    {
        for (var z = 0; z < Depth; z++)
        for (var y = 0; y < Height; y++)
        for (var x = 0; x < Width; x++)
        {
            int voxelI = random.Next(0, 4);

            if (y > Height * 0.75f) continue;
            if (y > Height / 2) voxelI = 2;
            
            SetVoxel(x, y, z, (Voxels)voxelI);
        }
    }

    public void SetVoxel(int x, int y, int z, Voxels voxel)
    {
        if (x < 0 || y < 0 || z < 0 || x >= Width || y >= Height || z >= Depth) return;

        int vi = x + y * Width + z * Width * Height;
        voxels[vi] = voxel;
        Changed = true;
    }

    public void SetVoxel(Vector3 pos, Voxels voxel)
    {
        var x = (int)pos.X;
        var y = (int)pos.Y;
        var z = (int)pos.Z;

        SetVoxel(x, y, z, voxel);
    }

    public Voxels GetVoxel(int x, int y, int z)
    {
        if (x < 0 || y < 0 || z < 0 || x >= Width || y >= Height || z >= Depth) return Voxels.Air;

        int vi = x + y * Width + z * Width * Height;
        return voxels[vi];
    }
    
    public Voxels GetVoxel(Vector3 pos)
    {
        var x = (int)pos.X;
        var y = (int)pos.Y;
        var z = (int)pos.Z;

        return GetVoxel(x, y, z);
    }

    public void UnmarkChanged()
    {
        Changed = false;
    }
    
    public void MarkChanged()
    {
        Changed = true;
    }
}