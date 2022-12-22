using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RocketJumper.Classes;

namespace RocketJumper
{
    public class MyGame : Game
    {
        Gameplay currentLevel;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private GUIRenderer GUIRenderer;
        private SpriteBatch GUIBatch;

        public static int ScreenWidth;
        public static int ScreenHeight;
        private Camera camera;

        public MyGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;

            // screen properties
            ScreenWidth = graphics.PreferredBackBufferWidth;
            ScreenHeight = graphics.PreferredBackBufferHeight;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            GUIRenderer = new GUIRenderer(Content.Load<SpriteFont>("Fonts/Font"));

            LoadGame("Content/Levels/test-map-2.json");
            camera = new Camera(currentLevel.Map.Width * currentLevel.Map.TileWidth);

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // fullscreen
            if (Keyboard.GetState().IsKeyDown(Keys.F11))
            {
                graphics.ToggleFullScreen();
            }


            camera.Follow(currentLevel.Player.PlayerSprite);
            currentLevel.CameraTransform = camera.Transform;
            currentLevel.Update(gameTime);
            GUIRenderer.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // game
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera.Transform);
            currentLevel.Draw(gameTime, spriteBatch);
            spriteBatch.End();

            // GUI
            spriteBatch.Begin();
            GUIRenderer.Draw(gameTime, spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void LoadGame(string fileName)
        {
            currentLevel = new Gameplay(Services, fileName);
            currentLevel.LoadContent();
            currentLevel.AddGUIRenderer(GUIRenderer);
        }

        private void ToggleFullscreen()
        {
            graphics.IsFullScreen = !graphics.IsFullScreen;
            graphics.ApplyChanges();
        }
    }
}