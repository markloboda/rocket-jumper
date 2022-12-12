using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RocketJumper.Classes
{

    public class Rocket
    {
        private AnimatedSprite rocketAnimation;

        public Physics Physics;

        public bool Collided = false;

        private Level level;
        private Player player;
        private Vector2 direction;
        private float rotation;

        public Rocket(Vector2 position, Level level, Vector2 direction, Player player)
        {
            this.level = level;
            this.player = player;
            this.direction = direction;
            this.rotation = (float)Math.Atan2(direction.Y, direction.X);

            rocketAnimation = this.level.RocketAnimation;
            Physics = new Physics(position, new Vector2(rocketAnimation.FrameWidth, rocketAnimation.FrameHeight), this.level);
            Physics.AddBoundingBox();
            Physics.IsBoundingBoxVisible = true;
            Physics.Velocity = 1000 * direction;
        }

        public void Update(GameTime gameTime)
        {
            Physics.Update(gameTime);
            if (Physics.TopCollision || Physics.BottomCollision || Physics.LeftCollision || Physics.RightCollision)
            {
                Collided = true;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            rocketAnimation.StartAnimation();
            rocketAnimation.Draw(gameTime, spriteBatch, Physics.Position, SpriteEffects.None, rotation);
        }
    }
}
