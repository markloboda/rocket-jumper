using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using RocketJumper.Classes.Controls;

namespace RocketJumper.Classes.States
{
    public class MenuState : State
    {
        private List<Component> components;

        // enum for menu state
        private enum MenuStateEnum
        {
            MainMenu,
            Options,
            Highscores
        }

        private MenuStateEnum currentMenuState;

        private List<Component> mainMenuComponents;
        private List<Component> optionsComponents;
        private List<Component> highscoresComponents;

        private float mainMenuHeightUnit;

        // content
        public Dictionary<string, SoundEffect> SoundEffects = new();

        private Texture2D background;

        // settings
        private TextComponent volumeText;


        public MenuState(MyGame game, ContentManager content)
            : base(game, content)
        {
        }

        public override void LoadContent()
        {
            SetupPosition();
            ChangeMenuState(MenuStateEnum.MainMenu);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in components)
                component.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            // draw title
            spriteBatch.DrawString(game.TitleFont, "Rocket Jumper", new Vector2(MyGame.ActualWidth / 7, MyGame.ActualHeight - 7 * mainMenuHeightUnit), Color.White, 0, game.Font.MeasureString("Rocket Jumper") * 0.65f, 1, SpriteEffects.None, 0);

            foreach (var component in components)
                component.Draw(gameTime, spriteBatch);

            spriteBatch.End();

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // Draw character

            spriteBatch.End();
        }

        public void SetupPosition()
        {
            mainMenuHeightUnit = MyGame.ActualHeight / 15;

            var buttonFont = game.Font;

            SoundEffects["gameStart"] = content.Load<SoundEffect>("Audio/gameStart");

            mainMenuComponents = new List<Component>();
            mainMenuComponents.Add(
            new Button(Tools.GetSingleColorTexture(game.GraphicsDevice, Color.White), buttonFont)
            {
                Position = new Vector2(MyGame.ActualWidth / 5, MyGame.ActualHeight - 5 * mainMenuHeightUnit),
                Text = "Play",
                Click = new EventHandler(Button_Play_Clicked)
            });

            mainMenuComponents.Add(
            new Button(Tools.GetSingleColorTexture(game.GraphicsDevice, Color.White), buttonFont)
            {
                Position = new Vector2(MyGame.ActualWidth / 5, MyGame.ActualHeight - 4 * mainMenuHeightUnit),
                Text = "Options",
                Click = new EventHandler(Button_Options_Clicked)
            });

            mainMenuComponents.Add(
            new Button(Tools.GetSingleColorTexture(game.GraphicsDevice, Color.White), buttonFont)
            {
                Position = new Vector2(MyGame.ActualWidth / 5, MyGame.ActualHeight - 3 * mainMenuHeightUnit),
                Text = "Highscores",
                Click = new EventHandler(Button_Highscores_Clicked)
            });

            mainMenuComponents.Add(
            new Button(Tools.GetSingleColorTexture(game.GraphicsDevice, Color.White), buttonFont)
            {
                Position = new Vector2(MyGame.ActualWidth / 5, MyGame.ActualHeight - 2 * mainMenuHeightUnit),
                Text = "Exit",
                Click = new EventHandler(Button_Exit_Clicked)
            });

            optionsComponents = new List<Component>();
            optionsComponents.Add(
            new Button(Tools.GetSingleColorTexture(game.GraphicsDevice, Color.White), buttonFont)
            {
                Position = new Vector2(MyGame.ActualWidth * 0.65f, MyGame.ActualHeight - 9 * mainMenuHeightUnit),
                Text = "Back",
                Click = new EventHandler(Button_Options_Back_Clicked)
            });

            // check screen resolution and set Options
            int screenWidthPixels = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int screenHeightPixels = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            var options = new List<string>();
            if (screenWidthPixels >= 2560 && screenHeightPixels >= 1440)
                options.Add("2560x1440");
            if (screenWidthPixels >= 1920 && screenHeightPixels >= 1080)
                options.Add("1920x1080");
            if (screenWidthPixels >= 1280 && screenHeightPixels >= 720)
                options.Add("1280x720");

            // check selected resolution
            int windowHeight = MyGame.ActualHeight;
            int windowWidth = MyGame.ActualWidth;

            int selectedResolution = 0;
            for (int i = 0; i < options.Count; i++)
            {
                string[] resolution = options[i].Split('x');
                if (windowWidth == int.Parse(resolution[0]) && windowHeight == int.Parse(resolution[1]))
                {
                    selectedResolution = i;
                    break;
                }
            }

            optionsComponents.Add(
            new TextComponent(buttonFont)
            {
                Position = new Vector2(MyGame.ActualWidth * 0.65f - 300, MyGame.ActualHeight - 8 * mainMenuHeightUnit),
                Text = "Resolution"
            });

            optionsComponents.Add(
            new Dropdown(Tools.GetSingleColorTexture(game.GraphicsDevice, Color.White), buttonFont)
            {
                Position = new Vector2(MyGame.ActualWidth * 0.65f, MyGame.ActualHeight - 8 * mainMenuHeightUnit),
                Options = options,
                SelectedIndex = selectedResolution,
                SelectChange = new EventHandler(Options_Resolution_Changed)
            });

            optionsComponents.Add(
            new TextComponent(buttonFont)
            {
                Position = new Vector2(MyGame.ActualWidth * 0.65f - 300, MyGame.ActualHeight - 7 * mainMenuHeightUnit),
                Text = "Volume"
            });

            optionsComponents.Add(
            new Button(Tools.GetSingleColorTexture(game.GraphicsDevice, Color.White), buttonFont)
            {
                Position = new Vector2(MyGame.ActualWidth * 0.65f - 150, MyGame.ActualHeight - 7 * mainMenuHeightUnit),
                Text = "<",
                Click = new EventHandler(Options_Volume_Down_Clicked)
            });

            volumeText = new TextComponent(buttonFont)
            {
                Position = new Vector2(MyGame.ActualWidth * 0.65f, MyGame.ActualHeight - 7 * mainMenuHeightUnit),
                Text = (game.Volume * 10).ToString()
            };
            optionsComponents.Add(volumeText);

            optionsComponents.Add(
            new Button(Tools.GetSingleColorTexture(game.GraphicsDevice, Color.White), buttonFont)
            {
                Position = new Vector2(MyGame.ActualWidth * 0.65f + 150, MyGame.ActualHeight - 7 * mainMenuHeightUnit),
                Text = ">",
                Click = new EventHandler(Options_Volume_Up_Clicked)
            });


            highscoresComponents = new List<Component>();
            highscoresComponents.Add(
            new Button(Tools.GetSingleColorTexture(game.GraphicsDevice, Color.White), buttonFont)
            {
                Position = new Vector2(MyGame.ActualWidth * 0.65f, 100),
                Text = "Back",
                Click = new EventHandler(Button_Highscores_Back_Clicked)
            });
        }


