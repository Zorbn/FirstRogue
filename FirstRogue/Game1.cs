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
    private SpriteBatch spriteBatch;
    
    private BasicEffect voxelEffect;
    private AlphaTestEffect spriteEffect;
    private VertexBuffer spriteVertexBuffer;
    
    private Matrix world;
    private Matrix projection;

    private readonly RasterizerState rasterizerState = new()
    {
        CullMode = CullMode.CullCounterClockwiseFace
    };

    private Point windowCenter;

    private Player player;
    private List<Sprite> sprites = new();
    private DrawableVoxelChunk chunk;

    private bool isFocused;
        
    public Game1()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += OnResize;
    }

    private void OnResize(object sender, EventArgs eventArgs)
    {
        UpdateWindowCenter();
    }

    private void UpdateWindowCenter()
    {
        windowCenter.X = GraphicsDevice.Viewport.Width / 2;
        windowCenter.Y = GraphicsDevice.Viewport.Height / 2;
    }

    protected override void Initialize()
    {
        player = new Player();
        sprites.Add(new Sprite(new Vector3(-4, 0, -4)));
        sprites.Add(new Sprite(new Vector3(-5, 0, -5)));
            
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

        spriteVertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColorTexture), 6, BufferUsage.WriteOnly);
        spriteVertexBuffer.SetData(SpriteMesh.Mesh);

        GraphicsDevice.RasterizerState = rasterizerState;
        GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    private void LockMouse(bool isLocked)
    {
        Mouse.SetPosition(windowCenter.X, windowCenter.Y);
        IsMouseVisible = !isLocked;
        isFocused = isLocked;
    }

    private bool IsMouseInWindow(MouseState mouseState)
    {
        return mouseState.X >= 0 && mouseState.Y >= 0 && mouseState.X < graphics.PreferredBackBufferWidth &&
               mouseState.Y < graphics.PreferredBackBufferWidth;
    }

    protected override void Update(GameTime gameTime)
    {
        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
        KeyboardState keyState = Keyboard.GetState();
        MouseState preMouseState = Mouse.GetState();
            
        if (keyState.IsKeyDown(Keys.Escape))
        {
            LockMouse(false);
        }

        if (IsMouseInWindow(preMouseState) && preMouseState.LeftButton == ButtonState.Pressed)
        {
            LockMouse(true);
        }

        if (!isFocused) return;
            
        MouseState mouseState = Mouse.GetState();
            
        player.Update(deltaTime, keyState, mouseState, windowCenter);

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
        
        GraphicsDevice.SetVertexBuffer(spriteVertexBuffer);
        
        foreach (Sprite sprite in sprites)
        {
            spriteEffect.World = sprite.GetModelMatrix(player.Pos);
            
            foreach (EffectPass currentTechniquePass in spriteEffect.CurrentTechnique.Passes)
            {
                currentTechniquePass.Apply();
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, SpriteMesh.PrimitiveCount);
            }
        }
        
        base.Draw(gameTime);
    }
}