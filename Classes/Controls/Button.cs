using System;
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

        public Texture2D Texture;

        public EventHandler Click;
        public EventArgs EventArgs;

        public Color Color { get; set; } = Color.Transparent;
        public Color HoverColor { get; set; } = Color.Gray;

        public bool Clicked { get; private set; }

        public float Scale = 1.0f;

        public bool IsVisible = true;
        public bool IsDarkened = false;

        public Vector2 Origin
        {
            get
            {
                if (!string.IsNullOrEmpty(Text))
                    return new Vector2((font.MeasureString(Text).X * Scale) / 2, (font.MeasureString(Text).Y * Scale) / 2);
                else
                    return new Vector2((Texture.Width * Scale) / 2, (Texture.Height * Scale) / 2);
            }
        }

        public Vector2 Position;

        public Rectangle Bounds
        {
            get
            {
                if (!string.IsNullOrEmpty(Text))
                    return new Rectangle((int)(Position.X - Origin.X - Padding.X), (int)(Position.Y - Origin.Y - Padding.Y), (int)((font.MeasureString(Text).X + 2 * Padding.X) * Scale), (int)((font.MeasureString(Text).Y + 2 * Padding.Y) * Scale));
                else
                    return new Rectangle((int)(Position.X - Origin.X), (int)(Position.Y - Origin.Y), (int)(Texture.Width * Scale), (int)(Texture.Height * Scale));
            }
        }

        public Vector2 Padding = new Vector2(50, 10);
        public string Text;
        public SpriteEffects SpriteEffects = SpriteEffects.None;


        public Button(Texture2D texture, SpriteFont font)
        {
            this.Texture = texture;
            this.font = font;
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsVisible)
                return;

            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            var mouseRectangle = new Rectangle(currentMouseState.X, currentMouseState.Y, 1, 1);

            isHovering = false;
            if (mouseRectangle.Intersects(Bounds))
            {
                isHovering = true;

                if (currentMouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed)
                    Click?.Invoke(this, new EventArgs());
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            var color = Color;

            if (isHovering || IsDarkened)
                color = HoverColor;

            spriteBatch.Draw(Texture, Bounds, null, color, 0.0f, Vector2.Zero, SpriteEffects, 0.5f);

            if (!string.IsNullOrEmpty(Text))
            {
                var x = (Bounds.X + (Bounds.Width / 2)) - (font.MeasureString(Text).X / 2);
                var y = (Bounds.Y + (Bounds.Height / 2)) - (font.MeasureString(Text).Y / 2);

                spriteBatch.DrawString(font, Text, new Vector2(x, y), Color.Black);
            }
        }
    }
}