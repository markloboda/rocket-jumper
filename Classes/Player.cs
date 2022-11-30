using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using RocketJumper.Classes.MapData;
using System.Collections.Generic;

namespace RocketJumper.Classes
{
    class Player
    {
        private Animation idleAnimation;
        private Animation runAnimation;

        private float horizontalSpeed = 300.0f;

        public int Height;
        public int Width;

        SpriteEffects flipEffect = SpriteEffects.None;

        public float PlayerSizeScale
        {
            get { return playerSizeScale; }
            set { playerSizeScale = value; }
        }
        private float playerSizeScale = 2.5f;

        // movement vars
        private float inputMovement;

        public Level Level;

        public Physics Physics;



        public Player(Level level, Vector2 position)
        {
            this.Level = level;

            LoadContent();

            Physics = new Physics(position, this.Level);
            Physics.BoundingBox = new Rectangle((int)position.X, (int)position.Y, Width, Height);
            Physics.IsBoundingBoxVisible = true;
        }




        public void LoadContent()
        {
            // Load animations.
            idleAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Idle"), 0.2f, true, 5, playerSizeScale);
            runAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Run"), 0.2f, true, 4, playerSizeScale);

            // Calculate bounds within texture size.
            Width = (int)(idleAnimation.FrameWidth * playerSizeScale);
            Height = (int)(idleAnimation.FrameHeight * playerSizeScale);
        }

        private void HandleInputs(KeyboardState keyboardState, GamePadState gamePadState)
        {
            // gamepad input

            // keyboard input
            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
                inputMovement = -1.0f;
            else if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
                inputMovement = 1.0f;
            else
                inputMovement = 0.0f;
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState, MouseState mouseState, GamePadState gamePadState)
        {
            HandleInputs(keyboardState, gamePadState);

            // add horizontal movement
            Physics.AddMovement(gameTime, new Vector2(inputMovement, 0.0f) * horizontalSpeed);
            Physics.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // flip if necessary
            if (inputMovement < 0)
            {
                flipEffect = SpriteEffects.FlipHorizontally;
            }
            else if (inputMovement > 0)
            {
                flipEffect = SpriteEffects.None;
            }

            // choose and draw right animation
            if (inputMovement == 0)
            {
                idleAnimation.StartAnimation();
                idleAnimation.Draw(gameTime, spriteBatch, Physics.Position, flipEffect);
            }
            else
            {
                runAnimation.StartAnimation();
                runAnimation.Draw(gameTime, spriteBatch, Physics.Position, flipEffect);
            }

            if (this.Physics.IsBoundingBoxVisible)
            {
                this.Physics.DrawBoundingBox(gameTime, spriteBatch);
            }

        }
    }
}