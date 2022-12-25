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
        private Texture2D background;

        public MenuState(MyGame game, ContentManager content)
            : base(game, content)
        {
        }

        public override void LoadContent()
        {
            var buttonFont = game.Font;

            components = new List<Component>();
            components.Add(
            new Button(Tools.GetSingleColorTexture(game.GraphicsDevice, Color.White), buttonFont)
            {
                Position = new Vector2(MyGame.ScreenWidth / 2, 100),
                Text = "Play",
                Click = new EventHandler(Button_Play_Clicked)
            });

            components.Add(
            new Button(Tools.GetSingleColorTexture(game.GraphicsDevice, Color.White), buttonFont)
            {
                Position = new Vector2(MyGame.ScreenWidth / 2, 200),
                Text = "Exit",
                Click = new EventHandler(Button_Exit_Clicked)
            });
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
    
        private void Button_Exit_Clicked(object sender, EventArgs e)
        {
            game.Exit();
        }
    }
}