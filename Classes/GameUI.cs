using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BigInteger = System.Numerics.BigInteger;

namespace RocketJumper.Classes
{
    public class GameUI
    {
        private SpriteFont timerFont;

        // vars
        BigInteger Timer;

        public GameUI(SpriteFont timerFont, int timer = 0)
        {
            this.timerFont = timerFont;

            Timer = timer;
        }

        public void Update(GameTime gameTime)
        {
            Timer += gameTime.ElapsedGameTime.Milliseconds;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // draw timer in hours:minutes:seconds:milliseconds on the bottom center of the window
            string timerString;
            if (Timer > 1000 * 60 * 60)
                timerString = $"{Timer / 1000 / 60 / 60}:{Timer / 1000 / 60}:{Timer / 1000}:{Timer % 1000}";
            else if (Timer > 1000 * 60)
                timerString = $"{Timer / 1000 / 60}:{Timer / 1000}:{Timer % 1000}";
            else
                timerString = $"{Timer / 1000}:{Timer % 1000}";

            Vector2 timerSize = timerFont.MeasureString(timerString);
            spriteBatch.DrawString(
                spriteFont: timerFont,
                text: timerString,
                position: new Vector2(MyGame.ScreenWidth / 2,
                MyGame.ScreenHeight - 5),
                color: Color.Wheat,
                rotation: 0,
                origin: new Vector2(timerSize.X / 2, timerSize.Y),
                scale: 1,
                effects: SpriteEffects.None,
                layerDepth: 0);
        }
    }
}