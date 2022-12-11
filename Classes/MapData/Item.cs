using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace RocketJumper.Classes.MapData
{
    class Item
    {
        public string Class;
        public int Gid;
        public int Id;
        public string Name;
        public float Rotation;
        public bool Visible;

        public TileSet tileSet;

        public Physics Physics;

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

        public Item(JObject objectJson, Level level, Map map)
        {
            Class = objectJson["class"].ToString();
            Gid = (int)objectJson["gid"];
            Id = (int)objectJson["id"];
            Name = objectJson["name"].ToString();
            Rotation = (float)objectJson["rotation"];
            Visible = (bool)objectJson["visible"];

            if (objectJson.ContainsKey("properties"))
            {
                JArray customProperties = objectJson["properties"].ToObject<JArray>();

                for (int i = 0; i < customProperties.Count; i++)
                {
                    var property = customProperties[i];
                    if (property["name"].ToString() == "AttachOffsetX")
                    {
                        AttachOffsetX = (int)property["value"];
                    }
                    else if (property["name"].ToString() == "AttachOffsetY")
                    {
                        AttachOffsetY = (int)property["value"];
                    }
                }
            }

            // find the tileset for this item
            foreach (TileSet tileSet in map.TileSets)
            {
                if (Gid >= tileSet.FirstGID)
                {
                    this.tileSet = tileSet;
                }
            }

            int x = (int)objectJson["x"];
            int y = (int)objectJson["y"];

            int height = (int)objectJson["height"];
            int width = (int)objectJson["width"];

            Physics = new Physics(new Vector2(x, y - height), new Vector2(width, height), level);
            Physics.AddBoundingBox();
            Physics.EnableGravity();
        }

        public void Update(GameTime gameTime)
        {
            Physics.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteEffects effects)
        {
            tileSet.DrawTile(Gid, Physics.Position, spriteBatch, effects);

            Physics.Draw(gameTime, spriteBatch);
        }

        public void MoveItemTo(Vector2 position)
        {
            Physics.MoveTo(position);
        }

        public void AddAttachmentOffset()
        {
            Physics.MoveBy(AttachOffset);
        }
    }
}