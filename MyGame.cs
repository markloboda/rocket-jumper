using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RocketJumper.Classes;
using System.IO;

namespace RocketJumper
{
    public class MyGame : Game
    {
        Level currentLevel;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public MyGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            LoadLevel("Content/Levels/Tutorial.txt");

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            currentLevel.Draw(null, spriteBatch);
            currentLevel.Player.Draw(gameTime, spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void LoadLevel(string filename)
        {
            using (Stream fileStream = TitleContainer.OpenStream(filename))
                currentLevel = new Level(Services, fileStream);
        }
    }
}