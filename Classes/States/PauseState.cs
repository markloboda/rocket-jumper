using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RocketJumper.Classes.Controls;
using Microsoft.Xna.Framework.Input;

namespace RocketJumper.Classes.States
{
    public class PauseState : State
    {
        private List<Component> components;

        private List<Component> pauseComponents;

        public Texture2D Background;

        GameState gameState;

        private KeyboardState keyboardState;

        public PauseState(MyGame game, ContentManager content, GameState gameState) : base(game, content)
        {
            this.gameState = gameState;
        }

        public override void LoadContent()
        {
            var buttonFont = game.Font;

            pauseComponents = new List<Component>();
            pauseComponents.Add(
            new Button(Tools.GetSingleColorTexture(game.GraphicsDevice, Color.White), buttonFont)
            {
                Position = new Vector2(MyGame.ActualWidth / 2, 100),
                Text = "Resume",
                Click = new EventHandler(Button_Resume_Clicked)
            });

            pauseComponents.Add(
            new Button(Tools.GetSingleColorTexture(game.GraphicsDevice, Color.White), buttonFont)
            {
                Position = new Vector2(MyGame.ActualWidth / 2, 200),
                Text = "Save",
                Click = new EventHandler(Button_Save_Clicked)
            });

            pauseComponents.Add(
            new Button(Tools.GetSingleColorTexture(game.GraphicsDevice, Color.White), buttonFont)
            {
                Position = new Vector2(MyGame.ActualWidth / 2, 300),
                Text = "Quit",
                Click = new EventHandler(Button_Quit_Clicked)
            });

            components = pauseComponents;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in components)
                component.Update(gameTime);

            UpdateKeyboardState();
            // Handle state inputs
            if (keyboardState.IsKeyUp(Keys.Escape))
                this.InitialEscapeReleased = true;
            if (this.InitialEscapeReleased && keyboardState.IsKeyDown(Keys.Escape))
            {
                game.ChangeState(gameState);
                gameState.stopWatch.Start();
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            // spriteBatch.Draw(Background, new Rectangle(0, 0, MyGame.VirtualWidth, MyGame.VirtualHeight), Color.White);
            foreach (var component in components)
                component.Draw(gameTime, spriteBatch);

            foreach (var component in components)
                component.PostDraw(gameTime, spriteBatch);

            spriteBatch.End();
        }

        private void UpdateKeyboardState()
        {
            keyboardState = Keyboard.GetState();
        }

        /*
        * Button Events
        */

        private void Button_Resume_Clicked(object sender, EventArgs e)
        {
            game.ChangeState(gameState);
            gameState.stopWatch.Start();
        }

        private void Button_Save_Clicked(object sender, EventArgs e)
        {
            gameState.SaveGame();
        }

        private void Button_Quit_Clicked(object sender, EventArgs e)
        {
            game.ChangeState(new MenuState(game, content));
        }
    }
}