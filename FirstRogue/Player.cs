using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FirstRogue;

public class Player
{
    public Vector3 Pos { get; private set; } = new(-4f, 0.5f, -3f);
    public Vector3 Size { get; private set; } = new(0.8f);
    
    private float speed = 4f;
    
    private float lookY = MathF.PI, lookX;
    private readonly float minLookRad = MathHelper.ToRadians(-89f), maxLookRad = MathHelper.ToRadians(89f);
    private const float MouseSens = 0.002f;

    public void Update(float deltaTime, KeyboardState keyState, MouseState mouseState, Point lastMousePos, VoxelChunk chunk)
    {
        Vector2 delta = (mouseState.Position - lastMousePos).ToVector2();
        // Console.WriteLine($"{mouseState.Position}, {lastMousePos} -> {delta}");
        // Console.WriteLine($"{delta.X}");

        lookX += delta.Y * MouseSens;
        lookY -= delta.X * MouseSens;

        lookX = Math.Clamp(lookX, minLookRad, maxLookRad);
        lookY = MathHelper.WrapAngle(lookY);

        Vector3 moveDir = Vector3.Zero;
            
        if (keyState.IsKeyDown(Keys.A))
        {
            moveDir.X -= 1f;
        }
            
        if (keyState.IsKeyDown(Keys.D))
        {
            moveDir.X += 1f;
        }
            
        if (keyState.IsKeyDown(Keys.W))
        {
            moveDir.Y += 1f;
        }
            
        if (keyState.IsKeyDown(Keys.S))
        {
            moveDir.Y -= 1f;
        }
        
        Move(moveDir, chunk, deltaTime);
    }

    public Matrix GetViewMatrix()
    {
        var yRot = Matrix.CreateRotationY(MathHelper.WrapAngle(lookY));
        var xRot = Matrix.CreateRotationX(Math.Clamp(lookX, minLookRad, maxLookRad));
        Vector3 lookAt = Pos + Vector3.Transform(Vector3.UnitZ, xRot * yRot);
        var view = Matrix.CreateLookAt(Pos, lookAt, Vector3.Up);

        return view;
    }

    public void Move(Vector3 direction, VoxelChunk chunk, float deltaTime)
    {
        if (direction.Length() != 0)
        {
            direction.Normalize();
        }

        direction *= deltaTime * speed;
        
        float forwardX = MathF.Sin(lookY);
        float forwardZ = MathF.Cos(lookY);

        float rightDir = lookY - MathF.PI * 0.5f;
        float rightX = MathF.Sin(rightDir);
        float rightZ = MathF.Cos(rightDir);

        Vector3 newPos = Pos;
        
        newPos.X += rightX * direction.X;
        newPos.X += forwardX * direction.Y;
        
        if (IsCollidingWithVoxel(newPos, chunk))
        {
            newPos.X = Pos.X;
        }

        newPos.Z += rightZ * direction.X;
        newPos.Z += forwardZ * direction.Y;

        if (IsCollidingWithVoxel(newPos, chunk))
        {
            newPos.Z = Pos.Z;
        }

        Pos = newPos;
    }

    private bool IsCollidingWithVoxel(Vector3 at, VoxelChunk chunk)
    {
        for (var i = 0; i < 8; i++)
        {
            int xOff = i % 2 * 2 - 1;
            int yOff = i / 4 * 2 - 1;
            int zOff = i / 2 % 2 * 2 - 1;

            Vector3 cornerPos = at + new Vector3(Size.X * 0.5f * xOff, Size.Y * 0.5f * yOff, Size.Z * 0.5f * zOff);

            if (chunk.GetVoxel(cornerPos) != Voxels.Air)
            {
                return true;
            }
        }

        return false;
    }
}