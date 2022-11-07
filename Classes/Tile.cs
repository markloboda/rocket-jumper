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
        public TileCollision Collision;
        public static readonly Vector2 size = new Vector2(Width, Height);

        public Tile(Texture2D texture, TileCollision collision)
        {
            Texture = texture;
            Collision = collision;
        }
    }
}