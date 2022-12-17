using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RocketJumper.Classes.MapData;
using System;
using System.Collections.Generic;

namespace RocketJumper.Classes
{
    public class Player
    {
        public AnimatedSprite PlayerSprite;

        // movement vars
        private float inputMovement;
        private float horizontalSpeed = 300.0f;
        private Vector2 jumpingForce = new Vector2(0.0f, -200.0f);

        // components
        public Level Level;

        public List<Sprite> Items = new();           // list of mapobject that draw onto the player

        // bazooka
        public Sprite Bazooka;
        public bool HasBazooka = false;
        public bool HasRocket = true;
        public const int FireRate = 1000;           // time between shots in milliseconds
        public int FireTimer = FireRate;
        public List<Rocket> RocketList = new();

        // inputs
        KeyboardState keyboardState;
        MouseState mouseState;
        GamePadState gamePadState;

        // other
        public const float PlayerSizeScale = 2.5f;

        public Player(AnimatedSprite playerSprite)
        {
            PlayerSprite = playerSprite;
            Level = PlayerSprite.Level;
        }

        public void Update(GameTime gameTime)
        {
            FireTimer -= gameTime.ElapsedGameTime.Milliseconds;

            HandleInputs();
            CheckItemCollision();

            // rockets
            for (int i = 0; i < RocketList.Count; i++)
            {
                RocketList[i].Update(gameTime);
                if (RocketList[i].Collided)
                {
                    RocketList.RemoveAt(i--);
                }
            }

            // add horizontal movement
            PlayerSprite.AddInputToPhysics(gameTime, new Vector2(inputMovement, 0.0f) * horizontalSpeed);
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

        private void HandleInputs()
        {
            GetInputs();

            // gamepad input

            //
            // keyboard and mouse input
            //

            // basic moving
            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
                inputMovement = -1.0f;
            else if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
                inputMovement = 1.0f;
            else
                inputMovement = 0.0f;

            // jumping
            if ((keyboardState.IsKeyDown(Keys.Space) || keyboardState.IsKeyDown(Keys.W)) && PlayerSprite.Physics.BottomCollision)
                PlayerSprite.Physics.AddTempForce(jumpingForce);

            // bazooka
            if (HasBazooka)
            {
                Vector2 mousePosition = mouseState.Position.ToVector2();
                Vector2 playerPosition = PlayerSprite.Physics.Position + new Vector2(PlayerSprite.Physics.Size.X / 2, PlayerSprite.Physics.Size.Y / 2);
                Vector2 direction = mousePosition - playerPosition;

                // take into account the transformation of the camera
                direction = Vector2.Transform(direction, Matrix.Invert(Level.CameraTransform));
                direction.Normalize();

                float angle = MathF.Atan2(direction.Y, direction.X);
                Bazooka.Physics.Rotation = angle;

                // shooting
                if (HasRocket && FireTimer <= 0 && mouseState.LeftButton == ButtonState.Pressed)
                {
                    RocketList.Add(new Rocket(PlayerSprite.Physics.Position, direction, Level)); ;
                    FireTimer = FireRate;
                }
            }
        }

        private void CheckItemCollision()
        {
            foreach (Sprite item in Level.ItemSprites)
            {
                if (item.Physics.BoundingBox.Intersects(PlayerSprite.Physics.BoundingBox))
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

            Level.ItemSprites.Remove(item);
            Items.Add(item);
            PlayerSprite.AddChild(item);
        }

        private void GetInputs()
        {
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();
            gamePadState = GamePad.GetState(PlayerIndex.One);
        }

    }
}