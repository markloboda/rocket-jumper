using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace RocketJumper.Classes
{
    class Player
    {
        private Animation idleAnimation;
        private Animation runAnimation;

        SpriteEffects flipEffect = SpriteEffects.None;

        private float playerSizeScale = 3.0f;

        // movement vars
        private float inputMovement;

        public Level Level
        {
            get { return level; }
        }
        Level level;

        public Vector2 Position
        {
            get { return position; }
        }
        Vector2 position;

        public Player(Level level, Vector2 position)
        {
            this.level = level;
            this.position = position;

            LoadContent();
        }
        
        private void ApplyMovement(GameTime gameTime)
        {
            // apply input movement
            Vector2 movement = new Vector2(inputMovement, 0.0f) * 100.0f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            position = position + movement;
        }


        public void LoadContent()
        {
            // Load animations.
            idleAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Idle"), 0.2f, true, 5, playerSizeScale);
            runAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Run"), 0.2f, true, 4, playerSizeScale);
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

            ApplyMovement(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // flip if necessary
            if (inputMovement < 0)
            {
                flipEffect = SpriteEffects.FlipHorizontally;
            } else if (inputMovement > 0)
            {
                flipEffect = SpriteEffects.None;
            }

            // choose and draw right animation
            if (inputMovement == 0) { 
                idleAnimation.StartAnimation();
                idleAnimation.Draw(gameTime, spriteBatch, position, flipEffect);
            } else
            {
                runAnimation.StartAnimation();
                runAnimation.Draw(gameTime, spriteBatch, position, flipEffect);
            }


        }
    }
}