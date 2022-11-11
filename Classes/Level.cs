using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RocketJumper.Classes
{
    class Level : IDisposable
    {
        private Tile[,] tiles;

        private Rectangle finishTileBounds;

        public Player Player
        {
            get { return player; }
        }
        Player player;

        private Vector2 start;


        // content
        public ContentManager Content
        {
            get { return content; }
        }
        ContentManager content;

        public Level(IServiceProvider serviceProvider, Stream fileStream)
        {
            content = new ContentManager(serviceProvider, "Content");

            LoadTiles(fileStream);

            player = new Player(this, start);
        }

        private Tile LoadTile(char tileType, int x, int y)
        {
            switch (tileType)
            {
                // empty:
                case '.':
                    return new Tile(null, TileCollision.Passable);

                // finish:
                case 'f':
                    return LoadFinishTile(x, y);

                // start:
                case 's':
                    return LoadStartTile(x, y);

                // normal block:
                case 'o':
                    return new Tile(Content.Load<Texture2D>("Tiles/Block"), TileCollision.Impassable);

                default:
                    throw new NotSupportedException("Unsupported tile type character '" + tileType + "' at position " + x + ", " + y + ".");
            }
        }

        private Tile LoadFinishTile(int x, int y)
        {
            finishTileBounds = GetTileBounds(x, y);
            return new Tile(Content.Load<Texture2D>("Tiles/Finish"), TileCollision.Passable);
        }

        private Tile LoadStartTile(int x, int y)
        {
            start = GetTileBounds(x, y).Center.ToVector2();
            return new Tile(null, TileCollision.Passable);
        }

        private void LoadTiles(Stream fileStream)
        {
            int levelWidth;
            List<string> lines = new List<string>();

            using (StreamReader sr = new StreamReader(fileStream))
            {
                string line = sr.ReadLine();
                levelWidth = line.Length;
                while (line != null)
                {
                    lines.Add(line);
                    if (line.Length != levelWidth)
                        throw new Exception("All lines in the level must be the same length.");
                    line = sr.ReadLine();
                }
            }


            tiles = new Tile[levelWidth, lines.Count];

            for (int iy = 0; iy < Height; iy++)
            {
                for (int ix = 0; ix < Width; ix++)
                {
                    char tileType = lines[iy][ix];
                    tiles[ix, iy] = LoadTile(tileType, ix, iy);
                }
            }
        }

        public int Width
        {
            get { return tiles.GetLength(0); }
        }


        public int Height
        {
            get { return tiles.GetLength(1); }
        }

        public Rectangle GetTileBounds(int x, int y)
        {
            return new Rectangle(x * Tile.Width, y * Tile.Height, Tile.Width, Tile.Height);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw player
            player.Draw(gameTime, spriteBatch);

            // Draw tiles:
            for (int iy = 0; iy < Height; iy++)
            {
                for (int ix = 0; ix < Width; ix++)
                {
                    Texture2D texture = tiles[ix, iy].Texture;
                    if (texture != null)
                    {
                        spriteBatch.Draw(texture, new Vector2(ix * Tile.Width, iy * Tile.Height), Color.White);
                    }
                }
            }

        }

        public void Update(GameTime gameTime, KeyboardState keyboardState, MouseState mouseState, GamePadState gamePadState)
        {
            player.Update(gameTime, keyboardState, mouseState, gamePadState);
        }



        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}