
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RocketJumper.Classes.MapData;

public struct ReplayData
{
    public Vector2 PlayerPosition;
}

namespace RocketJumper.Classes.States
{
    public class GameState : State
    {
        // const
        public const string MapFilePath = "Content/Levels/main_map-tiled.json";
        public const float PlayerScale = 2.5f;

        // debug variables
        public int TilesDrawnCount = 0;

        // camera
        public Matrix CameraTransform;
        public float currentYCameraOffset = 0;
        public static float MAX_UP_CAMERA_OFFSET = 380;
        public static float MAX_DOWN_CAMERA_OFFSET = -450;


        public Map Map;
        public Player Player;
        public Stopwatch stopWatch;


        // content
        public Animation_s RocketAnimation;

        public Dictionary<string, SoundEffect> SoundEffects = new();

        public List<Sprite> ControlSprites = new();
        public List<Sprite> ItemSprites = new();
        public List<Sprite> ObjectSprites = new();
        public List<Turret> Turrets = new();
        private Texture2D backgroundTexture;
        private Texture2D tutorialTexture;
        private Texture2D tutorialBazookaTexture;

        // camera
        public Camera camera;

        // inputs
        public KeyboardState KeyboardState;
        public MouseState MouseState;
        public GamePadState GamePadState;

        // loaded content
        public SpriteFont Font
        {
            get { return game.Font; }
        }
        public Texture2D AmmoTexture;
        public Texture2D ProgressBar;


        // save game
        public const string saveFile = "RocketJumper/SaveGame.xml";

        // Replay system
        private List<ReplayData> ReplayDataList = new();
        private int ReplayFrameIndex = 0;
        private bool IsReplaying = false;

