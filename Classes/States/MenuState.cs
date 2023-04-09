using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using RocketJumper.Classes.Controls;
using RocketJumper.Classes.MapData;

namespace RocketJumper.Classes.States
{
    public class MenuState : State
    {
        private List<Component> components
        {
            get
            {
                switch (currentMenuState)
                {
                    case MenuStateEnum.MainMenu:
                        return mainMenuComponents;
                    case MenuStateEnum.Options:
                        return optionsComponents;
                    case MenuStateEnum.Highscores:
                        return Enumerable.Concat(highscoresComponents, highscoresComponents_Scores).ToList();
                    default:
                        return mainMenuComponents;
                }
            }
        }

        // enum for menu state
        private enum MenuStateEnum
        {
            MainMenu,
            Options,
            Highscores,
            Username
        }

        private MenuStateEnum currentMenuState;
        private string currentMenuTitle
        {
            get
            {
                switch (currentMenuState)
                {
                    case MenuStateEnum.MainMenu:
                        return "Rocket Jumper";
                    case MenuStateEnum.Options:
                        return "Options";
                    case MenuStateEnum.Highscores:
                        return "Highscores";
                    default:
                        return "Main Menu";
                }
            }
        }

        private List<Component> mainMenuComponents;
        private List<Component> optionsComponents;
        private List<Component> highscoresComponents;
        private List<Component> highscoresComponents_Scores;

        private float mainMenuHeightUnit;

        // content
        public Dictionary<string, SoundEffect> SoundEffects = new();

        private Texture2D gameBackground;
        private Texture2D menuBackground;

        private Animation_s playerIdleAnimation;
        private int currentFrameId;
        private float timer;
        private Rectangle idleSourceRectangle;

        // settings
        private TextComponent volumeText;

        // highscores
        private bool isDisplayingGlobalHighscores = false;
        private Button globalHighscoresButton;
        private Button localHighscoresButton;


        public MenuState(MyGame game, ContentManager content)
            : base(game, content)
        {
        }

        public override void LoadContent()
        {
            // load background
            gameBackground = content.Load<Texture2D>("Background/GameBackground");
            menuBackground = content.Load<Texture2D>("Background/MenuBackground");

            // load character idle animation
            playerIdleAnimation = new Animation_s(content.Load<Texture2D>("Sprites/Player/Idle"), 5, 0.2f);
            // default to first frame
            currentFrameId = 0;

            SetupPosition();
            ChangeMenuState(MenuStateEnum.MainMenu);
        }

        public override void Update(GameTime gameTime)
        {
            // update character
            // handle frames
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            while (timer > playerIdleAnimation.FrameTime)
            {
                timer -= playerIdleAnimation.FrameTime;
                currentFrameId = (currentFrameId + 1) % playerIdleAnimation.FrameCount;
            }
            // set source rectangle
            idleSourceRectangle = GetSourceRectangle(currentFrameId);

            foreach (var component in components.ToArray())
                component.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // draw background
            spriteBatch.Begin();
            spriteBatch.Draw(gameBackground, new Rectangle(0, 0, MyGame.ActualWidth, gameBackground.Height), Color.White);

            // draw menu title
            spriteBatch.DrawString(game.TitleFont, currentMenuTitle, new Vector2(MyGame.ActualWidth / 7, MyGame.ActualHeight - 8 * mainMenuHeightUnit), Color.DarkGreen, 0, game.Font.MeasureString(currentMenuTitle) * 0.65f, 1, SpriteEffects.None, 0);
            spriteBatch.End();
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // scale of menu background
            float scale = 10;
            var tilesPosition = new Vector2(MyGame.ActualWidth - menuBackground.Width * scale, MyGame.ActualHeight - 3 * mainMenuHeightUnit);
            spriteBatch.Draw(menuBackground, new Vector2(MyGame.ActualWidth - menuBackground.Width * scale, MyGame.ActualHeight - 3 * mainMenuHeightUnit), null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);

            // draw character
            var characterPosition = new Vector2(tilesPosition.X + menuBackground.Width * scale / 2, tilesPosition.Y - playerIdleAnimation.FrameHeight * scale);
            spriteBatch.Draw(playerIdleAnimation.Texture, characterPosition, idleSourceRectangle, Color.White, 0, Vector2.Zero, scale, SpriteEffects.FlipHorizontally, 0);

            spriteBatch.End();


            spriteBatch.Begin();

            foreach (var component in components)
                component.Draw(gameTime, spriteBatch);

            foreach (var component in components)
                component.PostDraw(gameTime, spriteBatch);

            spriteBatch.End();
        }

