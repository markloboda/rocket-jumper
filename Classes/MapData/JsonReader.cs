using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace RocketJumper.Classes.MapData
{
    public class JsonReader
    {
        public static Sprite GetSpriteFromJson(JObject objectJson, Gameplay level, List<TileSet> tileSets)
        {
            string name = objectJson["name"].ToString();
            int gid = (int)objectJson["gid"];
            int id = (int)objectJson["id"];
            Vector2 size = new Vector2((float)objectJson["width"], (float)objectJson["height"]);
            Vector2 position = new Vector2((float)objectJson["x"], (float)objectJson["y"] - size.Y);

            Vector2 attachOffset = Vector2.Zero;
            bool moveOnAttach = false;
            int parentId = -1;
            if (objectJson.ContainsKey("properties"))
            {
                JArray customProperties = objectJson["properties"].ToObject<JArray>();

                for (int i = 0; i < customProperties.Count; i++)
                {
                    var property = customProperties[i];
                    string propertyName = property["name"].ToString();
                    if (propertyName == "AttachOffsetX")
                        attachOffset.X = (int)property["value"];
                    else if (propertyName == "AttachOffsetY")
                        attachOffset.Y = (int)property["value"];
                    else if (propertyName == "MoveOnAttach")
                        moveOnAttach = (bool)property["value"];
                    else if (propertyName == "Parent")
                        parentId = (int)property["value"];
                }
            }

            // find the tileset for this item
            TileSet tileSet = default;
            foreach (TileSet set in tileSets)
                if (gid >= set.FirstGID)
                    tileSet = set;


            return new StaticSprite(
                tileSet: tileSet,
                gid: gid,
                id: id,
                position: position,
                level: level,
                spriteSize: size,
                name: name,
                attachmentOffset: attachOffset,
                moveOnAttach: moveOnAttach,
                parentId: parentId
                );

        }
    }
}
