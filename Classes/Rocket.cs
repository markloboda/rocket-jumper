using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RocketJumper.Classes.MapData;

namespace RocketJumper.Classes
{

    public class Rocket
    {
        private Animation_s rocketAnimation;

        public AnimatedSprite RocketSprite;

        public bool Collided = false;

        private Vector2 direction;
        private float rotation;

        public Rocket(Vector2 position, Vector2 direction, Level level)
        {
            this.direction = direction;
            rotation = (float)Math.Atan2(direction.Y, direction.X);

            rocketAnimation = level.RocketAnimation;

            RocketSprite = new AnimatedSprite(
                new() { ["fly"] = rocketAnimation },
                position,
                level,
                "fly",
                isLooping: true,
                rotation: rotation
                );

            RocketSprite.Physics.Velocity = 1000 * this.direction;

            RocketSprite.Physics.Origin = RocketSprite.Physics.GetLocalCenter();
        }

        public void Update(GameTime gameTime)
        {
            RocketSprite.Update(gameTime);
            if (RocketSprite.Physics.TopCollision || RocketSprite.Physics.BottomCollision || RocketSprite.Physics.LeftCollision || RocketSprite.Physics.RightCollision)
            {
                Collided = true;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            RocketSprite.Draw(gameTime, spriteBatch);
        }
    }
}
