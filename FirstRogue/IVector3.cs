namespace FirstRogue;

public struct IVector3
{
    public static readonly IVector3 Up = new(0, 1, 0);
    public static readonly IVector3 Down = new(0, -1, 0);
    public static readonly IVector3 Right = new(1, 0, 0);
    public static readonly IVector3 Left = new(-1, 0, 0);
    public static readonly IVector3 Forward = new(0, 0, -1);
    public static readonly IVector3 Backward = new(0, 0, 1);

    public int X;
    public int Y;
    public int Z;

    public IVector3(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public static IVector3 operator +(IVector3 value1, IVector3 value2)
    {
        value1.X += value2.X;
        value1.Y += value2.Y;
        value1.Z += value2.Z;
        return value1;
    }
}