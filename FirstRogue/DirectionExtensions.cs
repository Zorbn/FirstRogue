using System;
using Microsoft.Xna.Framework;

namespace FirstRogue;

public static class DirectionExtensions
{
    public static Vector3 ToVec(this Directions direction)
    {
        return direction switch
        {
            Directions.Forward => Vector3.Forward,
            Directions.Backward => Vector3.Backward,
            Directions.Left => Vector3.Left,
            Directions.Right => Vector3.Right,
            Directions.Up => Vector3.Up,
            Directions.Down => Vector3.Down,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }

    public static int OutwardComponentI(this Directions direction)
    {
        return direction switch
        {
            Directions.Forward => 2,
            Directions.Backward => 2,
            Directions.Left => 0,
            Directions.Right => 0,
            Directions.Up => 1,
            Directions.Down => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }
}