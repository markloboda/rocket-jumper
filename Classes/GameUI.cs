using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BigInteger = System.Numerics.BigInteger;

namespace RocketJumper.Classes
{
    public class GameUI
    {
        private Player player;

        // content
        public SpriteFont TimerFont;
        public Texture2D AmmoTexture;

        // vars
        BigInteger Timer;

        public GameUI(Player player, int timer = 0)
        {
            this.player = player;

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

            Vector2 timerSize = TimerFont.MeasureString(timerString);
            spriteBatch.DrawString(
                spriteFont: TimerFont,
                text: timerString,
                position: new Vector2(MyGame.ScreenWidth / 2,
                MyGame.ScreenHeight - 5),
                color: Color.Wheat,
                rotation: 0,
                origin: new Vector2(timerSize.X / 2, timerSize.Y),
                scale: 1,
                effects: SpriteEffects.None,
                layerDepth: 0);

            // draw ammo
            int ammoCount = player.AmmoCount;
            for (int i = 0; i < ammoCount; i++)
            {
                // bottom right
                spriteBatch.Draw(
                    texture: AmmoTexture,
                    position: new Vector2(MyGame.ScreenWidth - 5 - i * 20, MyGame.ScreenHeight - 5),
                    sourceRectangle: null,
                    color: Color.White,
                    rotation: 10,
                    origin: Vector2.Zero,
                    scale: 1,
                    effects: SpriteEffects.None,
                    layerDepth: 0);
            }
        }
    }
}