using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RocketJumper.Classes
{
    class Tools
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

        public static Vector2 GetRectangleIntersectionDepth(Rectangle rectangle1, Rectangle rectangle2)
        {
            Rectangle intersection = Rectangle.Intersect(rectangle1, rectangle2);
            if (intersection.IsEmpty)
                return Vector2.Zero;

            return new Vector2(intersection.Width, intersection.Height);
        }

        public static Rectangle RectangleMove(Rectangle rectangle, Vector2 position)
        {
            rectangle.X = (int)position.X;
            rectangle.Y = (int)position.Y;
            return rectangle;
        }
    }
}