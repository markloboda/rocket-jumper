using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RocketJumper.Classes
{

    class Rocket
    {
        private Animation rocketAnimation;

        public Physics Physics;

        public bool Collided = false;

        private Level level;
        private Player player;

        public Rocket(Vector2 position, Level level, Player player)
        {
            this.level = level;
            this.player = player;

            rocketAnimation = this.level.RocketAnimation;
            Physics = new Physics(position, new Vector2(rocketAnimation.FrameWidth, rocketAnimation.FrameHeight), this.level);
            Physics.AddBoundingBox();
            Physics.IsBoundingBoxVisible = true;
            Physics.AddForce(new Vector2(10.0f, 0.0f));
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
            rocketAnimation.Draw(gameTime, spriteBatch, Physics.Position, SpriteEffects.None);
            Physics.Draw(gameTime, spriteBatch);
        }
    }
}
