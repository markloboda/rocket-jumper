using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RocketJumper.Classes.Controls
{
    public abstract class Component
    {
        public abstract void Update(GameTime gameTime);

        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}