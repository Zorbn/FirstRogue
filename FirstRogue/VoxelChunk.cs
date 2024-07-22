using System;
using Microsoft.Xna.Framework;

namespace FirstRogue;

public class VoxelChunk
{
    public const int Depth = 32;
    public const int Height = 32;
    public const int Width = 32;

    public const int DepthShift = 5;
    public const int HeightShift = 5;
    public const int WidthShift = 5;

    public readonly Voxels[] voxels;

    public VoxelChunk()
    {
        voxels = new Voxels[Width * Height * Depth];
    }

    public bool Changed { get; private set; }

    public void GenerateTerrain(Random random)
    {
        for (var z = 0; z < Depth; z++)
        for (var y = 0; y < Height; y++)
        for (var x = 0; x < Width; x++)
        {
            int voxelI = random.Next(0, 4);

            if (y == Height - 1) voxelI = 2;
            
            SetVoxel(x, y, z, (Voxels)voxelI);
        }
    }

    public void SetVoxelUnchecked(int x, int y, int z, Voxels voxel)
    {
        int vi = x + y * Width + z * Width * Height;
        voxels[vi] = voxel;
        Changed = true;
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
        var x = (int)MathF.Floor(pos.X);
        var y = (int)MathF.Floor(pos.Y);
        var z = (int)MathF.Floor(pos.Z);

        SetVoxel(x, y, z, voxel);
    }

    public Voxels GetVoxelUnchecked(int x, int y, int z)
    {
        int vi = x + y * Width + z * Width * Height;

        return voxels[vi];
    }

    public Voxels GetVoxel(int x, int y, int z)
    {
        if (x < 0 || y < 0 || z < 0 || x >= Width || y >= Height || z >= Depth) return Voxels.Air;

        int vi = x + y * Width + z * Width * Height;
        return voxels[vi];
    }

    public Voxels GetVoxel(Vector3 pos)
    {
        var x = (int)MathF.Floor(pos.X);
        var y = (int)MathF.Floor(pos.Y);
        var z = (int)MathF.Floor(pos.Z);

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