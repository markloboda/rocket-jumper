﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        // const
        public const float PlayerScale = 2.5f;


        // camera
        public Matrix CameraTransform;


        // vars
        private Vector2 start;


        public Map Map;
        public Player Player;

        // content
        public ContentManager Content;
        public Animation_s RocketAnimation;

        public List<Sprite> ItemSprites = new();
        public List<Sprite> Sprites = new();
        public List<Turret> Turrets = new();



        public Level(IServiceProvider serviceProvider, String filePath)
        {
            Content = new ContentManager(serviceProvider, "Content");
            Map = new Map(filePath, this);
        }

        public void LoadContent()
        {
            // PLAYER
            Dictionary<string, Animation_s> playerAnimationDict = new()
            {
                ["idle"] = new Animation_s(Content.Load<Texture2D>("Sprites/Player/Idle"), 5, 0.2f),
                ["run"] = new Animation_s(Content.Load<Texture2D>("Sprites/Player/Run"), 4, 0.2f)
            };
            AnimatedSprite playerSprite = new AnimatedSprite(playerAnimationDict, start, this, "idle", PlayerScale, true, true);

            // Create Player Physics
            // Load Player
            Player = new Player(playerSprite);

            // ROCKETS
            RocketAnimation = new Animation_s(Content.Load<Texture2D>("Sprites/Rocket"), 5, 0.2f);


            // load all items and mapObjects
            foreach (Layer layer in Map.Layers)
                if (layer.Type == "objectgroup")
                    if (layer.Class == "items")
                        ItemSprites = layer.ItemSprites.Values.ToList();
                    else if (layer.Class == "map-objects")
                        Sprites = layer.Sprites.Values.ToList();

        }

        public void Update(GameTime gameTime)
        {
            Player.Update(gameTime);
            UpdateSprites(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw player
            Player.Draw(gameTime, spriteBatch);

            // draw Layers
            foreach (Layer layer in Map.Layers)
                if (layer.Type == "tilelayer")
                    DrawTileLayer(gameTime, spriteBatch, layer);

            DrawSprites(gameTime, spriteBatch);
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
                        if (tileGID >= tileSet.FirstGID)
                            tileSet.DrawTile(tileGID, new Vector2(x * Map.TileWidth + layer.X, y * Map.TileHeight + layer.Y), spriteBatch);
                }
            }
        }

        private void UpdateSprites(GameTime gameTime)
        {
            // update all items
            foreach (Sprite item in ItemSprites)
                item.Update(gameTime);

            // update turrets
            foreach (Turret turret in Turrets)
                turret.Update(gameTime);
        }

        private void DrawSprites(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // draw sprites
            foreach (Sprite mapObject in Sprites)
                mapObject.Draw(gameTime, spriteBatch);

            // draw items
            foreach (Sprite item in ItemSprites)
                item.Draw(gameTime, spriteBatch);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}