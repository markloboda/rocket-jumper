using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RocketJumper.Classes
{
    public class RotatedRectangle
    {
        // Position of the center of the bounding box
        public Vector2 Position;

        public float X
        {
            get { return Position.X; }
            set { Position.X = value; }
        }
        public float Y
        {
            get { return Position.Y; }
            set { Position.Y = value; }
        }

        // Width and height of the bounding box
        public int Width;
        public int Height;

        public float Left
        {
            get
            {
                float cos = (float)Math.Cos(Rotation);
                float sin = (float)Math.Sin(Rotation);
                float x1 = (cos * (-Width / 2 - Origin.X)) - (sin * (-Height / 2 - Origin.Y)) + Position.X;
                float x2 = (cos * (Width / 2 - Origin.X)) - (sin * (-Height / 2 - Origin.Y)) + Position.X;
                return Math.Min(x1, x2);
            }
        }

        public float Right
        {
            get
            {
                float cos = (float)Math.Cos(Rotation);
                float sin = (float)Math.Sin(Rotation);
                float x1 = (cos * (-Width / 2 - Origin.X)) - (sin * (-Height / 2 - Origin.Y)) + Position.X;
                float x2 = (cos * (Width / 2 - Origin.X)) - (sin * (-Height / 2 - Origin.Y)) + Position.X;
                return Math.Max(x1, x2);
            }
        }

        public float Top
        {
            get
            {
                float cos = (float)Math.Cos(Rotation);
                float sin = (float)Math.Sin(Rotation);
                float y1 = (sin * (-Width / 2 - Origin.X)) + (cos * (-Height / 2 - Origin.Y)) + Position.Y;
                float y2 = (sin * (Width / 2 - Origin.X)) + (cos * (-Height / 2 - Origin.Y)) + Position.Y;
                return Math.Min(y1, y2);
            }
        }

        public float Bottom
        {
            get
            {
                float cos = (float)Math.Cos(Rotation);
                float sin = (float)Math.Sin(Rotation);
                float y1 = (sin * (-Width / 2 - Origin.X)) + (cos * (Height / 2 - Origin.Y)) + Position.Y;
                float y2 = (sin * (Width / 2 - Origin.X)) + (cos * (Height / 2 - Origin.Y)) + Position.Y;
                return Math.Max(y1, y2);
            }
        }

        // Rotation angle of the bounding box, in radians
        public float Rotation;
        public Vector2 Origin;

        public RotatedRectangle(int x, int y, int width, int height, float rotation, Vector2 origin)
        {
            Position.X = x;
            Position.Y = y;
            Width = width;
            Height = height;
            Rotation = rotation;
            Origin = origin;
        }

        public RotatedRectangle MoveTo(Vector2 position)
        {
            Position = position;
            return this;
        }

        public RotatedRectangle RotateTo(float rotationRadinas)
        {
            Rotation = rotationRadinas;
            return this;
        }

        public Vector2[] GetVertices()
        {
            Vector2[] vertices = new Vector2[4];

            vertices[0] = new Vector2(-Width / 2, -Height / 2);
            vertices[1] = new Vector2(Width / 2, -Height / 2);
            vertices[2] = new Vector2(Width / 2, Height / 2);
            vertices[3] = new Vector2(-Width / 2, Height / 2);

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = Vector2.Transform(vertices[i], Matrix.CreateRotationZ(Rotation));
                vertices[i] += Position;
                vertices[i] += Origin;
            }

            return vertices;
        }

        public Vector2[] GetNormals()
        {
            Vector2[] vertices = GetVertices();
            Vector2[] normals = new Vector2[4];

            for (int i = 0; i < vertices.Length; i++)
            {
                int nextIndex = (i + 1) % vertices.Length;
                Vector2 edge = vertices[nextIndex] - vertices[i];
                normals[i] = new Vector2(-edge.Y, edge.X);
                normals[i].Normalize();
            }

            return normals;
        }



        public void DrawRectangle(Color color, SpriteBatch spriteBatch)
        {
            // setup Texture2D for bounding box
            Texture2D recTexture = new Texture2D(spriteBatch.GraphicsDevice, Width, Height);
            Color[] data = new Color[Width * Height];
            for (int i = 0; i < Width; ++i)
            {
                data[i] = Color.White;
                data[(Height - 1) * Width + i] = Color.White;
            }
            for (int i = 0; i < Height; ++i)
            {
                data[i * Width] = Color.White;
                data[i * Width + Width - 1] = Color.White;
            }
            recTexture.SetData(data);

            spriteBatch.Draw(
                texture: recTexture,
                position: new Vector2(X, Y) + Origin,
                sourceRectangle: null,
                color: color,
                origin: Origin,
                rotation: Rotation,
                scale: 1,
                effects: SpriteEffects.None,
                layerDepth: 0);
        }
    }
}
