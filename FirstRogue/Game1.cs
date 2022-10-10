using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FirstRogue
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private BasicEffect basicEffect;
        private Matrix world;
        private Matrix projection;

        private readonly RasterizerState rasterizerState = new()
        {
            CullMode = CullMode.CullCounterClockwiseFace
        };

        private Point windowCenter;

        private Player player;
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
            
            chunk = new DrawableVoxelChunk(GraphicsDevice, 16, 16, 16);
            chunk.VoxelChunk.GenerateTerrain(new Random());
            chunk.GenerateMesh();
            
            UpdateWindowCenter();
            LockMouse(true);

            world = Matrix.Identity + Matrix.CreateTranslation(0, 0, 0);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90f),
                GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height, 0.01f, 100f);

            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.World = world;
            basicEffect.Projection = projection;
            basicEffect.TextureEnabled = true;
            basicEffect.Texture = Texture2D.FromFile(GraphicsDevice, "Content/voxelTemplate.png");
            
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

            basicEffect.View = player.GetViewMatrix();

            GraphicsDevice.SetVertexBuffer(chunk.VertexBuffer);

            foreach (EffectPass currentTechniquePass in basicEffect.CurrentTechnique.Passes)
            {
                currentTechniquePass.Apply();
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, chunk.PrimitiveCount);
            }
            
            base.Draw(gameTime);
        }
    }
}