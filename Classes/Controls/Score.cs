using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RocketJumper.Classes.Controls
{
    public class Score : Component
    {
        public Vector2 Position;
        private SpriteFont font;

        public long Time;
        public string Date;
        public string Username;

        public string Text
        {
            get
            {
                string timerString;
                if (Time > 1000 * 60 * 60)
                    timerString = $"{Time / 1000 / 60 / 60}:{Time / 1000 / 60}:{Time / 1000}:{Time % 1000}";
                else if (Time > 1000 * 60)
                    timerString = $"{Time / 1000 / 60}:{Time / 1000}:{Time % 1000}";
                else
                    timerString = $"{Time / 1000}:{Time % 1000}";

                return $"{Username} ({Date}): {timerString}s";
            }
        }

        public Vector2 Origin
        {
            get
            {
                return new Vector2(font.MeasureString(Text).X / 2, font.MeasureString(Text).Y / 2);
            }
        }

        public Score(SpriteFont font)
        {
            this.font = font;
        }


        public override void Update(GameTime gameTime)
        {
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var x = Position.X - Origin.X;
            var y = Position.Y - Origin.Y;
            spriteBatch.DrawString(font, Text, new Vector2(x, y), Color.Black);
        }

    }
}