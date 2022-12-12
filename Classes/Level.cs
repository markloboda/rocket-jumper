using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RocketJumper.Classes.MapData;

namespace RocketJumper.Classes
{
    public class Level : IDisposable
    {
        private Vector2 start;

        public Map Map;

        public Player Player;

        // content
        public ContentManager Content;
        public AnimatedSprite PlayerIdleAnimation, PlayerRunAnimation, RocketAnimation;

        public List<MapObject> Items = new();

        public Matrix CameraTransform;


        public Level(IServiceProvider serviceProvider, String filePath)
        {
            Content = new ContentManager(serviceProvider, "Content");
            Map = new Map(filePath, this);
        }

        public void LoadContent()
        {
            // Load Player
            PlayerIdleAnimation = new AnimatedSprite(Content.Load<Texture2D>("Sprites/Player/Idle"), 0.2f, true, 5, Player.PlayerSizeScale);
            PlayerRunAnimation = new AnimatedSprite(Content.Load<Texture2D>("Sprites/Player/Run"), 0.2f, true, 4, Player.PlayerSizeScale);
            Player = new Player(this, start);

            // load all items
            foreach (Layer layer in Map.Layers)
            {
                if (layer.Type == "objectgroup" && layer.Class == "items")
                {
                    foreach (MapObject item in layer.Items)
                    {
                        Items.Add(item);
                    }
                }
            }


            RocketAnimation = new AnimatedSprite(Content.Load<Texture2D>("Sprites/Rocket"), 0.2f, true, 5, 1.0f);
        }

        public void Update(GameTime gameTime)
        {
            Player.Update(gameTime);

            // update all items
            foreach (MapObject item in Items)
            {
                item.Update(gameTime);
            }

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw player
            Player.Draw(gameTime, spriteBatch);

            // draw Layers
            foreach (Layer layer in Map.Layers)
            {
                if (layer.Type == "tilelayer")
                    DrawTileLayer(gameTime, spriteBatch, layer);
                else if (layer.Type == "objectgroup")
                {
                    // skip items (are handled in LoadContent)
                    if (layer.Class == "items")
                        continue;
                    else if (layer.Class == "map-objects")
                        DrawMapObjects(gameTime, spriteBatch, layer);
                }
            }

            // draw items
            foreach (MapObject item in Items)
            {
                item.Draw(gameTime, spriteBatch);
            }

        }

        private void DrawTileLayer(GameTime gameTime, SpriteBatch spriteBatch, Layer layer)
        {
            for (int y = 0; y < layer.Height; y++)
            {
                for (int x = 0; x < layer.Width; x++)
                {
                    int tileGID = layer.Data[x + y * layer.Width];

                    if (tileGID == 0)
                        continue;

                    // find the tileset for this Tile and draw it
                    foreach (TileSet tileSet in Map.TileSets)
                    {
                        if (tileGID >= tileSet.FirstGID)
                        {
                            tileSet.DrawTile(tileGID, new Vector2(x * Map.TileWidth + layer.X, y * Map.TileHeight + layer.Y), spriteBatch);
                        }
                    }
                }
            }
        }

        private void DrawMapObjects(GameTime gameTime, SpriteBatch spriteBatch, Layer layer)
        {
            foreach (MapObject mapObject in layer.MapObjects)
            {
                mapObject.Draw(gameTime, spriteBatch);
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}