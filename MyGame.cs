using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json.Linq;
using RocketJumper.Classes.States;

namespace RocketJumper
{
    public class MyGame : Game
    {
        string USERNAME = "mark";

        private State currentState;
        private State nextState;

        // content
        public SpriteFont Font;
        public SpriteFont TitleFont;


        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public static int VirtualWidth;
        public static int VirtualHeight;
        public static int ActualWidth;
        public static int ActualHeight;

        public string SettingsFilePath {
            get {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "RocketJumper/settings.json");
                if (!File.Exists(path)) {
                    Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "RocketJumper"));
                    File.WriteAllText(path, File.ReadAllText("Content/defaultSettings.json"));
                }
                return path;
            }
        }

        public string ScoresFilePath {
            get {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "RocketJumper/scores.json");
                if (!File.Exists(path)) {
                    Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "RocketJumper"));
                    File.WriteAllText(path, "{}");
                }
                return path;
            }
        }

        public JObject Settings;
        public float Volume
        {
            get
            {
                return Settings["volume"].ToObject<float>();
            }
            set
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), SettingsFilePath);
                var volume = (float)Math.Round(value, 1);
                Settings["volume"] = volume;
                SoundEffect.MasterVolume = volume;
                File.WriteAllText(path, Settings.ToString());
            }
        }

        public Vector2 PrefferedResolution
        {
            get
            {
                return new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            }
            set
            {
                graphics.PreferredBackBufferWidth = (int)value.X;
                graphics.PreferredBackBufferHeight = (int)value.Y;
                graphics.ApplyChanges();

                ActualWidth = graphics.PreferredBackBufferWidth;
                ActualHeight = graphics.PreferredBackBufferHeight;

                // Write to settings.json
                Settings["resolution"]["width"] = ActualWidth;
                Settings["resolution"]["height"] = ActualHeight;
                File.WriteAllText(SettingsFilePath, Settings.ToString());
            }
        }

        public bool Borderless
        {
            get
            {
                return this.Window.IsBorderless;
            }
            set
            {
                this.Window.IsBorderless = value;
            }
        }

        public MyGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Settings = JObject.Parse(File.ReadAllText(SettingsFilePath));

            IsMouseVisible = true;

            // settings
            SoundEffect.MasterVolume = Settings["volume"].ToObject<float>();

            // screen properties
            PrefferedResolution = new Vector2(Settings["resolution"]["width"].ToObject<int>(), Settings["resolution"]["height"].ToObject<int>());
            graphics.ApplyChanges();

            Borderless = Settings["borderless"].ToObject<bool>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Font = Content.Load<SpriteFont>("Fonts/TimerFont");
            TitleFont = Content.Load<SpriteFont>("Fonts/TitleFont");

            currentState = new MenuState(this, Content);
            currentState.LoadContent();
            nextState = null;

            var mouseTexture = Content.Load<Texture2D>("UI/Cursor");
            Mouse.SetCursor(MouseCursor.FromTexture2D(mouseTexture, mouseTexture.Width / 2, mouseTexture.Height / 2));
        }

        protected override void Update(GameTime gameTime)
        {
            if (nextState != null)
            {
                currentState = nextState;
                if (!nextState.IsPaused)
                    currentState.LoadContent();
                nextState = null;
            }

            currentState.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            currentState.Draw(gameTime, spriteBatch);
            base.Draw(gameTime);
        }

        public void ChangeState(State state)
        {
            nextState.InitialEscapeReleased = false;
            nextState = state;
        }

        public Texture2D GetFrame()
        {
            Texture2D texture = new Texture2D(GraphicsDevice, ActualWidth, ActualHeight);
            Color[] data = new Color[ActualWidth * ActualHeight];
            GraphicsDevice.GetBackBufferData(data);
            texture.SetData(data);
            return texture;
        }

        public void SaveTime(string mapPath, long milliseconds)
        {
            // save time to scores.json
            dynamic json = JObject.Parse(File.ReadAllText(ScoresFilePath));

            // make jobject to save time
            JObject save = new JObject();
            save["time"] = milliseconds;
            save["date"] = System.DateTime.Now.ToString("dd/MM/yyyy");
            save["map"] = mapPath;

            if (json[USERNAME] == null)
            {
                json[USERNAME] = new JArray();
            }
            json[USERNAME].Add(save);

            File.WriteAllText(ScoresFilePath, json.ToString());
        }
    }
}