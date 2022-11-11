using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RocketJumper.Classes
{
    class Animation
    {
        public Texture2D Texture
        {
            get { return texture; }
        }
        Texture2D texture;

        // The time per animation frame.
        public float FrameTime
        {
            get { return frameTime; }
        }
        float frameTime;

        // Is animation looping?
        public bool IsLooping
        {
            get { return IsLooping; }
        }
        bool isLooping;

        // The number of frames in the animation.
        public int FrameCount
        {
            get { return frameCount; }
        }
        int frameCount;

        // Width of each frame.
        public int FrameWidth
        {
            get { return frameWidth; }
        }
        int frameWidth;

        // Height of each frame.
        public int FrameHeight
        {
            get { return frameHeight; }
        }
        int frameHeight;

        // The current frame of animation.
        public int FrameIndex
        {
            get { return frameIndex; }
        }
        int frameIndex;

        public float Scale
        {
            get { return scale; }
        }
        float scale;

        public Vector2 Origin
        {
            get { return new Vector2(frameWidth / 2.0f, frameHeight); }
        }

        // The time since we last updated the frame.
        private float time;

        // Is the animation playing?
        private bool isPlaying = false;


        public void StartAnimation()
        {
            if (isPlaying)
                return;

            this.frameIndex = 0;
            this.time = 0.0f;
            this.isPlaying = true;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects)
        {
            if (!isPlaying)
                return;

            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            while (time > frameTime)
            {
                time -= frameTime;

                // Advance the frame index; looping if necessary.
                if (isLooping)
                    frameIndex = (frameIndex + 1) % frameCount;
                else
                    frameIndex = Math.Min(frameIndex + 1, frameCount - 1);
            }

            // Calculate the source rectangle of the current frame.
            int frameX = (frameIndex % (texture.Width / frameWidth)) * frameWidth;
            int frameY = (frameIndex / (texture.Width / frameWidth)) * frameHeight;
            Rectangle source = new Rectangle(frameX, frameY, frameWidth, frameHeight);

            // Draw the current frame.
            spriteBatch.Draw(texture, position, source, Color.White, 0.0f, Origin, scale, spriteEffects, 0.0f);
        }

        public Animation(Texture2D texture, float frameTime, bool isLooping, int frameCount, float scale)
        {
            this.texture = texture;
            this.frameTime = frameTime;
            this.isLooping = isLooping;
            this.frameCount = frameCount;
            this.scale = scale;

            this.frameWidth = texture.Width / frameCount;
            this.frameHeight = texture.Height;
        }
    }
}