using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RocketJumper.Classes.MapData
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

        Vector2 Position;
        public TileCollision Collision;
        public static readonly Vector2 Size = new Vector2(Width, Height);
        public Rectangle BoundingBox;

        public Tile(Vector2 tilePosition, TileCollision collision)
        {
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