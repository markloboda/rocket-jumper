using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RocketJumper.Classes.MapData;
using System;
using System.Collections.Generic;

namespace RocketJumper.Classes
{
    public class AnimatedSprite : Sprite
    {
        public Dictionary<string, Animation_s> AnimationDict { get; set; }
        public string CurrentAnimationId { get; private set; }
        public Animation_s CurrentAnimation { get { return AnimationDict[CurrentAnimationId]; } }
        public List<Sprite> Children { get; set; }
        public SpriteEffects Effects { get; set; }
        public Vector2 FrameSize { get { return new Vector2(CurrentAnimation.FrameWidth, CurrentAnimation.FrameHeight); } }
        public bool IsLooping { get; set; }
        public int CurrentFrameId { get; private set; }
        public Physics Physics { get; set; }
        public Level Level { get; }

        // if Sprite attached
        public Vector2 AttachmentOffset { get; set; }
        public Vector2 AttachmentOrigin { get; set; }
        public bool MoveOnAttach { get; set; }

        // if Sprite from Tiled
        public string Name { get; set; }
        public int GID { get; set; }
        public int ID { get; set; }
        public int ParentId { get; set; }


        private Rectangle sourceRectangle;
        private float timer = 0.0f;

        public AnimatedSprite(Dictionary<string, Animation_s> animationDict, Vector2 position, Level level, string currentAnimationId, float scale = 1.0f, bool isLooping = false, bool gravityEnabled = false, float rotation = 0.0f, Vector2 attachmentOffset = default, Vector2 attachmentOrigin = default, bool moveOnAttach = false)
        {
            // default to first frame
            CurrentFrameId = 0;

            CurrentAnimationId = currentAnimationId;
            AnimationDict = animationDict;
            IsLooping = isLooping;

            Children = new();

            AttachmentOffset = attachmentOffset;
            AttachmentOrigin = attachmentOrigin;
            MoveOnAttach = moveOnAttach;

            Level = level;
            Physics = new Physics(position, FrameSize * scale, level, gravityEnabled, rotation);
        }

        public void Update(GameTime gameTime)
        {
            Physics.Update(gameTime);
            MoveChildren();

            // handle frames
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            while (timer > CurrentAnimation.FrameTime)
            {
                timer -= CurrentAnimation.FrameTime;
                if (IsLooping)
                    CurrentFrameId = (CurrentFrameId + 1) % CurrentAnimation.FrameCount;
                else
                    CurrentFrameId = (int)MathF.Min(CurrentFrameId + 1, CurrentAnimation.FrameCount - 1);
            }


            // set source rectangle
            sourceRectangle = GetSourceRectangle(CurrentFrameId);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Rectangle destinationRectangle = new Rectangle((int)Physics.Position.X, (int)Physics.Position.Y, (int)Physics.Size.X, (int)Physics.Size.Y);

            spriteBatch.Draw(
                texture: CurrentAnimation.Texture,
                sourceRectangle: sourceRectangle,
                destinationRectangle: destinationRectangle,
                color: Color.White,
                rotation: Physics.Rotation,
                origin: Physics.Origin,
                effects: Effects,
                layerDepth: 0);

            // draw children
            foreach (Sprite child in Children)
                child.Draw(gameTime, spriteBatch);

            Physics.Draw(gameTime, spriteBatch);
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

        public void AddInputToPhysics(GameTime gameTime, Vector2 movementVector)
        {
            Physics.AddInputMovement(gameTime, movementVector);
        }

        public void ChangeAnimation(string animationId)
        {
            CurrentAnimationId = animationId;

            timer = 0.0f;
            CurrentFrameId = 0;
        }

        private Rectangle GetSourceRectangle(int frameIndex)
        {
            return new Rectangle(frameIndex * CurrentAnimation.FrameWidth, 0, CurrentAnimation.FrameWidth, CurrentAnimation.FrameHeight);
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
    }
}
