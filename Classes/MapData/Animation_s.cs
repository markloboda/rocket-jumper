using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketJumper.Classes.MapData
{
    public struct Animation_s
    {
        public Texture2D Texture { get; private set; }
        public int FrameCount { get; private set; }
        public float FrameTime { get; private set; }
        public int FrameWidth { get; private set; }
        public int FrameHeight { get; private set; }
        public Vector2 FrameSize { get { return new Vector2(FrameWidth, FrameHeight); } }

        public Animation_s(Texture2D texture, int frameCount, float frameTime)
        {
            Texture = texture;
            FrameCount = frameCount;
            FrameTime = frameTime;

            FrameWidth = Texture.Width / FrameCount;
            FrameHeight = Texture.Height;
        }
    }
}
