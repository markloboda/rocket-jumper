using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using RocketJumper.Classes.States;
using System.Reflection.Metadata.Ecma335;

namespace RocketJumper.Classes.MapData
{
    public struct TileSet
    {
        public Texture2D Texture;
        public Vector2 TileSize;
        public int TileSizeX;
        public int TileSizeY;
        public int TileCount;
        public int Columns;
        public int ImageHeight;
        public int ImageWidth;
        public int FirstGID;  // id of the first tile in the set
        public bool IsIce;


        public TileSet(JObject tileSetJson, Texture2D texture)
        {
            Texture = texture;
            TileSize = new Vector2((int)tileSetJson["tilewidth"], (int)tileSetJson["tileheight"]);
            TileSizeX = (int)tileSetJson["tilewidth"];
            TileSizeY = (int)tileSetJson["tileheight"];
            TileCount = (int)tileSetJson["tilecount"];
            Columns = (int)tileSetJson["columns"];
            ImageHeight = (int)tileSetJson["imageheight"];
            ImageWidth = (int)tileSetJson["imagewidth"];
            FirstGID = (int)tileSetJson["firstgid"];

            // custom properties
            if (tileSetJson.ContainsKey("tiles"))
            {
                foreach (var properties in tileSetJson["tiles"]) {
                    if (properties["properties"] != null) {
                        foreach (var property in properties["properties"]) {
                            if (property["name"].ToString() == "IsIce") {
                                IsIce = bool.Parse(property["value"].ToString());
                            }
                        }
                    }
                }
            }
        }

        public void DrawTile(int tileGID, Vector2 position, SpriteBatch spriteBatch, Vector2 size, SpriteEffects effects = SpriteEffects.None, float rotation = 0.0f, Vector2 origin = default)
        {
            Rectangle sourceRectangle = GetSourceRectangle(tileGID);

            //spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White, rotation, origin, effects, 0.0f);
            spriteBatch.Draw(Texture, position: position, sourceRectangle, Color.White, rotation, origin, scale: size.X / TileSize.X, effects, 0.0f);
        }

        public void DrawTile(int tileGID, Vector2 position, SpriteBatch spriteBatch, SpriteEffects effects = SpriteEffects.None, float rotation = 0.0f, Vector2 origin = default)
        {
            Rectangle sourceRectangle = GetSourceRectangle(tileGID);

            //spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White, rotation, origin, effects, 0.0f);
            spriteBatch.Draw(Texture, position: position, sourceRectangle, Color.White, rotation, origin, scale: 1, effects, 0.0f);
        }

        public Rectangle GetSourceRectangle(int tileGID)
        {
            int tileIndex = tileGID - FirstGID;

            int row = tileIndex / Columns;
            int column = tileIndex % Columns;

            return new Rectangle(column * (int)TileSize.X, row * (int)TileSize.Y, (int)TileSize.X, (int)TileSize.Y);
        }
    }
}