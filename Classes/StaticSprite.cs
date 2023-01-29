using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using RocketJumper.Classes.MapData;
using System.Collections.Generic;
using RocketJumper.Classes.States;

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


        // if Sprite attached
        public Vector2 AttachmentOffset { get; set; }
        public Vector2 AttachmentOrigin { get; set; }
        public bool MoveOnAttach { get; set; }

        // if Sprite from Tiled
        public string Name { get; set; }
        public bool IsTileSetUsed;
        public TileSet TileSet { get; set; }
        public int GID { get; set; }
        public int ID { get; set; }
        public int ParentId { get; set; }

        public float SpriteScale { get; set; }

        public Physics Physics { get; set; }
        public Vector2 Scale { get { return new Vector2(Physics.Size.X / Texture.Width, Physics.Size.Y / Texture.Height); } }


        public StaticSprite(Dictionary<string, Texture2D> textureDict, string currentTextureId, Vector2 position, GameState level, Vector2 spriteSize, bool gravityEnabled = false, float rotation = 0.0f, Vector2 attachmentOffset = default, bool moveOnAttach = false, Vector2 attachmentOrigin = default)
        {
            CurrentAnimationId = currentTextureId;
            TextureDict = textureDict;
            Children = new();

            IsTileSetUsed = false;

            AttachmentOffset = attachmentOffset;
            AttachmentOrigin = attachmentOrigin;
            MoveOnAttach = moveOnAttach;

            Physics = new Physics(position, spriteSize, level, gravityEnabled, rotation);

        }

        public StaticSprite(TileSet tileSet, int gid, int id, Vector2 position, GameState gameState, Vector2 spriteSize, string name = default, bool gravityEnabled = false, float rotation = 0.0f, Vector2 attachmentOffset = default, bool moveOnAttach = false, Vector2 attachmentOrigin = default, int parentId = -1)
        {
            TileSet = tileSet;
            Name = name;
            GID = gid;
            ID = id;
            ParentId = parentId;
            Children = new();

            IsTileSetUsed = true;

            AttachmentOffset = attachmentOffset;
            AttachmentOrigin = attachmentOrigin;
            MoveOnAttach = moveOnAttach;

            Physics = new Physics(position, spriteSize, gameState, gravityEnabled, rotation);
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
                if (child.Physics != null && child.MoveOnAttach)
                {
                    child.Physics.MoveTo(Physics.Position);
                    child.AddAttachmentOffset();
                    child.AddOriginOffset();
                }
        }

        public void AddAttachmentOffset()
        {
            Physics.MoveBy(AttachmentOffset);
        }

        public void AddOriginOffset()
        {
            Physics.MoveBy(Physics.Origin);
        }

        public void AddChild(Sprite sprite)
        {
            Children.Add(sprite);
            sprite.Physics.Origin = sprite.AttachmentOrigin;
        }
        
        public bool CollidesWith(Sprite other)
        {
            return Physics.AABB.Intersects(other.Physics.AABB);
        }

        public void ChangeTexture()
        {

        }

        public void AddTexture(string id, Texture2D texture)
        {

        }
    }
}
