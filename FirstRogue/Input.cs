using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FirstRogue;

public class Input
{
    private Point lastMousePos;

    private KeyboardState previousKeyState, currentKeyState;
    private MouseState previousMouseState, currentMouseState;
    private Point windowCenter;

    public Input()
    {
        currentKeyState = Keyboard.GetState();
        previousKeyState = currentKeyState;
    }

    public bool IsFocused { get; private set; }
    public Vector2 MouseDelta { get; private set; }

    public void LockMouse(Game game, bool isLocked)
    {
        Mouse.SetPosition(windowCenter.X, windowCenter.Y);
        game.IsMouseVisible = !isLocked;
        IsFocused = isLocked;
    }

    public static bool IsMouseInWindow(Game game, MouseState mouseState)
    {
        return game.IsActive && mouseState.X >= 0 && mouseState.Y >= 0 &&
               mouseState.X < game.Window.ClientBounds.Width &&
               mouseState.Y < game.Window.ClientBounds.Height;
    }

    public void UpdateWindowCenter(Game game)
    {
        windowCenter.X = game.Window.ClientBounds.Width / 2;
        windowCenter.Y = game.Window.ClientBounds.Height / 2;
    }

    public void Update(Game game)
    {
        // Used to track the mouse regardless of if the window is focused, so that
        // the window can be refocused with a mouse click. For normal input, currentMouseState
        // or the relevant methods should be used.
        MouseState unfocusedMouseState = Mouse.GetState();

        previousKeyState = currentKeyState;
        previousMouseState = currentMouseState;

        // If IsFocused, collect new inputs.
        if (IsFocused)
        {
            currentKeyState = Keyboard.GetState();
            currentMouseState = unfocusedMouseState;

            MouseDelta = (currentMouseState.Position - lastMousePos).ToVector2();

            // Mouse delta, for now always lock the mouse when the window is focused:
            if (Mouse.GetState().Position != windowCenter) Mouse.SetPosition(windowCenter.X, windowCenter.Y);
        }
        else
        {
            currentKeyState = new KeyboardState();
            currentMouseState = new MouseState();
        }

        // Re-evaluate focus.
        if (IsFocused && IsKeyDown(Keys.Escape)) LockMouse(game, false);

        if (!IsFocused && IsMouseInWindow(game, unfocusedMouseState) &&
            unfocusedMouseState.LeftButton == ButtonState.Pressed)
            LockMouse(game, true);

        MouseState postMouseState = Mouse.GetState();

        lastMousePos = postMouseState.Position;
    }

    public bool IsKeyDown(Keys key)
    {
        return currentKeyState.IsKeyDown(key);
    }

    public bool WasKeyPressed(Keys key)
    {
        return currentKeyState.IsKeyDown(key) && !previousKeyState.IsKeyDown(key);
    }

    public bool IsMouseButtonDown(MouseButtons mouseButton)
    {
        return CheckMouseState(currentMouseState, mouseButton, ButtonState.Pressed);
    }

    public bool WasMouseButtonPressed(MouseButtons mouseButton)
    {
        return CheckMouseState(currentMouseState, mouseButton, ButtonState.Pressed) &&
               CheckMouseState(previousMouseState, mouseButton, ButtonState.Released);
    }

    private static bool CheckMouseState(MouseState mouseState, MouseButtons mouseButton, ButtonState buttonState)
    {
        return mouseButton switch
        {
            MouseButtons.Left => mouseState.LeftButton == buttonState,
            MouseButtons.Right => mouseState.RightButton == buttonState,
            MouseButtons.Middle => mouseState.MiddleButton == buttonState,
            _ => throw new ArgumentOutOfRangeException(nameof(mouseButton), mouseButton, null)
        };
    }
}