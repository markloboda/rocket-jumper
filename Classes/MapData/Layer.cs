using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace RocketJumper.Classes.MapData
{
    struct Layer
    {
        public string Type;                                 // type of the layer
        public string Name;                                 // name of the layer
        public int Opacity;                                 // opacity of the layer
        public bool Visible;                                // visibility of the layer
        public int X;                                       // x position of the layer
        public int Y;                                       // y position of the layer
        public int Id;                                      // id of the layer


        // tile layer specific
        public int[] Data;                                  // data of tiles
        public int Height;                                  // height of the layer (num of tiles)
        public int Width;                                   // width of the layer  (num of tiles)
        // custom properties //
        public bool Collidable = false;                     // is layer collidable
        public bool Static = true;                          // is the layer static (all the tiles are non moving)

        // object layer specific
        public List<MapObject> Objects = null;                     // list of objects

        public Layer(JObject tileSetJson)
        {
            Type = (string)tileSetJson["type"];
            Name = (string)tileSetJson["name"];
            Opacity = (int)tileSetJson["opacity"];
            Visible = (bool)tileSetJson["visible"];
            X = (int)tileSetJson["x"];
            Y = (int)tileSetJson["y"];
            Id = (int)tileSetJson["id"];
            if (Type == "tilelayer")
            {
                Data = tileSetJson["data"].ToObject<int[]>();
                Height = (int)tileSetJson["height"];
                Width = (int)tileSetJson["width"];

                // Custom properties
                JArray customProperties = tileSetJson["properties"].ToObject<JArray>();

                for (int i = 0; i < customProperties.Count; i++)
                {
                    var property = customProperties[i];
                    if (property["name"].ToString() == "Collidable")
                    {
                        Collidable = (bool)property["value"];
                    }
                    else if (property["name"].ToString() == "Static")
                    {
                        Static = (bool)property["value"];
                    }
                }

            }
            else if (Type == "objectgroup")
            {

            }
        }

        public int GetTileType(int x, int y)
        {
            return Data[x + y * Width];
        }
    }
}