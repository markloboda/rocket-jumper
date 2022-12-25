using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RocketJumper.Classes.Controls
{
    public class Button : Component
    {
        private MouseState currentMouseState;

        private SpriteFont font;

        private bool isHovering;

        private MouseState previousMouseState;

        private Texture2D texture;

        public EventHandler Click;

        public bool Clicked { get; private set; }

        public Vector2 Origin
        {
            get
            {
                if (!string.IsNullOrEmpty(Text))
                    return new Vector2((int)font.MeasureString(Text).X, (int)font.MeasureString(Text).Y);
                else
                    return new Vector2(texture.Width / 2, texture.Height / 2);
            }
        }

        public Vector2 Position { get; set; }

        public Rectangle Bounds
        {
            get
            {
                if (!string.IsNullOrEmpty(Text))
                    return new Rectangle((int)(Position.X - Origin.Y - Padding.X), (int)(Position.Y - Origin.Y - Padding.Y), (int)(font.MeasureString(Text).X + Padding.X), (int)(font.MeasureString(Text).Y + Padding.Y));
                else
                    return new Rectangle((int)(Position.X - Origin.Y - Padding.X), (int)(Position.Y - Origin.Y - Padding.Y), (int)(texture.Width + Padding.X), (int)(texture.Height + Padding.Y));
            }
        }

        public Vector2 Padding = new Vector2(50, 10);
        public string Text;


        public Button(Texture2D texture, SpriteFont font)
        {
            this.texture = texture;
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
                    Click.Invoke(this, new EventArgs());
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var color = Color.Transparent;

            if (isHovering)
                color = Color.Gray;

            spriteBatch.Draw(texture, Bounds, null, color, 0.0f, Vector2.Zero, SpriteEffects.None, 0);

            if (!string.IsNullOrEmpty(Text))
            {
                var x = (Bounds.X + (Bounds.Width / 2)) - (font.MeasureString(Text).X / 2);
                var y = (Bounds.Y + (Bounds.Height / 2)) - (font.MeasureString(Text).Y / 2);

                spriteBatch.DrawString(font, Text, new Vector2(x, y), Color.Black);
            }
        }
    }
}