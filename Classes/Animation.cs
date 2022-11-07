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

        public float FrameTime
        {
            get { return frameTime; }
        }
        float frameTime;

        public bool IsLooping
        {
            get { return IsLooping; }
        }
        bool isLooping;

        public int FrameCount
        {
            get { return frameCount; }
        }
        int frameCount;

        public int FrameWidth
        {
            get { return frameWidth; }
        }
        int frameWidth;

        public int FrameHeight
        {
            get { return frameHeight; }
        }
        int frameHeight;

        public Animation(Texture2D texture, float frameTime, bool isLooping, int frameCount)
        {
            this.texture = texture;
            this.frameTime = frameTime;
            this.isLooping = isLooping;
            this.frameCount = frameCount;

            this.frameWidth = texture.Width / frameCount;
            this.frameHeight = texture.Height;

        }
    }
}