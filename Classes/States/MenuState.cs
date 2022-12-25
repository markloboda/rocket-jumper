using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RocketJumper.Classes.Controls;

namespace RocketJumper.Classes.States
{
    public class MenuState : State
    {
        private List<Component> components;

        private List<Component> mainMenuComponents;
        private List<Component> optionsComponents;

        private Texture2D background;

        public MenuState(MyGame game, ContentManager content)
            : base(game, content)
        {
        }

        public override void LoadContent()
        {
            var buttonFont = game.Font;

            mainMenuComponents = new List<Component>();
            mainMenuComponents.Add(
            new Button(Tools.GetSingleColorTexture(game.GraphicsDevice, Color.White), buttonFont)
            {
                Position = new Vector2(MyGame.ScreenWidth / 2, 100),
                Text = "Play",
                Click = new EventHandler(Button_Play_Clicked)
            });

            mainMenuComponents.Add(
            new Button(Tools.GetSingleColorTexture(game.GraphicsDevice, Color.White), buttonFont)
            {
                Position = new Vector2(MyGame.ScreenWidth / 2, 200),
                Text = "Options",
                Click = new EventHandler(Button_Options_Clicked)
            });

            mainMenuComponents.Add(
            new Button(Tools.GetSingleColorTexture(game.GraphicsDevice, Color.White), buttonFont)
            {
                Position = new Vector2(MyGame.ScreenWidth / 2, 300),
                Text = "Exit",
                Click = new EventHandler(Button_Exit_Clicked)
            });

            optionsComponents = new List<Component>();
            optionsComponents.Add(
            new Button(Tools.GetSingleColorTexture(game.GraphicsDevice, Color.White), buttonFont)
            {
                Position = new Vector2(MyGame.ScreenWidth / 2, 100),
                Text = "Back",
                Click = new EventHandler(Button_Options_Back_Clicked)
            });

            components = mainMenuComponents;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in components)
                component.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            foreach (var component in components)
                component.Draw(gameTime, spriteBatch);

            spriteBatch.End();
        }

        private void Button_Play_Clicked(object sender, EventArgs e)
        {
            game.ChangeState(new GameState(game, content));
        }

        private void Button_Options_Clicked(object sender, EventArgs e)
        {
            components = optionsComponents;            
        }

        private void Button_Exit_Clicked(object sender, EventArgs e)
        {
            game.Exit();
        }

        private void Button_Options_Back_Clicked(object sender, EventArgs e)
        {
            components = mainMenuComponents;
        }
    }
}