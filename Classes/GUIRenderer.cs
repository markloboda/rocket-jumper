using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace RocketJumper.Classes
{
    public class GUIRenderer
    {
        protected SpriteBatch spriteBatch;

        private SpriteFont font;

        public int Timer = 0;
        public GUIRenderer(SpriteFont font, int timer = 0)
        {
            this.font = font;

            Timer = timer;
        }

        public void LoadContent()
        {
        }

        public void Update(GameTime gameTime)
        {
            Timer += gameTime.ElapsedGameTime.Milliseconds;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, "Time: " + Timer, new Vector2(10, 10), Color.White);
        }
    }
}