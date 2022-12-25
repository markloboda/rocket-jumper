using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace RocketJumper.Classes.States
{
    public abstract class State
    {
        protected MyGame game;
        protected ContentManager content;

        public State(MyGame game, ContentManager content)
        {
            this.game = game;
            this.content = content;
        }

        public abstract void LoadContent();

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}