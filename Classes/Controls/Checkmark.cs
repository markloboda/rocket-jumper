using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RocketJumper.Classes.Controls
{
    public class Checkmark : Component
    {
        private MouseState currentMouseState;

        private SpriteFont font;

        private bool isHovering;

        private MouseState previousMouseState;

        private Texture2D texture;

        public EventHandler Click;
        public bool Checked;

        public bool Clicked { get; private set; }

        public Vector2 Position;

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)(Position.X - Origin.X), (int)(Position.Y - Origin.Y), (int)Size.X, (int)Size.Y);
            }
        }

        public Vector2 Origin
        {
            get
            {
                return new Vector2(Size.X / 2, Size.Y / 2);
            }
        }

        public Vector2 Size = new Vector2(40, 40);

        public Checkmark(GraphicsDevice graphicsDevice, SpriteFont font)
        {
            this.texture = Tools.GetSingleColorTexture(graphicsDevice, Color.LightGray);
            this.font = font;
        }

        public override void Update(GameTime gameTime)
        {
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            var mouseRectangle = new Rectangle(currentMouseState.X, currentMouseState.Y, 1, 1);

            isHovering = false;
            if (mouseRectangle.Intersects(Bounds))
            {
                isHovering = true;

                if (currentMouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed)
                {
                    Checked = !Checked;
                    Click?.Invoke(this, new EventArgs());
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var color = Color.White;

            if (isHovering)
                color = Color.Gray;

            spriteBatch.Draw(texture, Bounds, null, color, 0.0f, Vector2.Zero, SpriteEffects.None, 0.5f);
            if (Checked)
            {
                var x = (Bounds.X + (Bounds.Width / 2)) - (font.MeasureString("x").X / 2);
                var y = (Bounds.Y + (Bounds.Height / 2)) - (font.MeasureString("x").Y / 2);
                spriteBatch.DrawString(font, "x", new Vector2(x, y), Color.Black);
            }
        }
    }
}