using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace RocketJumper.Classes.MapData
{
    struct Layer
    {
        public string Type;                                 // type of the layer
        public string Class;                                // class of the layer
        public string Name;                                 // name of the layer
        public int Opacity;                                 // opacity of the layer
        public bool Visible;                                // visibility of the layer
        public int X;                                       // x position of the layer
        public int Y;                                       // y position of the layer
        public int Id;                                      // id of the layer

        public Vector2 Position
        {
            get { return new Vector2(X, Y); }
        }


        // tile layer specific
        public int[] Data;                                  // data of tiles
        public int Height;                                  // height of the layer (num of tiles)
        public int Width;                                   // width of the layer  (num of tiles)
        // custom properties //
        public bool Collidable = false;                     // is layer collidable
        public bool Static = true;                          // is the layer static (all the tiles are non moving)

        // object layer specific
        public List<Item> Items = null;                     // list of objects

        public Layer(JObject layerJson, Level level, Map map)
        {
            Type = (string)layerJson["type"];
            Class = (string)layerJson["class"];
            Name = (string)layerJson["name"];
            Opacity = (int)layerJson["opacity"];
            Visible = (bool)layerJson["visible"];
            X = (int)layerJson["x"];
            Y = (int)layerJson["y"];
            Id = (int)layerJson["id"];


            if (Type == "tilelayer")
            {
                Data = layerJson["data"].ToObject<int[]>();
                Height = (int)layerJson["height"];
                Width = (int)layerJson["width"];

                // Custom properties
                JArray customProperties = layerJson["properties"].ToObject<JArray>();

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
                if (Class == "items")
                {
                    Items = new List<Item>();
                    JArray objects = layerJson["objects"].ToObject<JArray>();
                    for (int i = 0; i < objects.Count; i++)
                    {
                        Items.Add(new Item(objects[i].ToObject<JObject>(), level, map));
                    }
                }
            }
        }

        public int GetTileType(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return 0;
            return Data[x + y * Width];
        }
    }
}