        /*
        * Button Events
        */

        private void Button_Play_Clicked(object sender, EventArgs e)
        {
            // SoundEffects["gameStart"].Play();
            game.ChangeState(new GameState(game, content));
        }

        private void Button_Options_Clicked(object sender, EventArgs e)
        {
            ChangeMenuState(MenuStateEnum.Options);
        }

        private void Button_Highscores_Clicked(object sender, EventArgs e)
        {
            // load highscores
            dynamic json = JObject.Parse(File.ReadAllText("Content/high_scores.json"));

            // add highscores to highscoresComponents
            foreach (var username in json)
            {
                int i = 0;
                foreach (var score in username.Value)
                {
                    highscoresComponents.Add(
                    new Score(game.Font)
                    {
                        Position = new Vector2(MyGame.ActualWidth * 0.65f, 200 + i * 50),
                        Username = username.Name,
                        Time = long.Parse(score["time"].ToString()),
                        Date = score["date"].ToString()
                    });
                    i++;
                }
            }

            ChangeMenuState(MenuStateEnum.Highscores);
        }

        private void Button_Exit_Clicked(object sender, EventArgs e)
        {
            game.Exit();
        }

        private void Button_Options_Back_Clicked(object sender, EventArgs e)
        {
            ChangeMenuState(MenuStateEnum.MainMenu);
        }

        private void Button_Highscores_Back_Clicked(object sender, EventArgs e)
        {
            ChangeMenuState(MenuStateEnum.MainMenu);
        }

        private void Options_Resolution_Changed(object sender, EventArgs e)
        {
            var dropdown = sender as Dropdown;
            var resolution = dropdown.Options[dropdown.SelectedIndex].Split('x');

            game.PrefferedResolution = new Vector2(int.Parse(resolution[0]), int.Parse(resolution[1]));

            SetupPosition();
            ChangeMenuState(MenuStateEnum.Options);
        }

        private void Options_Volume_Down_Clicked(object sender, EventArgs e)
        {
            if (game.Volume > 0)
            {
                game.Volume -= 0.1f;
                volumeText.Text = ((int)(game.Volume * 10)).ToString();
            }
        }

        private void Options_Volume_Up_Clicked(object sender, EventArgs e)
        {
            if (game.Volume < 1)
            {
                game.Volume += 0.1f;
                volumeText.Text = ((int)(game.Volume * 10)).ToString();
            }
        }

        private void ChangeMenuState(MenuStateEnum menuState)
        {
            if (menuState == MenuStateEnum.MainMenu)
            {
                components = mainMenuComponents;
            }
            else if (menuState == MenuStateEnum.Options)
            {
                components = optionsComponents;
            }
            else if (menuState == MenuStateEnum.Highscores)
            {
                components = highscoresComponents;
            }
        }
    }
}