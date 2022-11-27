using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;

namespace RocketJumper.Classes.MapData
{
    struct Layer
    {
        public int[] Data;      // data of tiles
        public int Height;      // height of the layer
        public string Name;     // name of the layer
        public int Opacity;     // opacity of the layer
        public string Type;     // type of the layer
        public bool Visible;    // visibility of the layer
        public int Width;       // width of the layer
        public int X;           // x position of the layer
        public int Y;           // y position of the layer
        public int Id;          // id of the layer


        public Layer(JObject tileSetJson, Level level)
        {
            Data = tileSetJson["data"].ToObject<int[]>();
            Height = (int)tileSetJson["height"];
            Name = (string)tileSetJson["name"];
            Opacity = (int)tileSetJson["opacity"];
            Type = (string)tileSetJson["type"];
            Visible = (bool)tileSetJson["visible"];
            Width = (int)tileSetJson["width"];
            X = (int)tileSetJson["x"];
            Y = (int)tileSetJson["y"];
            Id = (int)tileSetJson["id"];
        }
    }
}