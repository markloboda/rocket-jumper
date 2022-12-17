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

        public List<MapObject> Items = new();
        public List<MapObject> MapObjects = new();
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
            {
                if (layer.Type == "objectgroup")
                {
                    if (layer.Class == "items")
                    {
                        foreach (MapObject item in layer.Items)
                        {
                            Items.Add(item);
                        }
                    }
                    else if (layer.Class == "map-objects")
                    {
                        List<MapObject> parents = new();
                        List<MapObject> children = new();
                        foreach (MapObject mapObject in layer.MapObjects)
                        {
                            MapObjects.Add(mapObject);
                            // add parents at the end (so that they are initialized with children (example: Turret))
                            if (mapObject.HasChildren)
                                parents.Add(mapObject);
                            else
                            {
                                if (mapObject.ParentId != -1)
                                    children.Add(mapObject);
                            }
                        }

                        // add children to parents and initialize them
                        foreach (MapObject parent in parents)
                        {
                            foreach (MapObject child in children)
                                if (child.ParentId == parent.Id)
                                    parent.Children.Add(child);
                            if (parent.Name == "TurretBase")
                                Turrets.Add(new Turret(parent, this));
                        }
                    }
                }
            }

        }

        public void Update(GameTime gameTime)
        {
            Player.Update(gameTime);

            // update all items
            foreach (MapObject item in Items)
            {
                item.Update(gameTime);
            }

            // update turrets
            foreach (Turret turret in Turrets)
            {
                turret.Update(gameTime);
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