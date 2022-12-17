using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using RocketJumper.Classes.MapData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.Emit;

namespace RocketJumper.Classes
{
    public class StaticSprite : Sprite
    {
        public Texture2D Texture { get { return TextureDict[CurrentAnimationId]; } }
        public Dictionary<string, Texture2D> TextureDict { get; set; }
        public string CurrentAnimationId { get; set; }
        public List<Sprite> Children { get; set; }
        public SpriteEffects Effects { get; set; }

        // if texture in tileset
        public bool IsTileSetUsed;
        public TileSet TileSet { get; set; }
        public int GID { get; private set; }

        // if Sprite attached
        public Vector2 AttachmentOffset { get; set; }
        public Vector2 AttachmentOrigin { get; set; }

        public float SpriteScale { get; set; }

        public Physics Physics { get; set; }


        public StaticSprite(Dictionary<string, Texture2D> textureDict, string currentTextureId, Vector2 position, Level level, Vector2 spriteSize, bool gravityEnabled = false, float rotation = 0.0f, Vector2 attachmentOffset = default, Vector2 attachmentOrigin = default)
        {
            CurrentAnimationId = currentTextureId;
            TextureDict = textureDict;
            Children = new();

            IsTileSetUsed = false;

            AttachmentOffset = attachmentOffset;
            AttachmentOrigin = attachmentOrigin;

            Physics = new Physics(position, spriteSize, level, gravityEnabled, rotation);

        }

        public StaticSprite(TileSet tileSet, int gid, Vector2 position, Level level, Vector2 spriteSize, bool gravityEnabled = false, float rotation = 0.0f, Vector2 attachmentOffset = default, Vector2 attachmentOrigin = default)
        {
            TileSet = tileSet;
            GID = gid;
            Children = new();

            IsTileSetUsed = true;

            AttachmentOffset = attachmentOffset;
            AttachmentOrigin = attachmentOrigin;

            Physics = new Physics(position, spriteSize, level, gravityEnabled, rotation);
        }

        public void Update(GameTime gameTime)
        {
            MoveChildren();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (IsTileSetUsed)
                TileSet.DrawTile(tileGID: GID, position: Physics.Position, spriteBatch: spriteBatch, size: Physics.Size, effects: Effects, rotation: Physics.Rotation, origin: Physics.Origin);
            else
            {
                // draw self
                Rectangle destinationRectangle = new Rectangle((int)Physics.Position.X, (int)Physics.Position.Y, (int)Physics.Size.X, (int)Physics.Size.Y);

                spriteBatch.Draw(
                    texture: Texture,
                    sourceRectangle: null,
                    destinationRectangle: destinationRectangle,
                    color: Color.White,
                    rotation: Physics.Rotation,
                    origin: Physics.Origin,
                    effects: Effects,
                    layerDepth: 0);
            }

            // draw children
            foreach (Sprite child in Children)
                child.Draw(gameTime, spriteBatch);

            Physics.Draw(gameTime, spriteBatch);
        }

        private void MoveChildren()
        {
            foreach (Sprite child in Children)
                if (child.Physics != null)
                {
                    child.Physics.MoveTo(Physics.Position);
                    child.AddAttachmentOffset();
                }
        }

        public void AddAttachmentOffset()
        {
            Physics.MoveBy(AttachmentOffset + Physics.Origin);
        }

        public void AddChild(Sprite sprite)
        {
            Children.Add(sprite);
            sprite.Physics.Origin = sprite.AttachmentOrigin;
        }

        public void ChangeTexture()
        {

        }

        public void AddTexture(string id, Texture2D texture)
        {

        }
    }
}
