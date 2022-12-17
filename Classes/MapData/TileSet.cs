using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
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


        public TileSet(JObject tileSetJson, Level level)
        {
            Texture = level.Content.Load<Texture2D>(tileSetJson["image"].ToString());
            TileSize = new Vector2((int)tileSetJson["tilewidth"], (int)tileSetJson["tileheight"]);
            TileSizeX = (int)tileSetJson["tilewidth"];
            TileSizeY = (int)tileSetJson["tileheight"];
            TileCount = (int)tileSetJson["tilecount"];
            Columns = (int)tileSetJson["columns"];
            ImageHeight = (int)tileSetJson["imageheight"];
            ImageWidth = (int)tileSetJson["imagewidth"];
            FirstGID = (int)tileSetJson["firstgid"];
        }

        public void DrawTile(int tileGID, Vector2 position, SpriteBatch spriteBatch, Vector2 size, SpriteEffects effects = SpriteEffects.None, float rotation = 0.0f, Vector2 origin = default)
        {
            int xScale = (int)(size.X / TileSize.X);
            int yScale = (int)(size.Y / TileSize.Y);

            Rectangle sourceRectangle = GetSourceRectangle(tileGID);
            Rectangle destinationRectangle = new Rectangle((int)position.X, (int)position.Y, (int)(TileSize.X * xScale), (int)(TileSize.Y * yScale));

            spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White, rotation, origin, effects, 0.0f);
        }

        public void DrawTile(int tileGID, Vector2 position, SpriteBatch spriteBatch, SpriteEffects effects = SpriteEffects.None, float rotation = 0.0f)
        {
            Rectangle sourceRectangle = GetSourceRectangle(tileGID);
            Rectangle destinationRectangle = new Rectangle((int)position.X, (int)position.Y, (int)TileSize.X, (int)TileSize.Y);

            spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White, rotation, Vector2.Zero, effects, 0.0f);
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