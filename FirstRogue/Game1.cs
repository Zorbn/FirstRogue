using System;
using System.Collections.Generic;
using FirstRogue.Gfx;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FirstRogue;

public class Game1 : Game
{
    private GraphicsDeviceManager graphics;
    // private SpriteBatch spriteBatch;
    
    private BasicEffect voxelEffect;
    private AlphaTestEffect spriteEffect;
    
    private Matrix world;
    private Matrix projection;

    private readonly RasterizerState rasterizerState = new()
    {
        CullMode = CullMode.CullCounterClockwiseFace
    };

    private Point windowCenter;

    private Player player;
    private readonly List<Sprite> sprites = new();
    private DrawableVoxelChunk chunk;

    // TODO: Refactor stuff like this into an Input class. Eg: Mouse delta, focusing/unfocusing, clicks...
    private bool isFocused;
    private Point lastMousePos;
        
    public Game1()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += OnResize;
        
        // Fullscreen:
        graphics.PreferredBackBufferWidth = 1920;
        graphics.PreferredBackBufferHeight = 1080;
        graphics.IsFullScreen = true;
        graphics.SynchronizeWithVerticalRetrace = false;
        IsFixedTimeStep = false;
    }

    private void OnResize(object sender, EventArgs eventArgs)
    {
        UpdateWindowCenter();
    }

    private void UpdateWindowCenter()
    {
        windowCenter.X = Window.ClientBounds.Width / 2;
        windowCenter.Y = Window.ClientBounds.Height / 2;
    }

    protected override void Initialize()
    {
        player = new Player();
        sprites.Add(new Sprite(GraphicsDevice, new Vector3(-4, 0, -4), 0, 0, 1, 1, 1));
        sprites.Add(new Sprite(GraphicsDevice, new Vector3(-5, 0, -5), 0, 1, 2, 2, 2));
            
        chunk = new DrawableVoxelChunk(GraphicsDevice, 16, 16, 16);
        chunk.VoxelChunk.GenerateTerrain(new Random());
        chunk.GenerateMesh();
            
        UpdateWindowCenter();
        LockMouse(true);

        world = Matrix.Identity + Matrix.CreateTranslation(0, 0, 0);
        projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90f),
            GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height, 0.01f, 100f);

        voxelEffect = new BasicEffect(GraphicsDevice);
        voxelEffect.World = world;
        voxelEffect.Projection = projection;
        voxelEffect.TextureEnabled = true;
        voxelEffect.Texture = Texture2D.FromFile(GraphicsDevice, "Content/voxelTemplate.png");
        voxelEffect.VertexColorEnabled = true;
        
        spriteEffect = new AlphaTestEffect(GraphicsDevice);
        spriteEffect.World = world;
        spriteEffect.Projection = projection;
        spriteEffect.Texture = Texture2D.FromFile(GraphicsDevice, "Content/entityAtlas.png");
        spriteEffect.VertexColorEnabled = true;

        GraphicsDevice.RasterizerState = rasterizerState;
        GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        // spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    private void LockMouse(bool isLocked)
    {
        Mouse.SetPosition(windowCenter.X, windowCenter.Y);
        IsMouseVisible = !isLocked;
        isFocused = isLocked;
    }

    private bool IsMouseInWindow(MouseState mouseState)
    {
        return IsActive && mouseState.X >= 0 && mouseState.Y >= 0 && mouseState.X < Window.ClientBounds.Width &&
               mouseState.Y < Window.ClientBounds.Height;
    }

    protected override void Update(GameTime gameTime)
    {
        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        KeyboardState keyState = Keyboard.GetState();
        MouseState mouseState = Mouse.GetState();

        if (isFocused)
        {
            player.Update(deltaTime, keyState, mouseState, lastMousePos, chunk.VoxelChunk);

            if (Mouse.GetState().Position != windowCenter)
            {
                Mouse.SetPosition(windowCenter.X, windowCenter.Y);
            }
        }
        
        if (isFocused && keyState.IsKeyDown(Keys.Escape))
        {
            LockMouse(false);
        }
        
        if (!isFocused && IsMouseInWindow(mouseState) && mouseState.LeftButton == ButtonState.Pressed)
        {
            LockMouse(true);
        }
        
        MouseState postMouseState = Mouse.GetState();

        lastMousePos = postMouseState.Position;
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        Matrix view = player.GetViewMatrix();
        voxelEffect.View = view;
        spriteEffect.View = view;
        
        GraphicsDevice.SetVertexBuffer(chunk.VertexBuffer);
        
        foreach (EffectPass currentTechniquePass in voxelEffect.CurrentTechnique.Passes)
        {
            currentTechniquePass.Apply();
            GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, chunk.PrimitiveCount);
        }
        
        foreach (Sprite sprite in sprites)
        {
            spriteEffect.World = sprite.GetModelMatrix(player.Pos);
            GraphicsDevice.SetVertexBuffer(sprite.VertexBuffer);

            foreach (EffectPass currentTechniquePass in spriteEffect.CurrentTechnique.Passes)
            {
                currentTechniquePass.Apply();
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, SpriteMesh.PrimitiveCount);
            }
        }
        
        base.Draw(gameTime);
    }
}