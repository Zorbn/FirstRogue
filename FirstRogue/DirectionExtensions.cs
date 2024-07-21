using System;

namespace FirstRogue;

public static class DirectionExtensions
{
    public static IVector3 ToVec(this Directions direction)
    {
        return direction switch
        {
            Directions.Forward => IVector3.Forward,
            Directions.Backward => IVector3.Backward,
            Directions.Left => IVector3.Left,
            Directions.Right => IVector3.Right,
            Directions.Up => IVector3.Up,
            Directions.Down => IVector3.Down,
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