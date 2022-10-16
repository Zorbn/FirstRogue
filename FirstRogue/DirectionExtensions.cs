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
}