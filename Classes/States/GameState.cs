
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RocketJumper.Classes.MapData;

namespace RocketJumper.Classes.States
{
    public class GameState : State
    {
        // const
        public const string MapFilePath = "Content/Levels/main_map-tiled.json";
        public const float PlayerScale = 2.5f;

        // camera
        public Matrix CameraTransform;

        public Map Map;
        public Player Player;
        public GameUI GUIRenderer;

        // content
        public Animation_s RocketAnimation;

        public Dictionary<string, SoundEffect> SoundEffects = new();

        public List<Sprite> ControlSprites = new();
        public List<Sprite> ItemSprites = new();
        public List<Sprite> ObjectSprites = new();
        public List<Turret> Turrets = new();

        // camera
        private Camera camera;

        // inputs
        public KeyboardState KeyboardState;
        public MouseState MouseState;
        public GamePadState GamePadState;

        public GameState(MyGame game, ContentManager content)
            : base(game, content)
        {
        }

        public override void LoadContent()
        {
            Map = new Map(MapFilePath, content, this);


            // PLAYER
            Dictionary<string, Animation_s> playerAnimationDict = new()
            {
                ["idle"] = new Animation_s(content.Load<Texture2D>("Sprites/Player/Idle"), 5, 0.2f),
                ["run"] = new Animation_s(content.Load<Texture2D>("Sprites/Player/Run"), 4, 0.2f)
            };
            AnimatedSprite playerSprite = new AnimatedSprite(playerAnimationDict, Map.start, this, "idle", PlayerScale, true, true);
            camera = new Camera();

            // Load Player
            Player = new Player(playerSprite, this);
            GUIRenderer = new GameUI(Player)
            {
                TimerFont = game.Font,
                AmmoTexture = content.Load<Texture2D>("UI/Ammo")
            };

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
        }

        public override void Update(GameTime gameTime)
        {
            GetInputs();

            // handle state inputs
            if (KeyboardState.IsKeyDown(Keys.Escape))
                PauseGame();

            Player.Update(gameTime);
            UpdateSprites(gameTime);
            camera.Follow(this.Player.PlayerSprite);
            CameraTransform = camera.Transform;
            GUIRenderer.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // game
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera.Transform);

            // Draw player
            Player.Draw(gameTime, spriteBatch);

            // draw Layers
            foreach (Layer layer in Map.Layers)
                if (layer.Type == "tilelayer")
                    DrawTileLayer(gameTime, spriteBatch, layer);

            DrawSprites(gameTime, spriteBatch);

            spriteBatch.End();

            // GUI
            spriteBatch.Begin();
            GUIRenderer.Draw(gameTime, spriteBatch);
            spriteBatch.End();
        }

        public void AddGUIRenderer(GameUI guiRenderer)
        {
            GUIRenderer = guiRenderer;
        }

        public Vector2 GetScreenPosition(Vector2 position)
        {
            return Vector2.Transform(position, camera.Transform);
        }

        public void Finished()
        {
            // save the score
            

            game.ChangeState(new MenuState(game, content));
        }

        private void DrawTileLayer(GameTime gameTime, SpriteBatch spriteBatch, Layer layer)
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
                        if (tileGID >= tileSet.FirstGID)
                            tileSet.DrawTile(tileGID, new Vector2(x * Map.TileWidth, y * Map.TileHeight), spriteBatch);
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

        private void PauseGame()
        {
            IsPaused = true;
            PauseState pauseState = new PauseState(game, content, this);

            // set background to last frame
            // pauseState.Background = game.GetFrame();

            game.ChangeState(pauseState);
        }

        private void GetInputs()
        {
            KeyboardState = Keyboard.GetState();
            MouseState = Mouse.GetState();
            GamePadState = GamePad.GetState(PlayerIndex.One);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}