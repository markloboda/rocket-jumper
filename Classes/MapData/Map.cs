using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using RocketJumper.Classes.States;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace RocketJumper.Classes.MapData
{
    public struct Map
    {
        public string MapPath;
        private GameState gameState;


        public int Height;              // Number of tile rows
        public int Width;               // Number of tile columns
        public int TileHeight;          // Height of grid (1 tile)
        public int TileWidth;           // Width of grid (1 tile)
        public List<TileSet> TileSets;      // Array of tile sets
        public List<Layer> Layers;          // Array of layers

        public Vector2 start = new Vector2(28 * 32, 120 * 32);
        public Rectangle finish;

        public int WidthInPixels
        {
            get { return Width * TileWidth; }
        }

        public int HeightInPixels
        {
            get { return Height * TileHeight; }
        }

        public Map(string filePath, ContentManager content, GameState gameState)
        {
            MapPath = filePath;
            this.gameState = gameState;

            dynamic json = JObject.Parse(File.ReadAllText(MapPath));

            Width = json.width;
            Height = json.height;
            TileWidth = json.tilewidth;
            TileHeight = json.tileheight;

            // load tilesets
            TileSets = new List<TileSet>();
            foreach (dynamic tileSetJson in json.tilesets)
            {
                string tileSet = tileSetJson["image"].ToString();
                // check if .png
                if (tileSet.Contains(".png"))
                {
                    // remove .png
                    tileSet = tileSet.Substring(0, tileSet.Length - 4);
                }
                // \/ to /
                tileSet.Replace("\\/", "/");
                // add content
                tileSet = tileSet.Insert(2, "/Content");


                Texture2D tileSetTexture = content.Load<Texture2D>(tileSet);
                TileSets.Add(new TileSet(tileSetJson, tileSetTexture));
            }

            // load Layers (data)
            Layers = new List<Layer>();
            foreach (dynamic layerJson in json.layers)
            {
                var newLayer = new Layer(layerJson, gameState, this);
                Layers.Add(newLayer);

                if (newLayer.Class == "map-control") {
                    // add finish
                    foreach (var sprite in newLayer.Sprites.Values) {
                        if (sprite.Name == "Finish") {
                            finish = sprite.Physics.AABB;
                        }
                    }                    
                }
            }
        }

        public Rectangle GetBounds(int x, int y)
        {
            return new Rectangle(x * TileWidth, y * TileHeight, TileWidth, TileHeight);
        }

        public int GetTileId(int x, int y)
        {
            foreach (Layer layer in Layers)
            {
                if (layer.Type == "tilelayer")
                {
                    if (x < 0 || x >= Width || y < 0 || y >= Height)
                    {
                        return 0;
                    }

                    int id = layer.Data[x + y * Width];
                    if (id != 0)
                    {
                        return id;
                    }
                    // else check next layer
                }
            }
            return 0;
        }

        public TileSet GetTileSet(int id)
        {
            foreach (TileSet tileSet in TileSets)
            {
                if (id >= tileSet.FirstGID && id < tileSet.FirstGID + tileSet.TileCount)
                {
                    return tileSet;
                }
            }
            throw new System.Exception("TileSet not found");
        }
    }
}