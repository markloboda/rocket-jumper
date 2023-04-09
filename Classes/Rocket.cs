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
        public bool SideOfMapCollision = false;

        public Vector2 Direction;
        public Sprite TargetSprite;
        public bool PathFindingRocket = false;

        public float Speed = 500.0f;

        public Rocket(Vector2 position, Vector2 direction, GameState gameState, bool hitsPlayer = false)
        {
            this.gameState = gameState;
            this.Direction = direction;

            rocketAnimation = gameState.RocketAnimation;

            RocketSprite = new AnimatedSprite(
                new() { ["fly"] = rocketAnimation },
                position,
                gameState,
                "fly",
                isLooping: true,
                rotation: (float)Math.Atan2(direction.Y, direction.X),
                boundingBoxType: "RBB",
                origin: new Vector2(rocketAnimation.FrameWidth / 2, rocketAnimation.FrameHeight / 2)
                );

            RocketSprite.Physics.Velocity = Speed * this.Direction;

            RocketSprite.Physics.IsBoundingBoxVisible = false;

            HitsPlayer = hitsPlayer;
        }

        public Rocket(Vector2 position, Vector2 direction, GameState gameState, Sprite targetSprite, bool hitsPlayer = false)
        {
            this.gameState = gameState;
            this.Direction = direction;

            rocketAnimation = gameState.RocketAnimation;

            RocketSprite = new AnimatedSprite(
                new() { ["fly"] = rocketAnimation },
                position,
                gameState,
                "fly",
                isLooping: true,
                rotation: (float)Math.Atan2(direction.Y, direction.X)
                );
            RocketSprite.Physics.Velocity = Speed * this.Direction;

            RocketSprite.Physics.Origin = RocketSprite.Physics.GetLocalCenter();

            TargetSprite = targetSprite;
            PathFindingRocket = true;
            HitsPlayer = hitsPlayer;
        }

        public void Update(GameTime gameTime)
        {
            if (PathFindingRocket)
            {
                Direction = TargetSprite.Physics.GetGlobalCenter() - RocketSprite.Physics.GetGlobalCenter();
                Direction.Normalize();
                RocketSprite.Physics.Rotation = (float)Math.Atan2(Direction.Y, Direction.X);
                RocketSprite.Physics.Velocity = Speed * Direction;
            }

            RocketSprite.Update(gameTime);
            Collided = RocketSprite.Physics.Collided;
            SideOfMapCollision = RocketSprite.Physics.SideOfMapCollision;


            // check if player is hit
            if (HitsPlayer && RocketSprite.CollidesWith(gameState.Player.PlayerSprite))
                Collided = true;

            if (Collided && !SideOfMapCollision)
            {
                if (gameState.SoundEffects.ContainsKey("explosion"))
                    gameState.SoundEffects["explosion"].CreateInstance().Play();
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            RocketSprite.Draw(gameTime, spriteBatch);
        }
    }
}
