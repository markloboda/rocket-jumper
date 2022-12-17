using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace RocketJumper.Classes.MapData
{
    public class MapObject
    {
        public Sprite ObjectSprite;

        // read data
        public string Class;
        public int Gid;
        public int Id;
        public string Name;
        public float Rotation;
        public bool Visible;
        public TileSet TileSet;

        public Physics Physics;

        public SpriteEffects SpriteEffects;

        public Vector2 Origin;

        // custom properties //
        public int AttachOffsetX = 0;
        public int AttachOffsetY = 0;
        public Vector2 AttachOffset
        {
            get { return new Vector2(AttachOffsetX, AttachOffsetY); }
            set
            {
                this.AttachOffsetX = (int)value.X;
                this.AttachOffsetY = (int)value.Y;
            }
        }

        public MapObject Parent;
        public List<MapObject> Children;
        public int ParentId = -1;
        public bool HasChildren = false;

        public MapObject(JObject objectJson, Level level, Map map)
        {
            // READ PROPERTIES
            Class = objectJson["class"].ToString();
            Gid = (int)objectJson["gid"];
            Id = (int)objectJson["id"];
            Name = objectJson["name"].ToString();
            Rotation = (float)objectJson["rotation"];
            Visible = (bool)objectJson["visible"];
            SpriteEffects = SpriteEffects.None;

            int x = (int)objectJson["x"];
            int y = (int)objectJson["y"];

            int height = (int)objectJson["height"];
            int width = (int)objectJson["width"];

            if (objectJson.ContainsKey("properties"))
            {
                JArray customProperties = objectJson["properties"].ToObject<JArray>();

                for (int i = 0; i < customProperties.Count; i++)
                {
                    var property = customProperties[i];
                    string propertyName = property["name"].ToString();
                    if (propertyName == "AttachOffsetX")
                    {
                        AttachOffsetX = (int)property["value"];
                    }
                    else if (propertyName == "AttachOffsetY")
                    {
                        AttachOffsetY = (int)property["value"];
                    }
                    else if (propertyName == "Parent")
                    {
                        ParentId = (int)property["value"];
                    }
                    else if (propertyName == "HasChildren")
                    {
                        HasChildren = (bool)property["value"];
                        Children = new List<MapObject>();
                    }
                }
            }

            // find the tileset for this item
            foreach (TileSet tileSet in map.TileSets)
            {
                if (Gid >= tileSet.FirstGID)
                {
                    TileSet = tileSet;
                }
            }

            // create Sprite
            ObjectSprite = new StaticSprite(tileSet: TileSet, gid: Gid, position: new Vector2(x, y - height), level: level, spriteSize: new Vector2(width, height), attachmentOffset: AttachOffset);
        }

        public void Update(GameTime gameTime)
        {
            ObjectSprite.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            ObjectSprite.Draw(gameTime, spriteBatch);
        }

        public void AddAttachmentOffset()
        {
            ObjectSprite.Physics.MoveBy(AttachOffset);
        }
    }
}