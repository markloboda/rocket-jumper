using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RocketJumper.Classes.States;
using BigInteger = System.Numerics.BigInteger;

namespace RocketJumper.Classes
{
    public class GameUI
    {
        private Player player;
        private GameState gameState;

        // content
        public SpriteFont TimerFont;
        public Texture2D AmmoTexture;

        private long timeMilliseconds;

        // visible components
        public bool AmountOfTilesDrawn = true;


        public GameUI(Player player, GameState gameState)
        {
            this.player = player;
            this.gameState = gameState;
        }

        public void Update(GameTime gameTime)
        {
            timeMilliseconds = gameState.stopWatch.ElapsedMilliseconds;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // draw timer in hours:minutes:seconds:milliseconds on the bottom center of the window
            string timerString;
            if (timeMilliseconds > 1000 * 60 * 60)
                timerString = $"{timeMilliseconds / 1000 / 60 / 60}:{timeMilliseconds / 1000 / 60}:{timeMilliseconds / 1000}:{timeMilliseconds % 1000}";
            else if (timeMilliseconds > 1000 * 60)
                timerString = $"{timeMilliseconds / 1000 / 60}:{timeMilliseconds / 1000}:{timeMilliseconds % 1000}";
            else
                timerString = $"{timeMilliseconds / 1000}:{timeMilliseconds % 1000}";

            Vector2 timerSize = TimerFont.MeasureString(timerString);
            spriteBatch.DrawString(
                spriteFont: TimerFont,
                text: timerString,
                position: new Vector2(MyGame.ActualWidth / 2,
                MyGame.ActualHeight - 5),
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
                    position: new Vector2(MyGame.ActualWidth - 5 - i * 20, MyGame.ActualHeight - 5),
                    sourceRectangle: null,
                    color: Color.White,
                    rotation: 10,
                    origin: Vector2.Zero,
                    scale: 1,
                    effects: SpriteEffects.None,
                    layerDepth: 0);
            }

            // draw amount of tiles drawn
            if (AmountOfTilesDrawn)
            {
                string amountOfTilesDrawnString = $"Tiles drawn: {gameState.TilesDrawnCount}";
                Vector2 amountOfTilesDrawnSize = TimerFont.MeasureString(amountOfTilesDrawnString);
                spriteBatch.DrawString(
                    spriteFont: TimerFont,
                    text: amountOfTilesDrawnString,
                    position: new Vector2(5, MyGame.ActualHeight - 5),
                    color: Color.Black,
                    rotation: 0,
                    origin: new Vector2(0, amountOfTilesDrawnSize.Y),
                    scale: 1,
                    effects: SpriteEffects.None,
                    layerDepth: 0);
            }
        }
    }
}