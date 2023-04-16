using Microsoft.Xna.Framework;

namespace FirstRogue;

public struct Hit
{
    public Voxels Voxel;
    public float Distance;
    public Vector3 Pos;
    public Vector3 LastPos;
}