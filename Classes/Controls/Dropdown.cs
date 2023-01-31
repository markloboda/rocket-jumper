using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RocketJumper.Classes.Controls
{
    public class Dropdown : Component
    {
        private MouseState currentMouseState;
        private MouseState previousMouseState;
        public Vector2 Position;
        public List<string> Options;
        public int SelectedIndex = 0;
        public Vector2 CenterOffset
        {
            get { return new Vector2(itemWidth / 2, itemHeight / 2); }
        }

        private bool isExpanded;
        private int itemHeight;
        private int itemWidth;
        private SpriteFont font;
        private Texture2D texture;
        private bool isHovering;
        private int hoveringIndex = -1;

        public EventHandler SelectChange;

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)(Position.X - CenterOffset.X), (int)(Position.Y - CenterOffset.Y), (int)(itemWidth), (int)(itemHeight));
            }
        }

        public Dropdown(Texture2D texture2D, SpriteFont font)
        {
            this.isExpanded = false;
            this.itemHeight = 50;
            this.itemWidth = 200;
            this.font = font;
            this.texture = texture2D;
        }

        public override void Update(GameTime gameTime)
        {
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();


            var dropdownRect = new Rectangle((int)(Position.X - CenterOffset.X), (int)(Position.Y - CenterOffset.Y), itemWidth, itemHeight);
            var mouseRectangle = new Rectangle(currentMouseState.X, currentMouseState.Y, 1, 1);

            isHovering = false;
            if (mouseRectangle.Intersects(dropdownRect))
            {
                isHovering = true;
                if (currentMouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed)
                {
                    isExpanded = !isExpanded;
                }
            }
            else if (isExpanded)
            {
                if (currentMouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed)
                {
                    isExpanded = false;
                    for (int i = 0; i < Options.Count; i++)
                    {
                        Rectangle itemRect = new Rectangle((int)(Position.X - CenterOffset.X), (int)(Position.Y + (itemHeight * (i + 1)) - CenterOffset.Y), itemWidth, itemHeight);
                        if (mouseRectangle.Intersects(itemRect))
                        {
                            SelectedIndex = i;
                            SelectChange?.Invoke(this, new EventArgs());
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < Options.Count; i++)
                    {
                        Rectangle itemRect = new Rectangle((int)(Position.X - CenterOffset.X), (int)(Position.Y + (itemHeight * (i + 1)) - CenterOffset.Y), itemWidth, itemHeight);
                        if (mouseRectangle.Intersects(itemRect))
                        {
                            hoveringIndex = i;
                            break;
                        }
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var textColor = Color.Black;
            var backgroundColor = Color.White;
            var hoverColor = Color.LightGray;

            if (isHovering)
                spriteBatch.Draw(texture, Bounds, hoverColor);
            else
                spriteBatch.Draw(texture, Bounds, backgroundColor);

            Vector2 textSize = font.MeasureString(Options[SelectedIndex]);
            Vector2 centerTextOffset = new Vector2(textSize.X / 2, textSize.Y / 2);
            Vector2 textPosition = new Vector2(Bounds.X + itemWidth / 2, Bounds.Y + itemHeight / 2) - centerTextOffset;
            spriteBatch.DrawString(font, Options[SelectedIndex], textPosition, textColor);

            if (isExpanded)
            {
                for (int i = 0; i < Options.Count; i++)
                {
                    Rectangle itemRect = new Rectangle((int)(Position.X - CenterOffset.X), (int)(Position.Y - CenterOffset.Y) + (itemHeight * (i + 1)), itemWidth, itemHeight);
                    if (i == hoveringIndex)
                        spriteBatch.Draw(texture, itemRect, null, hoverColor, 0, Vector2.Zero, SpriteEffects.None, 0);
                    else
                        spriteBatch.Draw(texture, itemRect, null, backgroundColor, 0, Vector2.Zero, SpriteEffects.None, 0);

                    Vector2 itemTextSize = font.MeasureString(Options[i]);
                    Vector2 itemTextCenterOffset = new Vector2(itemTextSize.X / 2, itemTextSize.Y / 2);
                    Vector2 itemTextPosition = new Vector2(itemRect.X + itemRect.Width / 2, itemRect.Y + itemRect.Height / 2) - itemTextCenterOffset;
                    spriteBatch.DrawString(font, Options[i], itemTextPosition, Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                }
            }
        }

    }
}
