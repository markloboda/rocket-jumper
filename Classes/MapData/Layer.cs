﻿using Newtonsoft.Json.Linq;
using RocketJumper.Classes.States;
using System.Collections.Generic;
using System.Numerics;

namespace RocketJumper.Classes.MapData
{
    public struct Layer
    {
        public string Type;                                 // type of the layer
        public string Class;                                // class of the layer
        public string Name;                                 // name of the layer
        public int Opacity;                                 // opacity of the layer
        public bool Visible;                                // visibility of the layer
        public int Id;                                      // id of the layer

        // tile layer specific
        public int[] Data;                                  // data of tiles
        public int Height;                                  // height of the layer (num of tiles)
        public int Width;                                   // width of the layer  (num of tiles)
        // custom properties //
        public bool Collidable = false;                     // is layer collidable
        public bool Static = true;                          // is the layer static (all the tiles are non moving)

        // object layer specific
        public Dictionary<int, Sprite> Sprites = new Dictionary<int, Sprite>();

        public Layer(JObject layerJson, GameState gameState, Map map)
        {
            Type = (string)layerJson["type"];
            Class = (string)layerJson["class"];
            Name = (string)layerJson["name"];
            Opacity = (int)layerJson["opacity"];
            Visible = (bool)layerJson["visible"];
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
                        Collidable = (bool)property["value"];
                    else if (property["name"].ToString() == "Static")
                        Static = (bool)property["value"];
                }

            }
            else if (Type == "objectgroup")
            {
                JArray objects = layerJson["objects"].ToObject<JArray>();
                for (int i = 0; i < objects.Count; i++)
                {
                    Sprite sprite = JsonReader.GetSpriteFromJson(objects[i].ToObject<JObject>(), gameState, map.TileSets);
                    if (Class == "items")
                        Sprites.Add(sprite.ID, sprite);
                    else if (Class == "map-objects")
                        Sprites.Add(sprite.ID, sprite);
                    else if (Class == "map-controls")
                        Sprites.Add(sprite.ID, sprite);
                }

                // add children to parents
                foreach (Sprite sprite in Sprites.Values)
                {
                    if (sprite.ParentId != -1)
                    {
                        Sprites[sprite.ParentId].AddChild(sprite);
                        Sprites.Remove(sprite.ID);
                    }
                }
            }
        }

        public int GetTileTypeFromTile(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return 0;
            return Data[x + y * Width];
        }
    }
}