using System;
using System.Collections.Generic;
using FirstRogue.Gfx;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FirstRogue;

public class Game1 : Game
{
    private readonly RasterizerState rasterizerState = new()
    {
        CullMode = CullMode.CullCounterClockwiseFace
    };

    private readonly List<Sprite> sprites = new();

    private readonly GraphicsDeviceManager graphics;
    private readonly Input input;

    private Random random;
    private DrawableWorld drawableWorld;
    private Player player;
    private Matrix projection;

    private AlphaTestEffect spriteEffect;
    // private SpriteBatch spriteBatch;

    private BasicEffect voxelEffect;

    private Matrix worldMatrix;

    public Game1()
    {
        graphics = new GraphicsDeviceManager(this);
        graphics.GraphicsProfile = GraphicsProfile.HiDef;
        Content.RootDirectory = "Content";
        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += OnResize;

        // Fullscreen:
        // graphics.PreferredBackBufferWidth = 1920;
        // graphics.PreferredBackBufferHeight = 1080;
        // graphics.IsFullScreen = true;
        // graphics.SynchronizeWithVerticalRetrace = false;
        // IsFixedTimeStep = false;

        input = new Input();
    }

    private void OnResize(object sender, EventArgs eventArgs)
    {
        input.UpdateWindowCenter(this);
    }

    protected override void Initialize()
    {
        random = new Random();
        
        player = new Player();
        sprites.Add(new Sprite(GraphicsDevice, new Vector3(-4, 0, -4), 0, 0, 1, 1, 1));
        sprites.Add(new Sprite(GraphicsDevice, new Vector3(-5, 0, -5), 0, 1, 2, 2, 2));

        drawableWorld = new DrawableWorld(GraphicsDevice, 2, 2, 2, 16, 16, 16);
        drawableWorld.World.GenerateWorld(random);

        input.UpdateWindowCenter(this);
        input.LockMouse(this, true);

        worldMatrix = Matrix.Identity + Matrix.CreateTranslation(0, 0, 0);
        projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90f),
            GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height, 0.01f, 100f);

        voxelEffect = new BasicEffect(GraphicsDevice);
        voxelEffect.World = worldMatrix;
        voxelEffect.Projection = projection;
        voxelEffect.TextureEnabled = true;
        voxelEffect.Texture = Texture2D.FromFile(GraphicsDevice, "Content/voxelTemplate.png");
        voxelEffect.VertexColorEnabled = true;

        spriteEffect = new AlphaTestEffect(GraphicsDevice);
        spriteEffect.World = worldMatrix;
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

    protected override void Update(GameTime gameTime)
    {
        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        input.Update(this);

        if (input.IsFocused)
        {
            player.Update(deltaTime, input, drawableWorld.World);

            drawableWorld.Update();
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        Matrix view = player.GetViewMatrix();       
        voxelEffect.View = view;
        spriteEffect.View = view;

        drawableWorld.Draw(GraphicsDevice, voxelEffect);

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