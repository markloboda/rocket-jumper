using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RocketJumper.Classes.Controls;
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
        public Texture2D ProgressBar;

        // Replay 
        private List<Component> replayComponents;

        // flags
        public bool ReloadBarVisible = false;

        public long TimeMilliseconds;

        // visible components
        public bool AmountOfTilesDrawn = false;

        private Button replayPlayPauseButton;


        public GameUI(Player player, GameState gameState)
        {
            this.player = player;
            this.gameState = gameState;

            // Replay
            replayComponents = new List<Component>();


            replayPlayPauseButton = new Button(texture: gameState.PlayButtonTexture, font: gameState.Font)
            {
                Position = new Vector2(MyGame.ActualWidth / 2, MyGame.ActualHeight - 50),
                Scale = 0.1f,
                Color = Color.Gray,
                HoverColor = Color.DarkGray,
                Click = Button_Pause_Click
            };

            // add replay components
            replayComponents.Add(replayPlayPauseButton);

            replayComponents.Add(new Button(texture: gameState.ForwardButtonTexture, font: gameState.Font)
            {
                Position = new Vector2(MyGame.ActualWidth / 2 + 50, MyGame.ActualHeight - 50),
                Scale = 0.1f,
                Color = Color.Gray,
                HoverColor = Color.DarkGray,
                Click = Button_Forward_Click
            });

            replayComponents.Add(new Button(texture: gameState.ForwardButtonTexture, font: gameState.Font)
            {
                Position = new Vector2(MyGame.ActualWidth / 2 - 50, MyGame.ActualHeight - 50),
                Scale = 0.1f,
                Color = Color.Gray,
                HoverColor = Color.DarkGray,
                Click = Button_Backward_Click,
                SpriteEffects = SpriteEffects.FlipHorizontally
            });
        }

        public void Update(GameTime gameTime)
        {
            if (gameState.IsReplaying)
            {
                foreach (Component component in replayComponents)
                {
                    component.Update(gameTime);
                }
            }

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            if (gameState.IsReplaying)
            {
                foreach (Component component in replayComponents)
                {
                    component.Draw(gameTime, spriteBatch);
                }
            }

            spriteBatch.End();

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // draw timer in hours:minutes:seconds:milliseconds on the bottom center of the window
            TimeSpan time = TimeSpan.FromMilliseconds(TimeMilliseconds);
            string timerString;
            if (TimeMilliseconds > 1000 * 60 * 60)
                timerString = $"{time.Hours}:{time.Minutes}:{time.Seconds}:{time.Milliseconds}";
            else if (TimeMilliseconds > 1000 * 60)
                timerString = $"{time.Minutes}:{time.Seconds}:{time.Milliseconds}";
            else
                timerString = $"{time.Seconds}:{time.Milliseconds}";

            Vector2 timerSize = TimerFont.MeasureString(timerString);
            spriteBatch.DrawString(
                spriteFont: TimerFont,
                text: timerString,
                position: new Vector2(MyGame.ActualWidth / 2,
                MyGame.ActualHeight - 5),
                color: Color.DarkGoldenrod,
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
                    scale: 3,
                    effects: SpriteEffects.None,
                    layerDepth: 0);
            }

            // draw reload bar
            if (ReloadBarVisible)
            {
                float reloadProgress = 1.0f - ((float)this.player.ReloadTimer / this.player.ReloadRate);
                Rectangle progressRectangle = new Rectangle(0, 0, (int)(reloadProgress * ProgressBar.Width), ProgressBar.Height);

                Vector2 barPosition = Vector2.Transform(this.player.PlayerSprite.Physics.Position, this.gameState.camera.Transform) + new Vector2(this.player.PlayerSprite.Physics.Width * 1.5f, -20);

                // draw outer part
                spriteBatch.Draw(
                    texture: ProgressBar,
                    position: barPosition,
                    sourceRectangle: null,
                    color: Color.Black,
                    rotation: 0.0f,
                    origin: Vector2.Zero,
                    scale: 1,
                    effects: SpriteEffects.None,
                    layerDepth: 0
                );

                Vector2 origin = new Vector2(ProgressBar.Width / 2, ProgressBar.Height / 2);
                // draw inner part
                spriteBatch.Draw(
                    texture: ProgressBar,
                    position: barPosition + origin,
                    sourceRectangle: progressRectangle,
                    color: Color.LimeGreen,
                    rotation: 0.0f,
                    origin: origin,
                    scale: new Vector2(0.9f, 0.8f),
                    effects: SpriteEffects.None,
                    layerDepth: 0
                );
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

            spriteBatch.End();
        }

        public void Button_Play_Click(object sender, EventArgs e)
        {
            gameState.IsReplayPaused = false;
            replayPlayPauseButton.Texture = gameState.PlayButtonTexture;
            replayPlayPauseButton.Click = Button_Pause_Click;
        }

        public void Button_Pause_Click(object sender, EventArgs e)
        {
            gameState.IsReplayPaused = true;
            replayPlayPauseButton.Texture = gameState.PauseButtonTexture;
            replayPlayPauseButton.Click = Button_Play_Click;
        }

        public void Button_Forward_Click(object sender, EventArgs e)
        {

        }

        public void Button_Backward_Click(object sender, EventArgs e)
        {

        }
    }
}