        public void SetupPosition()
        {
            mainMenuHeightUnit = MyGame.ActualHeight / 15;

            var buttonFont = game.Font;

            try
            {
                SoundEffects["gameStart"] = content.Load<SoundEffect>("Audio/gameStart");
            }
            catch (NoAudioHardwareException e)
            {
                Console.WriteLine(e.Message);
            }

            if (File.Exists(GameState.SaveFilePath))
            {
                mainMenuComponents = new List<Component>();
                mainMenuComponents.Add(
                new Button(Tools.GetSingleColorTexture(game.GraphicsDevice, Color.White), buttonFont)
                {
                    Position = new Vector2(MyGame.ActualWidth / 5, MyGame.ActualHeight - 6 * mainMenuHeightUnit),
                    Text = "Resume",
                    Click = new EventHandler(Button_Play_Clicked)
                });

                mainMenuComponents.Add(
                new Button(Tools.GetSingleColorTexture(game.GraphicsDevice, Color.White), buttonFont)
                {
                    Position = new Vector2(MyGame.ActualWidth / 5, MyGame.ActualHeight - 5 * mainMenuHeightUnit),
                    Text = "Reset Save",
                    Click = new EventHandler(Button_Reset_Save_Clicked)
                });
            }
            else
            {
                mainMenuComponents = new List<Component>();
                mainMenuComponents.Add(
                new Button(Tools.GetSingleColorTexture(game.GraphicsDevice, Color.White), buttonFont)
                {
                    Position = new Vector2(MyGame.ActualWidth / 5, MyGame.ActualHeight - 5 * mainMenuHeightUnit),
                    Text = "Play",
                    Click = new EventHandler(Button_Play_Clicked)
                });
            }

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

            mainMenuComponents.Add(
            new TextComponent(buttonFont)
            {
                Position = new Vector2(MyGame.ActualWidth / 5, MyGame.ActualHeight - 7 * mainMenuHeightUnit),
                Text = "Online highscores are not available at the moment.",
                Color = Color.Red
            });


            optionsComponents = new List<Component>();
            optionsComponents.Add(
            new Button(Tools.GetSingleColorTexture(game.GraphicsDevice, Color.White), buttonFont)
            {
                Position = new Vector2(MyGame.ActualWidth * 0.65f, MyGame.ActualHeight - 10 * mainMenuHeightUnit),
                Text = "Back",
                Click = new EventHandler(Button_Options_Back_Clicked)
            });

            optionsComponents.Add(
            new TextComponent(buttonFont)
            {
                Position = new Vector2(MyGame.ActualWidth * 0.65f - 300, MyGame.ActualHeight - 9 * mainMenuHeightUnit),
                Text = "Borderless"
            });

            var optionsWindowMode = new List<string> { "Windowed", "Borderless", "Fullscreen" };
            int selectedWindow = 0;
            for (int i = 0; i < optionsWindowMode.Count; i++)
            {
                string windowMode = optionsWindowMode[i];
                if (windowMode == game.WindowMode.ToString())
                {
                    selectedWindow = i;
                    break;
                }
            }

            optionsComponents.Add(
            new Dropdown(Tools.GetSingleColorTexture(game.GraphicsDevice, Color.White), buttonFont)
            {
                Position = new Vector2(MyGame.ActualWidth * 0.65f, MyGame.ActualHeight - 9 * mainMenuHeightUnit),
                Options = optionsWindowMode,
                SelectedIndex = selectedWindow,
                SelectChange = new EventHandler(Options_Window_Changed)
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

            optionsComponents.Add(
            new TextComponent(buttonFont)
            {
                Position = new Vector2(MyGame.ActualWidth * 0.65f - 300, MyGame.ActualHeight - 8 * mainMenuHeightUnit),
                Text = "Resolution"
            });

            // check screen resolution and set Options
            int screenWidthPixels = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int screenHeightPixels = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            var optionsResolution = new List<string>();
            if (screenWidthPixels >= 2560 && screenHeightPixels >= 1440)
                optionsResolution.Add("2560x1440");
            if (screenWidthPixels >= 1920 && screenHeightPixels >= 1080)
                optionsResolution.Add("1920x1080");
            if (screenWidthPixels >= 1280 && screenHeightPixels >= 720)
                optionsResolution.Add("1280x720");

            // check selected resolution
            int windowHeight = MyGame.ActualHeight;
            int windowWidth = MyGame.ActualWidth;

            int selectedResolution = 0;
            for (int i = 0; i < optionsResolution.Count; i++)
            {
                string[] resolution = optionsResolution[i].Split('x');
                if (windowWidth == int.Parse(resolution[0]) && windowHeight == int.Parse(resolution[1]))
                {
                    selectedResolution = i;
                    break;
                }
            }

            optionsComponents.Add(
            new Dropdown(Tools.GetSingleColorTexture(game.GraphicsDevice, Color.White), buttonFont)
            {
                Position = new Vector2(MyGame.ActualWidth * 0.65f, MyGame.ActualHeight - 8 * mainMenuHeightUnit),
                Options = optionsResolution,
                SelectedIndex = selectedResolution,
                SelectChange = new EventHandler(Options_Resolution_Changed)
            });


            highscoresComponents = new List<Component>();
            highscoresComponents_Scores = new List<Component>();
            highscoresComponents.Add(
            new Button(Tools.GetSingleColorTexture(game.GraphicsDevice, Color.White), buttonFont)
            {
                Position = new Vector2(MyGame.ActualWidth * 0.55f, 100),
                Text = "Back",
                Click = new EventHandler(Button_Highscores_Back_Clicked)
            });

            globalHighscoresButton = new Button(Tools.GetSingleColorTexture(game.GraphicsDevice, Color.White), game.Font)
            {
                Position = new Vector2(MyGame.ActualWidth * 0.90f - 50, 100),
                Text = "Global",
                Click = new EventHandler(Button_Highscores_Global_Clicked),
                IsVisible = false
            };

            localHighscoresButton = new Button(Tools.GetSingleColorTexture(game.GraphicsDevice, Color.White), game.Font)
            {
                Position = new Vector2(MyGame.ActualWidth * 0.90f - 250, 100),
                Text = "Local",
                Click = new EventHandler(Button_Highscores_Local_Clicked),
                IsVisible = false
            };

            highscoresComponents.Add(globalHighscoresButton);
            highscoresComponents.Add(localHighscoresButton);
        }


        /*
        * Button Events
        */

        private void Button_Play_Clicked(object sender, EventArgs e)
        {
            if (SoundEffects.ContainsKey("gameStart"))
                SoundEffects["gameStart"].Play();
                
            game.ChangeState(new GameState(game, content));
        }

        private void Button_Reset_Save_Clicked(object sender, EventArgs e)
        {
            // reset save
            File.Delete(GameState.SaveFilePath);

            SetupPosition();
        }

        private void Button_Replay_Clicked(object sender, EventArgs e)
        {
            var path = ((ReplayEventArgs)((Button)sender).EventArgs).ReplayId;
            if (path == null)
                return;

            var replayGameState = new GameState(game, content);
            game.ChangeState(replayGameState);
            replayGameState.StartReplay(game.ReplayFolderDirectory + path);
        }

        private void Button_Options_Clicked(object sender, EventArgs e)
        {
            ChangeMenuState(MenuStateEnum.Options);
        }

        private void Button_Highscores_Clicked(object sender, EventArgs e)
        {
            RefreshScores();
            ChangeMenuState(MenuStateEnum.Highscores);
        }

        private void RefreshScores()
        {
            highscoresComponents_Scores.Clear();

            dynamic json;
            if (isDisplayingGlobalHighscores)
                json = JArray.Parse(File.ReadAllText(MyGame.GlobalScoresFilePath));
            else
                json = JArray.Parse(File.ReadAllText(MyGame.LocalScoresFilePath));

            // add highscores to highscoresComponents
            int i = 0;
            foreach (var score in json)
            {
                highscoresComponents_Scores.Add(
                new Score(game.Font)
                {
                    Position = new Vector2(MyGame.ActualWidth * 0.65f, 200 + i * 50),
                    Username = score["username"].ToString(),
                    ScoreTime = long.Parse(score["score"].ToString()),
                    Date = score["date"].ToString()
                });

                highscoresComponents_Scores.Add(
                new Button(Tools.GetSingleColorTexture(game.GraphicsDevice, Color.White), game.Font)
                {
                    Position = new Vector2(MyGame.ActualWidth * 0.85f, 200 + i * 50),
                    Text = "Watch Replay",
                    EventArgs = new ReplayEventArgs(score["replayId"]?.ToString()),
                    Click = new EventHandler(Button_Replay_Clicked)
                });

                i++;

            }

            // load highscores from network with NetworkLeaderboards.GetLeaderboards
            NetworkLeaderboards.GetLeaderboards(null).GetAwaiter().OnCompleted(() =>
            {
                globalHighscoresButton.IsVisible = true;
                localHighscoresButton.IsVisible = true;
            });
        }

        private void Button_Highscores_Global_Clicked(object sender, EventArgs e)
        {
            isDisplayingGlobalHighscores = true;
            globalHighscoresButton.IsDarkened = true;
            localHighscoresButton.IsDarkened = false;
            RefreshScores();
        }

        private void Button_Highscores_Local_Clicked(object sender, EventArgs e)
        {
            isDisplayingGlobalHighscores = false;
            globalHighscoresButton.IsDarkened = false;
            localHighscoresButton.IsDarkened = true;
            RefreshScores();
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

        private void Options_Window_Changed(object sender, EventArgs e)
        {
            var dropdown = sender as Dropdown;
            var option = dropdown.Options[dropdown.SelectedIndex];
            switch (option)
            {
                case "Windowed":
                    game.WindowMode = eWindowMode.Windowed;
                    break;
                case "Borderless":
                    game.WindowMode = eWindowMode.Borderless;
                    break;
                case "Fullscreen":
                    game.WindowMode = eWindowMode.Fullscreen;
                    break;
            }
            SetupPosition();
            ChangeMenuState(MenuStateEnum.Options);
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
                currentMenuState = MenuStateEnum.MainMenu;
            else if (menuState == MenuStateEnum.Options)
                currentMenuState = MenuStateEnum.Options;
            else if (menuState == MenuStateEnum.Highscores)
                currentMenuState = MenuStateEnum.Highscores;
        }


        private Rectangle GetSourceRectangle(int frameIndex)
        {
            return new Rectangle(frameIndex * playerIdleAnimation.FrameWidth, 0, playerIdleAnimation.FrameWidth, playerIdleAnimation.FrameHeight);
        }

    }
}