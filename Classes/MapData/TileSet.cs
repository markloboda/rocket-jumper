﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;

namespace RocketJumper.Classes.MapData
{
    struct TileSet
    {
        public Texture2D Texture;
        public Vector2 TileSize;
        public int TileCount;
        public int Columns;
        public int ImageHeight;
        public int ImageWidth;
        public int FirstGID;  // id of the first tile in the set

        private Level level;

        public TileSet(JObject tileSetJson, Level level)
        {
            this.level = level;

            Texture = level.Content.Load<Texture2D>(tileSetJson["image"].ToString());
            TileSize = new Vector2((int)tileSetJson["tilewidth"], (int)tileSetJson["tileheight"]);
            TileCount = (int)tileSetJson["tilecount"];
            Columns = (int)tileSetJson["columns"];
            ImageHeight = (int)tileSetJson["imageheight"];
            ImageWidth = (int)tileSetJson["imagewidth"];
            FirstGID = (int)tileSetJson["firstgid"];
        }
    }
}