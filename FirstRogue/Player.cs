using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FirstRogue;

public class Player
{
    private const float MouseSens = 0.002f;
    private readonly float minLookRad = MathHelper.ToRadians(-89f), maxLookRad = MathHelper.ToRadians(89f);

    private float lookY = MathF.PI, lookX;

    private readonly float speed = 4f;
    public Vector3 Pos { get; private set; } = new(-4f, 0.5f, -3f);
    public Vector3 Size { get; } = new(0.8f);

    public void Update(float deltaTime, Input input, VoxelChunk chunk)
    {
        Vector2 mouseDelta = input.MouseDelta;
        lookX += mouseDelta.Y * MouseSens;
        lookY -= mouseDelta.X * MouseSens;

        lookX = Math.Clamp(lookX, minLookRad, maxLookRad);
        lookY = MathHelper.WrapAngle(lookY);

        Vector3 moveDir = Vector3.Zero;

        if (input.IsKeyDown(Keys.A)) moveDir.X -= 1f;

        if (input.IsKeyDown(Keys.D)) moveDir.X += 1f;

        if (input.IsKeyDown(Keys.W)) moveDir.Z += 1f;

        if (input.IsKeyDown(Keys.S)) moveDir.Z -= 1f;
        
        if (input.IsKeyDown(Keys.Space)) moveDir.Y += 1f;

        if (input.IsKeyDown(Keys.LeftShift)) moveDir.Y -= 1f;

        Move(moveDir, chunk, deltaTime);

        if (input.WasMouseButtonPressed(MouseButtons.Left))
        {
            Hit hit = Raycast.Cast(chunk, Pos, GetLookVector(), 10f);
            chunk.SetVoxel(hit.Pos, Voxels.Air);
        }
    }

    public Vector3 GetLookVector()
    {
        var yRot = Matrix.CreateRotationY(MathHelper.WrapAngle(lookY));
        var xRot = Matrix.CreateRotationX(Math.Clamp(lookX, minLookRad, maxLookRad));
        return Vector3.Transform(Vector3.UnitZ, xRot * yRot);
    }

    public Matrix GetViewMatrix()
    {
        Vector3 look = GetLookVector();
        Vector3 lookAt = Pos + look;
        var view = Matrix.CreateLookAt(Pos, lookAt, Vector3.Up);

        return view;
    }

    public void Move(Vector3 direction, VoxelChunk chunk, float deltaTime)
    {
        if (direction.Length() != 0) direction.Normalize();

        direction *= deltaTime * speed;

        float forwardX = MathF.Sin(lookY);
        float forwardZ = MathF.Cos(lookY);

        float rightDir = lookY - MathF.PI * 0.5f;
        float rightX = MathF.Sin(rightDir);
        float rightZ = MathF.Cos(rightDir);

        Vector3 newPos = Pos;

        newPos.X += rightX * direction.X;
        newPos.X += forwardX * direction.Z;

        if (IsCollidingWithVoxel(newPos, chunk)) newPos.X = Pos.X;

        newPos.Z += rightZ * direction.X;
        newPos.Z += forwardZ * direction.Z;

        if (IsCollidingWithVoxel(newPos, chunk)) newPos.Z = Pos.Z;

        newPos.Y += direction.Y;
        
        if (IsCollidingWithVoxel(newPos, chunk)) newPos.Y = Pos.Y;

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

            if (chunk.GetVoxel(cornerPos) != Voxels.Air) return true;
        }

        return false;
    }
}