        public static string SaveFilePath
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), saveFile); }
        }



        private long previousSaveElapsedMilliseconds = 0;
        public long TotalElapsedMilliseconds
        {
            get { return stopWatch.ElapsedMilliseconds + previousSaveElapsedMilliseconds; }
        }

        public GameState(MyGame game, ContentManager content)
            : base(game, content)
        {
        }

        public override void LoadContent()
        {
            Map = new Map(MapFilePath, content, this);

            stopWatch = new Stopwatch();

            // PLAYER
            Dictionary<string, Animation_s> playerAnimationDict = new()
            {
                ["idle"] = new Animation_s(content.Load<Texture2D>("Sprites/Player/Idle"), 5, 0.2f),
                ["run"] = new Animation_s(content.Load<Texture2D>("Sprites/Player/Run"), 4, 0.2f)
            };
            AnimatedSprite playerSprite = new AnimatedSprite(playerAnimationDict, Map.start, this, "idle", PlayerScale, true, true, customBoundingBoxScale: new Vector2(0.5f, 1));
            camera = new Camera();

            // Load Player
            AmmoTexture = content.Load<Texture2D>("UI/Ammo");
            ProgressBar = content.Load<Texture2D>("UI/ReloadBar");
            Player = new Player(playerSprite, this);


            // ROCKETS
            RocketAnimation = new Animation_s(content.Load<Texture2D>("Sprites/Rocket"), 5, 0.2f);
            SoundEffects["woosh"] = content.Load<SoundEffect>("Audio/woosh");
            SoundEffects["explosion"] = content.Load<SoundEffect>("Audio/explosion");

            // load all items and mapObjects
            foreach (Layer layer in Map.Layers)
                if (layer.Type == "objectgroup")
                    if (layer.Class == "items")
                        ItemSprites = layer.Sprites.Values.ToList();
                    else if (layer.Class == "map-objects")
                        ObjectSprites = layer.Sprites.Values.ToList();
                    else if (layer.Class == "map-controls")
                        ControlSprites = layer.Sprites.Values.ToList();

            // initialize turrets
            foreach (Sprite sprite in ObjectSprites)
                if (sprite.Name == "Turret")
                    Turrets.Add(new Turret(sprite, this));

            // screen sizes
            MyGame.VirtualWidth = Map.WidthInPixels;
            MyGame.VirtualHeight = Map.HeightInPixels;

            backgroundTexture = content.Load<Texture2D>("Background/GameBackground");

            tutorialTexture = content.Load<Texture2D>("UI/Tutorial-0");
            tutorialBazookaTexture = content.Load<Texture2D>("UI/Tutorial-1");

            // load save game
            LoadGame();

            stopWatch.Start();
        }

        public override void Update(GameTime gameTime)
        {
            // if (!game.IsActive)
            // {
            //     PauseGame();
            //     return;
            // }


            if (IsReplaying)
            {
                UpdateReplay(gameTime);
                return;
            }

            UpdateInputStates();
            // Handle state inputs
            if (KeyboardState.IsKeyUp(Keys.Escape))
                this.InitialEscapeReleased = true;
            if (this.InitialEscapeReleased && KeyboardState.IsKeyDown(Keys.Escape))
                PauseGame();

            Player.Update(gameTime);
            UpdateSprites(gameTime);

            // update replay data
            SaveReplayFrame();

            // camera movement
            camera.Follow(this.Player.PlayerSprite);

            if (KeyboardState.IsKeyDown(Keys.W))
            {
                if (currentYCameraOffset <= MAX_UP_CAMERA_OFFSET)
                    currentYCameraOffset += 80;
            }
            else if (KeyboardState.IsKeyDown(Keys.S))
            {
                if (currentYCameraOffset >= MAX_DOWN_CAMERA_OFFSET)
                    currentYCameraOffset -= 80;
            }
            else
            {
                if (currentYCameraOffset > 0)
                    currentYCameraOffset -= 80;
                else if (currentYCameraOffset < 0)
                    currentYCameraOffset += 80;
            }

            camera.AddVerticalOffset(currentYCameraOffset);
            CameraTransform = camera.Transform;
        }

        public void UpdateReplay(GameTime gameTime)
        {
            if (ReplayFrameIndex >= ReplayDataList.Count)
            {
                IsReplaying = false;
                game.ChangeState(new MenuState(game, content));
                return;
            }

            ReplayData replayData = ReplayDataList[ReplayFrameIndex];
            Player.UpdateReplay(gameTime, replayData);
            UpdateSprites(gameTime);

            // camera movement
            camera.Follow(this.Player.PlayerSprite);



            camera.AddVerticalOffset(currentYCameraOffset);
            CameraTransform = camera.Transform;

            ReplayFrameIndex++;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            // Draw tutorial
            spriteBatch.Begin(transformMatrix: camera.Transform);

            // get camera view rectangle
            Rectangle cameraRectangle = camera.GetCameraRectangle();

            // draw background
            float xResize = (float)Map.WidthInPixels / backgroundTexture.Width;

            int y = 0;
            int backgroundHeight = (int)Math.Ceiling((decimal)(backgroundTexture.Height * xResize));
            int backgroundWidth = (int)Math.Ceiling((decimal)(backgroundTexture.Width * xResize));
            // increment y by the height of the background texture until it is in the camera view
            while (y < cameraRectangle.Top)
                y += backgroundHeight;

            // draw one background texture above the camera view also
            y -= backgroundHeight;

            // draw background until it is out of the camera view
            while (y < cameraRectangle.Bottom)
            {
                spriteBatch.Draw(backgroundTexture, new Rectangle(0, y, backgroundWidth, backgroundHeight), Color.White);
                y += backgroundHeight;
            }

            // draw tutorial
            spriteBatch.Draw(tutorialTexture, Map.start, null, Color.Black, 0, Vector2.Zero, 0.2f, SpriteEffects.None, 0);

            // draw bazooka tutorial
            if (Player.HasBazooka)
                spriteBatch.Draw(tutorialBazookaTexture, Map.start + new Vector2(300, 0), null, Color.Black, 0, Vector2.Zero, 0.2f, SpriteEffects.None, 0);

            spriteBatch.End();

            // game
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera.Transform);

            // Draw player
            Player.Draw(gameTime, spriteBatch);

            // draw Layers
            TilesDrawnCount = 0;
            foreach (Layer layer in Map.Layers)
                if (layer.Type == "tilelayer")
                    DrawTileLayer(gameTime, spriteBatch, layer, cameraRectangle);

            DrawSprites(gameTime, spriteBatch);

            spriteBatch.End();

            // GUI
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            Player.GUIRenderer.Draw(gameTime, spriteBatch);
            spriteBatch.End();
        }

        public Vector2 GetScreenPosition(Vector2 position)
        {
            return Vector2.Transform(position, camera.Transform);
        }

        public void Finished()
        {
            stopWatch.Stop();

            string replayId = game.CurrentReplayId;
            
            // save the time
            game.SaveTime(MapFilePath, TotalElapsedMilliseconds, replayId);

            // save replay to file
            game.SaveReplay(replayId, ReplayDataList);

            // go to menu
            game.ChangeState(new MenuState(game, content));
        }

        private void DrawTileLayer(GameTime gameTime, SpriteBatch spriteBatch, Layer layer, Rectangle cameraRectangle)
        {
            for (int y = 0; y < layer.Height; y++)
            {
                for (int x = 0; x < layer.Width; x++)
                {
                    int tileGID = layer.Data[x + y * layer.Width];

                    if (tileGID == 0)
                        continue;

                    // find the tileset for this Tile and draw it
                    foreach (TileSet tileSet in Map.TileSets)
                    {
                        Vector2 tilePosition = new Vector2(x * Map.TileWidth, y * Map.TileHeight);
                        if (tileGID >= tileSet.FirstGID && cameraRectangle.Top <= tilePosition.Y + Map.TileHeight && cameraRectangle.Bottom >= tilePosition.Y)
                        {
                            tileSet.DrawTile(tileGID, tilePosition, spriteBatch);
                            TilesDrawnCount++;
                        }
                    }
                }
            }
        }

        private void UpdateSprites(GameTime gameTime)
        {
            // update turret objects before updating its sprites
            foreach (Turret turret in Turrets)
                turret.Update(gameTime);

            // update sprites
            foreach (Sprite sprite in ObjectSprites)
                sprite.Update(gameTime);

            foreach (Sprite item in ItemSprites)
                item.Update(gameTime);

            foreach (Sprite control in ControlSprites)
                control.Update(gameTime);
        }

        private void DrawSprites(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Sprite mapObject in ObjectSprites)
                mapObject.Draw(gameTime, spriteBatch);

            foreach (Sprite item in ItemSprites)
                item.Draw(gameTime, spriteBatch);

            foreach (Sprite control in ControlSprites)
                control.Draw(gameTime, spriteBatch);
        }

        public void PauseGame()
        {
            IsPaused = true;
            this.stopWatch.Stop();
            PauseState pauseState = new PauseState(game, content, this);

            // set background to last frame
            // pauseState.Background = game.GetFrame();

            game.ChangeState(pauseState);
        }

        private void UpdateInputStates()
        {
            KeyboardState = Keyboard.GetState();
            MouseState = Mouse.GetState();
            GamePadState = GamePad.GetState(PlayerIndex.One);
        }

        public void SaveGame()
        {
            // create the save file if it doesn't exist
            FileStream fileStream;
            if (!File.Exists(SaveFilePath))
                fileStream = File.Create(SaveFilePath);
            else
                fileStream = File.Open(SaveFilePath, FileMode.Truncate);

            // save the game
            using (XmlWriter writer = XmlWriter.Create(fileStream))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("SaveGame");

                writer.WriteStartElement("Position");
                writer.WriteAttributeString("X", Player.PlayerSprite.Physics.Position.X.ToString());
                writer.WriteAttributeString("Y", Player.PlayerSprite.Physics.Position.Y.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("HasBazooka");
                writer.WriteAttributeString("Value", Player.HasBazooka.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("AmmoCount");
                writer.WriteAttributeString("Value", Player.AmmoCount.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("FireTimer");
                writer.WriteAttributeString("Value", Player.FireTimer.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("ElapsedMilliseconds");
                writer.WriteAttributeString("Value", TotalElapsedMilliseconds.ToString());
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();

                writer.Close();
                fileStream.Close();
            }
        }
        
        public void LoadGame()
        {
            if (!File.Exists(SaveFilePath))
                return;

            using (XmlReader reader = XmlReader.Create(SaveFilePath))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "Position":
                                float x = float.Parse(reader.GetAttribute("X"));
                                float y = float.Parse(reader.GetAttribute("Y"));
                                Player.PlayerSprite.Physics.Position = new Vector2(x, y);
                                break;
                            case "HasBazooka":
                                Player.HasBazooka = bool.Parse(reader.GetAttribute("Value"));
                                Player.AddBazookaToPlayer();
                                break;
                            case "AmmoCount":
                                Player.AmmoCount = int.Parse(reader.GetAttribute("Value"));
                                break;
                            case "FireTimer":
                                Player.FireTimer = int.Parse(reader.GetAttribute("Value"));
                                break;
                            case "ElapsedMilliseconds":
                                previousSaveElapsedMilliseconds = long.Parse(reader.GetAttribute("Value"));
                                break;
                        }
                    }
                }
            }
        }

        private void SaveReplayFrame()
        {
            ReplayData replayData = new ReplayData();
            replayData.PlayerPosition = Player.PlayerSprite.Physics.Position;

            ReplayDataList.Add(replayData);
        }

        public void StartReplay(string replayPath)
        {
            // load the replay
            ReplayDataList = game.LoadReplay(replayPath);

            // start the replay
            IsReplaying = true;
        }
    }
}