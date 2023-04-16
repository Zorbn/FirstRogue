using System;
using Microsoft.Xna.Framework;

namespace FirstRogue;

public static class Raycast
{
    public static Hit Cast(World world, Vector3 start, Vector3 direction, float range)
    {
        var tileDirection = new Vector3(MathF.Sign(direction.X), MathF.Sign(direction.Y), MathF.Sign(direction.Z));

        var step = new Vector3
        {
            X = MathF.Abs(1 / direction.X),
            Y = MathF.Abs(1 / direction.Y),
            Z = MathF.Abs(1 / direction.Z)
        };

        Vector3 initialStep;

        if (direction.X > 0)
            initialStep.X = (MathF.Ceiling(start.X) - start.X) * step.X;
        else
            initialStep.X = (start.X - MathF.Floor(start.X)) * step.X;

        if (direction.Y > 0)
            initialStep.Y = (MathF.Ceiling(start.Y) - start.Y) * step.Y;
        else
            initialStep.Y = (start.Y - MathF.Floor(start.Y)) * step.Y;

        if (direction.Z > 0)
            initialStep.Z = (MathF.Ceiling(start.Z) - start.Z) * step.Z;
        else
            initialStep.Z = (start.Z - MathF.Floor(start.Z)) * step.Z;

        Vector3 distanceToNext = initialStep;
        var voxelPos = new Vector3(MathF.Floor(start.X), MathF.Floor(start.Y), MathF.Floor(start.Z));
        var lastPos = voxelPos;

        float lastDistanceToNext = distanceToNext.X;
        float lastStep = step.X;

        var hitVoxel = Voxels.Air;
        while (hitVoxel == Voxels.Air && lastDistanceToNext - lastStep < range)
        {
            lastPos = voxelPos;
            
            if (distanceToNext.X < distanceToNext.Y && distanceToNext.X < distanceToNext.Z)
            {
                distanceToNext.X += step.X;
                voxelPos.X += tileDirection.X;

                lastDistanceToNext = distanceToNext.X;
                lastStep = step.X;
            }
            else if (distanceToNext.Y < distanceToNext.X && distanceToNext.Y < distanceToNext.Z)
            {
                distanceToNext.Y += step.Y;
                voxelPos.Y += tileDirection.Y;

                lastDistanceToNext = distanceToNext.Y;
                lastStep = step.Y;
            }
            else
            {
                distanceToNext.Z += step.Z;
                voxelPos.Z += tileDirection.Z;

                lastDistanceToNext = distanceToNext.Z;
                lastStep = step.Z;
            }

            hitVoxel = world.GetVoxel(voxelPos);
        }

        return new Hit
        {
            Distance = lastDistanceToNext - lastStep,
            Voxel = hitVoxel,
            Pos = voxelPos,
            LastPos = lastPos
        };
    }
}