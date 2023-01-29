using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace RocketJumper.Classes
{
    public class Tools
    {
        public static void DrawRectangle(Rectangle rec, Color color, SpriteBatch spriteBatch)
        {
            // setup Texture2D for bounding box
            Texture2D recTexture = new Texture2D(spriteBatch.GraphicsDevice, rec.Width, rec.Height);
            Color[] data = new Color[rec.Width * rec.Height];
            for (int i = 0; i < rec.Width; ++i)
            {
                data[i] = Color.White;
                data[(rec.Height - 1) * rec.Width + i] = Color.White;
            }
            for (int i = 0; i < rec.Height; ++i)
            {
                data[i * rec.Width] = Color.White;
                data[i * rec.Width + rec.Width - 1] = Color.White;
            }
            recTexture.SetData(data);

            spriteBatch.Draw(recTexture, new Vector2(rec.X, rec.Y), color);
        }

        public static void DrawRectangle(RotatedRectangle rec, Color color, SpriteBatch spriteBatch)
        {
            // setup Texture2D for bounding box
            Texture2D recTexture = new Texture2D(spriteBatch.GraphicsDevice, rec.Width, rec.Height);
            Color[] data = new Color[rec.Width * rec.Height];
            for (int i = 0; i < rec.Width; ++i)
            {
                data[i] = Color.White;
                data[(rec.Height - 1) * rec.Width + i] = Color.White;
            }
            for (int i = 0; i < rec.Height; ++i)
            {
                data[i * rec.Width] = Color.White;
                data[i * rec.Width + rec.Width - 1] = Color.White;
            }
            recTexture.SetData(data);

            Vector2 origin = new Vector2(rec.Width / 2, rec.Height / 2);
            spriteBatch.Draw(
                texture: recTexture,
                position: new Vector2(rec.X, rec.Y) + origin,
                sourceRectangle: null,
                color: color,
                origin: origin,
                rotation: rec.Rotation,
                scale: 1,
                effects: SpriteEffects.None,
                layerDepth: 0);
        }

        public static Texture2D GetSingleColorTexture(GraphicsDevice graphicsDevice, Color color)
        {
            Texture2D texture = new Texture2D(graphicsDevice, 1, 1);
            texture.SetData(new Color[] { color });
            return texture;
        }

        public static Vector2 GetRectangleIntersectionDepth(Rectangle rectangle1, Rectangle rectangle2)
        {
            Rectangle intersection = Rectangle.Intersect(rectangle1, rectangle2);
            if (intersection.IsEmpty)
                return Vector2.Zero;

            return new Vector2(intersection.Width, intersection.Height);
        }

        public static Rectangle RectangleMoveTo(Rectangle rectangle, Vector2 position)
        {
            rectangle.X = (int)position.X;
            rectangle.Y = (int)position.Y;
            return rectangle;
        }

        public static Rectangle RectangleMoveBy(Rectangle rectangle, Vector2 delta)
        {
            rectangle.X += (int)delta.X;
            rectangle.Y += (int)delta.Y;
            return rectangle;
        }

        public static Vector2 RectangleCollisionDepth(Rectangle rec1, Rectangle rec2)
        {
            Vector2 depth = Vector2.Zero;

            if (rec1.Left < rec2.Right && rec1.Right > rec2.Left)
            {
                if (rec1.Top < rec2.Bottom && rec1.Bottom > rec2.Top)
                {
                    depth = new Vector2(
                        rec1.Left < rec2.Right ? rec1.Right - rec2.Left : rec1.Left - rec2.Right,
                        rec1.Top < rec2.Bottom ? rec1.Bottom - rec2.Top : rec1.Top - rec2.Bottom
                    );
                }
            }

            return depth;
        }
    }
}