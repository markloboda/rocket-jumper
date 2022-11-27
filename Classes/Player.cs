using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using RocketJumper.Classes.MapData;

namespace RocketJumper.Classes
{
    class Player
    {
        private Animation idleAnimation;
        private Animation runAnimation;

        private Physics physics;
        private float speed = 300.0f;

        public int Height { get { return height; } }
        private int height = 26;
        public int Width { get { return width; } }
        private int width = 16;

        SpriteEffects flipEffect = SpriteEffects.None;

        public float PlayerSizeScale
        {
            get { return playerSizeScale; }
            set { playerSizeScale = value; }
        }
        float playerSizeScale = 3.0f;

        // movement vars
        private float inputMovement;

        public Level Level
        {
            get { return level; }
        }
        Level level;

        public Physics Physics
        {
            get { return physics; }
            set { physics = value; }
        }



        public Player(Level level, Vector2 position, Tile[] collidables)
        {
            this.level = level;

            // offset position
            position.Y -= 26 * playerSizeScale;


            physics = new Physics(position, collidables);
            physics.BoundingBox = new Rectangle((int)position.X, (int)position.Y, width * (int)playerSizeScale, height * (int)playerSizeScale);
            physics.IsBoundingBoxVisible = true;

            LoadContent();
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

            // add horizontal movement
            physics.AddMovement(gameTime, new Vector2(inputMovement, 0.0f) * speed);
            physics.Update(gameTime);
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
                idleAnimation.Draw(gameTime, spriteBatch, physics.Position, flipEffect);
            }
            else
            {
                runAnimation.StartAnimation();
                runAnimation.Draw(gameTime, spriteBatch, physics.Position, flipEffect);
            }

            if (this.Physics.IsBoundingBoxVisible)
            {
                this.Physics.DrawBoundingBox(gameTime, spriteBatch);
            }

        }
    }
}