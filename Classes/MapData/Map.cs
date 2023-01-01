﻿using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using System.Reflection.Emit;
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

        public int WidthInPixels
        {
            get { return Width * TileWidth; }
        }

        public Map(string filePath, ContentManager content)
        {
            MapPath = filePath;

            dynamic json = JObject.Parse(File.ReadAllText(MapPath));

            Width = json.width;
            Height = json.height;
            TileWidth = json.tilewidth;
            TileHeight = json.tileheight;

            // load tilesets
            TileSets = new List<TileSet>();
            foreach (dynamic tileSetJson in json.tilesets)
            {
                Texture2D tileSetTexture = content.Load<Texture2D>(tileSetJson["image"].ToString());
                TileSets.Add(new TileSet(tileSetJson, tileSetTexture));
            }

            // load Layers (data)
            Layers = new List<Layer>();
            foreach (dynamic layerJson in json.layers)
            {
                Layers.Add(new Layer(layerJson, gameState, this));
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
    }
}