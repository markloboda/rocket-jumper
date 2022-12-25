using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RocketJumper.Classes.MapData;
using RocketJumper.Classes.States;

namespace RocketJumper.Classes
{

    public class Rocket
    {
        private Animation_s rocketAnimation;
        private GameState gameState;
        public bool HitsPlayer = false;

        public AnimatedSprite RocketSprite;

        public bool Collided = false;

        private Vector2 direction;
        public Sprite TargetSprite;
        public bool PathFindingRocket = false;

        public float Speed = 1000.0f;

        public Rocket(Vector2 position, Vector2 direction, GameState gameState, bool hitsPlayer = false)
        {
            this.gameState = gameState;
            this.direction = direction;

            rocketAnimation = gameState.RocketAnimation;

            RocketSprite = new AnimatedSprite(
                new() { ["fly"] = rocketAnimation },
                position,
                gameState,
                "fly",
                isLooping: true,
                rotation: (float)Math.Atan2(direction.Y, direction.X)
                );

            RocketSprite.Physics.Velocity = Speed * this.direction;

            RocketSprite.Physics.Origin = RocketSprite.Physics.GetLocalCenter();

            HitsPlayer = hitsPlayer;
        }

        public Rocket(Vector2 position, Vector2 direction, GameState level, Sprite targetSprite, bool hitsPlayer = false)
        {
            this.gameState = level;
            this.direction = direction;
            Speed = 500.0f;

            rocketAnimation = level.RocketAnimation;

            RocketSprite = new AnimatedSprite(
                new() { ["fly"] = rocketAnimation },
                position,
                level,
                "fly",
                isLooping: true,
                rotation: (float)Math.Atan2(direction.Y, direction.X)
                );
            RocketSprite.Physics.Velocity = Speed * this.direction;

            RocketSprite.Physics.Origin = RocketSprite.Physics.GetLocalCenter();

            TargetSprite = targetSprite;
            PathFindingRocket = true;
            HitsPlayer = hitsPlayer;
        }

        public void Update(GameTime gameTime)
        {
            if (PathFindingRocket)
            {
                direction = TargetSprite.Physics.Position - RocketSprite.Physics.Position;
                direction.Normalize();
                RocketSprite.Physics.Rotation = (float)Math.Atan2(direction.Y, direction.X);
                RocketSprite.Physics.Velocity = Speed * direction;
            }

            RocketSprite.Update(gameTime);
            if (RocketSprite.Physics.Collided)
                Collided = true;

            // check if player is hit
            if (HitsPlayer && RocketSprite.CollidesWith(gameState.Player.PlayerSprite))
                Collided = true;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            RocketSprite.Draw(gameTime, spriteBatch);
        }
    }
}
