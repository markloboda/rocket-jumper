using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RocketJumper.Classes
{

    enum TileCollision
    {
        Passable = 0,
        Impassable = 1,
        Platform = 2
    }

    struct Tile
    {
        public const int Width = 32;
        public const int Height = 32;

        public Texture2D Texture;
        Vector2 Position;
        public TileCollision Collision;
        public static readonly Vector2 Size = new Vector2(Width, Height);
        public Rectangle BoundingBox;

        public Tile(Texture2D texture, Vector2 tilePosition, TileCollision collision)
        {
            Texture = texture;
            Collision = collision;
            Position = tilePosition;

            if (Collision == TileCollision.Impassable)
            {
                BoundingBox = new Rectangle((int)Position.X * Width, (int)Position.Y * Height, Width, Height);
            }
            else
            {
                BoundingBox = new Rectangle(0, 0, 0, 0);
            }
        }
    }
}