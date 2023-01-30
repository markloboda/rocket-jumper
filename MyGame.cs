using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        public JObject Settings;

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
                File.WriteAllText("Content/settings.json", Settings.ToString());
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
            Settings = JObject.Parse(File.ReadAllText("Content/settings.json"));

            IsMouseVisible = true;

            // screen properties
            PrefferedResolution = new Vector2(Settings["resolution"]["width"].ToObject<int>(), Settings["resolution"]["height"].ToObject<int>());
            graphics.ApplyChanges();

            this.Window.IsBorderless = false;

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
            // save time to high_scores.json
            dynamic json = JObject.Parse(File.ReadAllText("Content/high_scores.json"));

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

            File.WriteAllText("Content/high_scores.json", json.ToString());
        }
    }
}