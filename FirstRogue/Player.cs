using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FirstRogue;

public class Player
{
    public Vector3 Pos { get; private set; } = new(0, 0, 3);
    private float speed = 4f;
    
    private float lookY = MathF.PI, lookX;
    private readonly float minLookRad = MathHelper.ToRadians(-89f), maxLookRad = MathHelper.ToRadians(89f);
    private const float MouseSens = 0.002f;

    public void Update(float deltaTime, KeyboardState keyState, MouseState mouseState, Point windowCenter)
    {
        Vector2 delta = mouseState.Position.ToVector2() - windowCenter.ToVector2();
        Mouse.SetPosition(windowCenter.X, windowCenter.Y);

        lookX += delta.Y * MouseSens;
        lookY -= delta.X * MouseSens;

        lookX = Math.Clamp(lookX, minLookRad, maxLookRad);
        lookY = MathHelper.WrapAngle(lookY);
            
        float forwardX = MathF.Sin(lookY);
        float forwardZ = MathF.Cos(lookY);

        float rightDir = lookY - MathF.PI * 0.5f;
        float rightX = MathF.Sin(rightDir);
        float rightZ = MathF.Cos(rightDir);
            
        Vector2 moveDir = Vector2.Zero;
            
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

        if (moveDir.Length() != 0)
        {
            moveDir.Normalize();
        }

        moveDir *= deltaTime * speed;

        Vector3 newPos = Pos;
        
        newPos.X += rightX * moveDir.X;
        newPos.Z += rightZ * moveDir.X;
        
        newPos.X += forwardX * moveDir.Y;
        newPos.Z += forwardZ * moveDir.Y;

        Pos = newPos;
    }

    public Matrix GetViewMatrix()
    {
        var yRot = Matrix.CreateRotationY(MathHelper.WrapAngle(lookY));
        var xRot = Matrix.CreateRotationX(Math.Clamp(lookX, minLookRad, maxLookRad));
        Vector3 lookAt = Pos + Vector3.Transform(Vector3.UnitZ, xRot * yRot);
        var view = Matrix.CreateLookAt(Pos, lookAt, Vector3.Up);

        return view;
    }
}