using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace RocketJumper.Classes
{
    class Player
    {
        private Animation idleAnimation;
        private Animation runAnimation;

        private float scale = 3.0f;

        public Level Level
        {
            get { return level; }
        }
        Level level;

        public Vector2 Position
        {
            get { return position; }
        }
        Vector2 position;

        public Player(Level level, Vector2 position)
        {
            this.level = level;
            this.position = position;

            LoadContent();
        }
        
        public void LoadContent()
        {
            // Load animations.
            idleAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Idle"), 0.2f, true, 5, 3.0f);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            idleAnimation.StartAnimation();
            // Draw that sprite.
            idleAnimation.Draw(gameTime, spriteBatch, position);
        }
    }
}