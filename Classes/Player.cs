using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RocketJumper.Classes.MapData;
using RocketJumper.Classes.States;
using System;
using System.Collections.Generic;

namespace RocketJumper.Classes
{
    public class Player
    {
        public AnimatedSprite PlayerSprite;

        // movement vars
        private float inputMovement;
        public static float HorizontalSpeed = 300;

        // components
        public GameState GameState
        {
            get { return PlayerSprite.GameState; }
        }

        public List<Sprite> Items = new();           // list of mapobject that draw onto the player

        // bazooka
        public Sprite Bazooka;
        public bool HasBazooka = false;
        public const int FireRate = 1000;           // time between shots in milliseconds
        public int FireTimer = FireRate;
        public const int ReloadRate = 3000;         // time between reloads in milliseconds
        public int ReloadTimer = ReloadRate;
        public int AmmoCount = 2;
        public List<Rocket> RocketList = new();
        public Vector2 ShootingPosition {
            get {
                return PlayerSprite.Physics.Position + Bazooka.AttachmentOrigin + new Vector2(0, 15);
            }
        }

        // other
        public const float PlayerSizeScale = 2.5f;

        public Player(AnimatedSprite playerSprite)
        {
            PlayerSprite = playerSprite;
            playerSprite.Physics.IsBoundingBoxVisible = true;
        }

        public void Update(GameTime gameTime)
        {
            FireTimer -= gameTime.ElapsedGameTime.Milliseconds;
            if (AmmoCount != 2)
                ReloadTimer -= gameTime.ElapsedGameTime.Milliseconds;

            // handle reload
            if (HasBazooka && AmmoCount != 2 && ReloadTimer <= 0)
                ReloadBazooka();

            HandleInputs();
            CheckItemCollision();

            // rockets
            for (int i = 0; i < RocketList.Count; i++)
            {
                RocketList[i].Update(gameTime);
                if (RocketList[i].Collided)
                {
                    // calcualte direction from rocket to player
                    Vector2 direction = PlayerSprite.Physics.GetGlobalCenter() - RocketList[i].RocketSprite.Physics.GetGlobalCenter();
                    // get length of direction and calculate force based on it where the closer the player is the more force is applied
                    float length = direction.Length();
                    float force = 10000.0f / length;
                    // normalize direction
                    direction.Normalize();
                    // add force to player
                    PlayerSprite.Physics.AddTempForce(direction * force);

                    RocketList.RemoveAt(i--);
                }
            }

            // add horizontal movement
            PlayerSprite.AddInputToPhysics(new Vector2(inputMovement, 0.0f) * HorizontalSpeed);
            PlayerSprite.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // flip if necessary
            if (inputMovement < 0)
                PlayerSprite.Effects = SpriteEffects.FlipHorizontally;
            else if (inputMovement > 0)
                PlayerSprite.Effects = SpriteEffects.None;

            // choose and draw right player animation
            if (inputMovement == 0)
            {
                if (PlayerSprite.CurrentAnimationId != "idle")
                    PlayerSprite.ChangeAnimation("idle");
            }
            else
            {
                if (PlayerSprite.CurrentAnimationId != "run")
                    PlayerSprite.ChangeAnimation("run");
            }

            // rockets
            foreach (Rocket rocket in RocketList)
                rocket.Draw(gameTime, spriteBatch);

            PlayerSprite.Draw(gameTime, spriteBatch);
        }

        public void ReloadBazooka()
        {
            AmmoCount = 2;
            ReloadTimer = ReloadRate;
        }

        private void HandleInputs()
        {
            // basic moving
            if (GameState.KeyboardState.IsKeyDown(Keys.A) || GameState.KeyboardState.IsKeyDown(Keys.Left))
                inputMovement = -1.0f;
            else if (GameState.KeyboardState.IsKeyDown(Keys.D) || GameState.KeyboardState.IsKeyDown(Keys.Right))
                inputMovement = 1.0f;
            else
                inputMovement = 0.0f;

            // bazooka
            if (HasBazooka)
            {
                Vector2 mousePosition = GameState.MouseState.Position.ToVector2();
                Vector2 playerPosition = PlayerSprite.Physics.GetGlobalCenter();
                Vector2 direction = mousePosition - playerPosition;

                // take into account the transformation of the camera
                direction = Vector2.Transform(direction, Matrix.Invert(GameState.CameraTransform));
                direction.Normalize();

                float angle = MathF.Atan2(direction.Y, direction.X);
                Bazooka.Physics.Rotation = angle;
                
                // if bazooka is facing left, flip it
                if (angle > MathF.PI / 2 || angle < -MathF.PI / 2)
                    Bazooka.Effects = SpriteEffects.FlipVertically;
                else
                    Bazooka.Effects = SpriteEffects.None;

                // shooting
                if (AmmoCount > 0 && FireTimer <= 0 && GameState.MouseState.LeftButton == ButtonState.Pressed)
                {
                    Rocket rocket = new Rocket(ShootingPosition, direction, GameState);
                    rocket.RocketSprite.Physics.Origin = new Vector2(rocket.RocketSprite.Physics.Width / 2, rocket.RocketSprite.Physics.Height / 2);
                    RocketList.Add(rocket);
                    FireTimer = FireRate;
                    AmmoCount--;

                    // play sound
                    GameState.SoundEffects["woosh"].CreateInstance().Play();
                }
            }
        }

        private void CheckItemCollision()
        {
            foreach (Sprite item in GameState.ItemSprites)
            {
                if (item.Physics.AABB.Intersects(PlayerSprite.Physics.AABB))
                {
                    AddItemToPlayer(item);
                    break;
                }
            }
        }

        private void AddItemToPlayer(Sprite item)
        {
            if (item.Name == "Bazooka")
            {
                Bazooka = item;
                HasBazooka = true;
                Bazooka.AttachmentOrigin = new Vector2(PlayerSprite.Physics.Width / 2, Bazooka.AttachmentOffset.Y);
            }

            GameState.ItemSprites.Remove(item);
            Items.Add(item);
            PlayerSprite.AddChild(item);
        }
    }
}