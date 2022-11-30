using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RocketJumper.Classes.MapData;

namespace RocketJumper.Classes
{
    class Level : IDisposable
    {
        private Vector2 start;
        private Rectangle finishTileBounds;

        public Map Map;

        public Player Player;

        // content
        public ContentManager Content;

        public Level(IServiceProvider serviceProvider, String filePath)
        {
            Content = new ContentManager(serviceProvider, "Content");

            Map = new Map(filePath, this);
            Player = new Player(this, start);
        }

        

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw player
            Player.Draw(gameTime, spriteBatch);

            // draw Layers
            foreach (Layer layer in Map.Layers)
            {
                DrawTileLayer(gameTime, spriteBatch, layer);
            }
        }

        private void DrawTileLayer(GameTime gameTime, SpriteBatch spriteBatch, Layer layer)
        {

            for (int y = 0; y < layer.Height; y++)
            {
                for (int x = 0; x < layer.Width; x++)
                {
                    int tileIndex = layer.Data[x + y * layer.Width];

                    if (tileIndex == 0)
                        continue;

                    int tileSetIndex = 0;
                    for (int i = 0; i < Map.TileSets.Count; i++)
                    {
                        if (tileIndex < Map.TileSets[i].FirstGID)
                        {
                            tileSetIndex = i - 1;
                            break;
                        }
                    }

                    int tileSetTileIndex = tileIndex - Map.TileSets[tileSetIndex].FirstGID;

                    DrawTile(tileSetIndex, tileSetTileIndex, new Vector2(x * Map.TileWidth + layer.X, y * Map.TileHeight + layer.Y), spriteBatch);
                }
            }
        }

        private void DrawTile(int tileSetIndex, int tileIndex, Vector2 position, SpriteBatch spriteBatch)
        {
            TileSet tileSet = Map.TileSets[tileSetIndex];

            int row = tileIndex / tileSet.Columns;
            int column = tileIndex % tileSet.Columns;

            Rectangle sourceRectangle = new Rectangle(column * (int)tileSet.TileSize.X, row * (int)tileSet.TileSize.Y, (int)tileSet.TileSize.X, (int)tileSet.TileSize.Y);
            Rectangle destinationRectangle = new Rectangle((int)position.X, (int)position.Y, (int)tileSet.TileSize.X, (int)tileSet.TileSize.Y);

            spriteBatch.Draw(tileSet.Texture, destinationRectangle, sourceRectangle, Color.White);
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState, MouseState mouseState, GamePadState gamePadState)
        {
            Player.Update(gameTime, keyboardState, mouseState, gamePadState);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}