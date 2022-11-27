using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Newtonsoft.Json.Linq;
using RocketJumper.Classes.MapData;

namespace RocketJumper.Classes
{
    class Level : IDisposable
    {
        private Tile[] collidables;
        List<TileSet> tileSets;
        List<Layer> layers;

        private Rectangle finishTileBounds;

        public Player Player
        {
            get { return player; }
        }
        Player player;

        private Vector2 start;

        public int Width
        {
            get { return width; }
        }
        private int width;

        public int Height
        {
            get { return height; }
        }
        private int height;

        public int TileWidth
        {
            get { return tileWidth; }
        }
        private int tileWidth;

        public int TileHeight
        {
            get { return tileHeight; }
        }
        private int tileHeight;


        // content
        public ContentManager Content
        {
            get { return content; }
        }
        ContentManager content;

        public Level(IServiceProvider serviceProvider, String filePath)
        {
            content = new ContentManager(serviceProvider, "Content");

            LoadJsonMap(filePath);

            player = new Player(this, start, collidables);
        }

        private void LoadJsonMap(String file)
        {
            dynamic json = JObject.Parse(File.ReadAllText(file));

            width = json.width;
            height = json.height;

            tileWidth = json.tilewidth;
            tileHeight = json.tileheight;

            // load tilesets
            tileSets = new List<TileSet>();
            foreach (dynamic tileSetJson in json.tilesets)
            {
                tileSets.Add(new TileSet(tileSetJson, this));
            }

            // load layers (data)
            layers = new List<Layer>();
            foreach (dynamic layerJson in json.layers)
            {
                layers.Add(new Layer(layerJson, this));
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw player
            player.Draw(gameTime, spriteBatch);

            // draw layers
            foreach (Layer layer in layers)
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
                    for (int i = 0; i < tileSets.Count; i++)
                    {
                        if (tileIndex < tileSets[i].FirstGID)
                        {
                            tileSetIndex = i - 1;
                            break;
                        }
                    }

                    int tileSetTileIndex = tileIndex - tileSets[tileSetIndex].FirstGID;

                    DrawTile(tileSetIndex, tileSetTileIndex, new Vector2(x * TileWidth, y * TileHeight), spriteBatch);
                }
            }
        }

        private void DrawTile(int tileSetIndex, int tileIndex, Vector2 position, SpriteBatch spriteBatch)
        {
            TileSet tileSet = tileSets[tileSetIndex];

            int row = tileIndex / tileSet.Columns;
            int column = tileIndex % tileSet.Columns;

            Rectangle sourceRectangle = new Rectangle(column * (int)tileSet.TileSize.X, row * (int)tileSet.TileSize.Y, (int)tileSet.TileSize.X, (int)tileSet.TileSize.Y);
            Rectangle destinationRectangle = new Rectangle((int)position.X, (int)position.Y, (int)tileSet.TileSize.X, (int)tileSet.TileSize.Y);

            spriteBatch.Draw(tileSet.Texture, destinationRectangle, sourceRectangle, Color.White);
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