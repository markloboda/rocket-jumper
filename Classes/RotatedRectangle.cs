using System;
using Microsoft.Xna.Framework;

namespace RocketJumper.Classes
{
    public class RotatedRectangle
    {
        // Position of the center of the bounding box
        public Vector2 Position;

        // Width and height of the bounding box
        public int Width;
        public int Height;

        // Rotation angle of the bounding box, in radians
        public int Rotation;

        public RotatedRectangle(int x, int y, int width, int height, int rotation)
        {
            Position.X = x;
            Position.Y = y;
            Width = width;
            Height = height;
            Rotation = rotation;
        }

        // Calculates the corners of the bounding box
        public Vector2[] GetCorners()
        {
            // Calculate the half-width and half-height of the bounding box
            float halfWidth = Width / 2;
            float halfHeight = Height / 2;

            // Calculate the four corners of the bounding box
            Vector2 topLeft = new Vector2(-halfWidth, -halfHeight);
            Vector2 topRight = new Vector2(halfWidth, -halfHeight);
            Vector2 bottomLeft = new Vector2(-halfWidth, halfHeight);
            Vector2 bottomRight = new Vector2(halfWidth, halfHeight);

            // Rotate the corners around the center of the bounding box
            topLeft = RotatePoint(topLeft, Rotation);
            topRight = RotatePoint(topRight, Rotation);
            bottomLeft = RotatePoint(bottomLeft, Rotation);
            bottomRight = RotatePoint(bottomRight, Rotation);

            // Offset the corners by the position of the bounding box
            topLeft += Position;
            topRight += Position;
            bottomLeft += Position;
            bottomRight += Position;

            // Return the corners as an array
            return new Vector2[] { topLeft, topRight, bottomRight, bottomLeft };
        }

        public bool Contains(Vector2 point)
        {
            // Rotate the point around the center of the bounding box
            point = RotatePoint(point - Position, -Rotation);

            // Check if the point is contained within the bounding box
            if (Math.Abs(point.X) <= Width / 2 && Math.Abs(point.Y) <= Height / 2)
            {
                return true;
            }
            return false;
        }

        public RotatedRectangle MoveTo(Vector2 position)
        {
            Position = position;
            return this;
        }

        // Rotates a point around the origin by a given angle
        private Vector2 RotatePoint(Vector2 point, float angle)
        {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);
            return new Vector2(point.X * cos - point.Y * sin, point.X * sin + point.Y * cos);
        }
    }
}
