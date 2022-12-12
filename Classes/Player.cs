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

        private float horizontalSpeed = 300.0f;
        private Vector2 jumpingForce = new Vector2(0.0f, -200.0f);

        SpriteEffects characterFlipEffect = SpriteEffects.None;

        public const float PlayerSizeScale = 2.5f;

        // movement vars
        private float inputMovement;

        public Level Level;

        public Physics Physics;

        public List<MapObject> Items = new();           // list of mapobject that draw onto the player

        public MapObject Bazooka;
        public bool HasBazooka = false;
        public bool HasRocket = true;
        public const int FireRate = 1000;           // time between shots in milliseconds
        public int FireTimer = FireRate;
        public List<Rocket> RocketList = new();

        // inputs
        KeyboardState keyboardState;
        MouseState mouseState;
        GamePadState gamePadState;



        public Player(Level level, Vector2 position)
        {
            Level = level;
            Physics = new Physics(position, new Vector2(Level.PlayerIdleAnimation.FrameWidth * PlayerSizeScale, Level.PlayerIdleAnimation.FrameHeight * PlayerSizeScale), Level);
            Physics.AddBoundingBox();
            Physics.EnableGravity();
        }

        public void Update(GameTime gameTime)
        {
            FireTimer -= gameTime.ElapsedGameTime.Milliseconds;

            HandleInputs();
            CheckItemCollision();
            MoveItemsToPlayer();

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
            Physics.AddInputMovement(gameTime, new Vector2(inputMovement, 0.0f) * horizontalSpeed);
            Physics.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // flip if necessary
            if (inputMovement < 0)
                characterFlipEffect = SpriteEffects.FlipHorizontally;
            else if (inputMovement > 0)
                characterFlipEffect = SpriteEffects.None;

            // choose and draw right player animation
            if (inputMovement == 0)
                PlayAnimation(Level.PlayerIdleAnimation, Physics.Position, gameTime, spriteBatch);
            else
                PlayAnimation(Level.PlayerRunAnimation, Physics.Position, gameTime, spriteBatch);

            // items
            foreach (MapObject item in Items)
            {
                item.Draw(gameTime, spriteBatch);
            }

            // rockets
            foreach (Rocket rocket in RocketList)
                rocket.Draw(gameTime, spriteBatch);

            Physics.Draw(gameTime, spriteBatch);
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
            if ((keyboardState.IsKeyDown(Keys.Space) || keyboardState.IsKeyDown(Keys.W)) && Physics.BottomCollision)
                Physics.AddTempForce(jumpingForce);

            // bazooka
            if (HasBazooka)
            {
                Vector2 mousePosition = mouseState.Position.ToVector2();
                Vector2 playerPosition = Physics.Position + new Vector2(Physics.Size.X / 2, Physics.Size.Y / 2);
                Vector2 direction = mousePosition - playerPosition;

                // take into account the transformation of the camera
                direction = Vector2.Transform(direction, Matrix.Invert(Level.CameraTransform));
                direction.Normalize();

                float angle = MathF.Atan2(direction.Y, direction.X);
                Bazooka.Rotation = angle;

                // shooting
                if (HasRocket && FireTimer <= 0 && mouseState.LeftButton == ButtonState.Pressed)
                {
                    RocketList.Add(new Rocket(Physics.Position, Level, direction, this)); ;
                    FireTimer = FireRate;
                }
            }
        }

        private void CheckItemCollision()
        {
            foreach (MapObject item in Level.Items)
            {
                if (item.Physics.BoundingBox.Intersects(Physics.BoundingBox))
                {
                    if (item.Name == "Bazooka")
                    {
                        HasBazooka = true;
                        Level.Items.Remove(item);
                        Items.Add(item);
                        Bazooka = item;
                        break;
                    }
                }
            }
        }

        private void MoveItemsToPlayer()
        {
            foreach (MapObject item in Items)
            {
                item.Physics.MoveTo(Physics.Position);
                item.AddAttachmentOffset();
            }
        }

        private void PlayAnimation(AnimatedSprite animation, Vector2 position, GameTime gameTime, SpriteBatch spriteBatch)
        {
            animation.StartAnimation();
            animation.Draw(gameTime, spriteBatch, position, characterFlipEffect);
        }
        private void GetInputs()
        {
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();
            gamePadState = GamePad.GetState(PlayerIndex.One);
        }
    }
}