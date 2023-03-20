using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json.Linq;
using RocketJumper.Classes.States;


namespace RocketJumper
{
    public enum eWindowMode
    {
        Windowed,
        Borderless,
        Fullscreen
    }


    public class MyGame : Game
    {

        [DllImport("user32.dll")]
        static extern void ClipCursor(ref Rectangle rect);

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

        public string SettingsFilePath
        {
            get
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "RocketJumper/settings.json");
                if (!File.Exists(path))
                {
                    Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "RocketJumper"));
                    File.WriteAllText(path, File.ReadAllText("Content/defaultSettings.json"));
                }
                return path;
            }
        }

        public static string LocalScoresFilePath
        {
            get
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "RocketJumper/local_scores.json");
                if (!File.Exists(path))
                {
                    Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "RocketJumper"));
                    File.WriteAllText(path, "[]");
                }
                return path;
            }
        }

        public static string GlobalScoresFilePath
        {
            get
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "RocketJumper/online_scores.json");
                if (!File.Exists(path))
                {
                    Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "RocketJumper"));
                    File.WriteAllText(path, "[]");
                }
                return path;
            }
        }

        public string ReplayFolderDirectory
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "RocketJumper/replays/"); }
        }

        public string CurrentDateReplayId
        {
            get
            {
                string replayId = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".bin";
                return replayId;
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

        public eWindowMode WindowMode
        {
            get
            {
                return Settings["windowType"].ToObject<eWindowMode>();
            }
            set
            {
                if (value == WindowMode)
                    return;
                // save previous resolution
                if (WindowMode == eWindowMode.Windowed)
                {
                    Settings["windowedResolution"]["width"] = graphics.PreferredBackBufferWidth;
                    Settings["windowedResolution"]["height"] = graphics.PreferredBackBufferHeight;
                }
                else
                {
                    Settings["fullscreenResolution"]["width"] = graphics.PreferredBackBufferWidth;
                    Settings["fullscreenResolution"]["height"] = graphics.PreferredBackBufferHeight;
                }
                File.WriteAllText(SettingsFilePath, Settings.ToString());

                switch (value)
                {
                    case eWindowMode.Windowed:
                        SetWindowed();
                        break;
                    case eWindowMode.Borderless:
                        SetFullscreen(isBorderless: true);
                        break;
                    case eWindowMode.Fullscreen:
                        SetFullscreen(isBorderless: false);
                        break;
                }

                Settings["windowType"] = value.ToString();
                File.WriteAllText(SettingsFilePath, Settings.ToString());
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

            // window properties
            WindowMode = Settings["windowType"].ToObject<eWindowMode>();

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

            if (this.IsActive && (WindowMode == eWindowMode.Borderless || WindowMode == eWindowMode.Fullscreen))
            {
                Rectangle rect = Window.ClientBounds;
                ClipCursor(ref rect);
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
            nextState.InitialEscapeReleased = false;
        }

        public Texture2D GetFrame()
        {
            Texture2D texture = new Texture2D(GraphicsDevice, ActualWidth, ActualHeight);
            Color[] data = new Color[ActualWidth * ActualHeight];
            GraphicsDevice.GetBackBufferData(data);
            texture.SetData(data);
            return texture;
        }

        public void SaveTime(string mapPath, long milliseconds, string replayId)
        {
            // save time to local_scores.json
            dynamic json = JArray.Parse(File.ReadAllText(LocalScoresFilePath));

            // make jobject to save time
            JObject save = new JObject();
            save["username"] = USERNAME;
            save["score"] = milliseconds;
            save["date"] = System.DateTime.Now.ToString("dd/MM/yyyy");
            save["map"] = mapPath;
            save["replayId"] = replayId;

            json.Add(save);

            File.WriteAllText(LocalScoresFilePath, json.ToString());
        }

        public void SetWindowed()
        {
            // set resolution
            PrefferedResolution = new Vector2(Settings["windowedResolution"]["width"].ToObject<int>(), Settings["windowedResolution"]["height"].ToObject<int>());

            // unset fullscreen
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
        }

        public void SetFullscreen(bool isBorderless = false)
        {
            // set resolution
            PrefferedResolution = new Vector2(Settings["fullscreenResolution"]["width"].ToObject<int>(), Settings["fullscreenResolution"]["height"].ToObject<int>());

            // set fullscreen
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.HardwareModeSwitch = !isBorderless;

            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
        }

        public List<ReplayData> LoadReplay(string filePath)
        {
            var dataList = new List<ReplayData>();

            // load binarized replay from ReplayFile
            using (Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        byte[] buffer = reader.ReadBytes(Marshal.SizeOf(typeof(ReplayData)));
                        dataList.Add(toReplayData(buffer));
                    }
                }
            }

            return dataList;
        }

        private ReplayData toReplayData(byte[] buffer)
        {
            ReplayData aux = new ReplayData();
            int length = Marshal.SizeOf(aux);
            IntPtr ptr = Marshal.AllocHGlobal(length);

            Marshal.Copy(buffer, 0, ptr, length);

            aux = (ReplayData)Marshal.PtrToStructure(ptr, aux.GetType());
            Marshal.FreeHGlobal(ptr);

            return aux;
        }

        public void SaveReplay(string replayId, List<ReplayData> dataList)
        {
            // save binarized replay to ReplayFile 
            using (Stream stream = new FileStream(getReplayFile(replayId), FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    // get all data
                    foreach (ReplayData data in dataList)
                    {
                        byte[] newBuffer = toBytes(data);
                        writer.Write(newBuffer);
                    }
                }
            }
        }

        private byte[] toBytes(ReplayData aux)
        {
            int length = Marshal.SizeOf(aux);
            IntPtr ptr = Marshal.AllocHGlobal(length);
            byte[] myBuffer = new byte[length];

            Marshal.StructureToPtr(aux, ptr, true);
            Marshal.Copy(ptr, myBuffer, 0, length);
            Marshal.FreeHGlobal(ptr);

            return myBuffer;
        }

        private string getReplayFile(string replayId)
        {
            string path = ReplayFolderDirectory;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            // generate unique replay id based on current time
            path = Path.Combine(path, replayId);

            return path;
        }
    }
}