using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace RocketJumper.Classes
{
    class Player
    {
        private Animation idleAnimation;
        private Animation runAnimation;

        public Level Level
        {
            get { return level; }
        }
        Level level;
        
        public Player(Level level, Vector2 position)
        {
            this.level = level;

            LoadContent();
        }
        
        public void LoadContent()
        {
            // Load animations.
            idleAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Idle"), 0.1f, true, 5);
        }
        
    }
}