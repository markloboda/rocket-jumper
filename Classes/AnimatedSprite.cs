using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RocketJumper.Classes.MapData;
using RocketJumper.Classes.States;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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
        public GameState GameState { get; }
        public float Scale { get { return Physics.Size.X / FrameSize.X; } }

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

        public AnimatedSprite(Dictionary<string, Animation_s> animationDict, Vector2 position, GameState gameState, string currentAnimationId, float scale = 1.0f, bool isLooping = false, bool gravityEnabled = false, float rotation = 0.0f, Vector2 attachmentOffset = default, Vector2 attachmentOrigin = default, bool moveOnAttach = false)
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

            GameState = gameState;
            Physics = new Physics(position, FrameSize * scale, gameState, gravityEnabled, rotation);
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
            Physics.Position = new Vector2((int)Physics.Position.X, (int)Physics.Position.Y);

            spriteBatch.Draw(
                texture: CurrentAnimation.Texture,
                position: Physics.Position,
                sourceRectangle: sourceRectangle,
                color: Color.White,
                rotation: Physics.Rotation,
                origin: Physics.Origin,
                scale: Scale,
                effects: Effects,
                0
                );

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

        public bool CollidesWith(Sprite other)
        {
            return Physics.AABB.Intersects(other.Physics.AABB);
        }
    }
}
