using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RocketJumper.Classes.MapData;
using RocketJumper.Classes.States;
using System;
using System.Collections.Generic;


namespace RocketJumper.Classes
{
    public class Player
    {
        private GameState gameState;
        public AnimatedSprite PlayerSprite;
        public GameUI GUIRenderer;


        // movement vars
        private float inputMovement;
        public static float MaxHorizontalSpeed = 300;
        public Rectangle BoundingBoxWidth;

        // components
        public GameState GameState
        {
            get { return PlayerSprite.GameState; }
        }

        public List<Sprite> Items = new();           // list of mapobject that draw onto the player

        // bazooka
        public Sprite Bazooka;
        public bool HasBazooka = false;
        public int FireRate = 500;           // time between shots in milliseconds
        public int FireTimer;
        public int ReloadRate = 2000;         // time between reloads in milliseconds
        public int ReloadTimer;
        public int MaxAmmo = 2;
        public int AmmoCount;
        public List<Rocket> RocketList = new();
        public float MaxExplosionForce = 280.0f;
        public Vector2 ShootingPosition
        {
            get { return PlayerSprite.Physics.Position + Bazooka.AttachmentOrigin + new Vector2(0, 15); }
        }

        // other
        public const float PlayerSizeScale = 2.5f;

        public Player(AnimatedSprite playerSprite, GameState gameState)
        {
            this.gameState = gameState;
            PlayerSprite = playerSprite;
            playerSprite.Physics.IsBoundingBoxVisible = false;

            GUIRenderer = new GameUI(this, this.gameState)
            {
                TimerFont = this.gameState.Font,
                AmmoTexture = this.gameState.AmmoTexture,
                ProgressBar = this.gameState.ProgressBar
            };

            FireTimer = FireRate;
            ReloadTimer = ReloadRate;
        }

        public void Update(GameTime gameTime)
        {
            if (HasBazooka)
            {
                FireTimer -= gameTime.ElapsedGameTime.Milliseconds;
                if (AmmoCount <= MaxAmmo)
                    ReloadTimer -= gameTime.ElapsedGameTime.Milliseconds;

                // handle reload
                if (AmmoCount < MaxAmmo && ReloadTimer <= 0)
                    ReloadBazooka();

                if (AmmoCount <= 0)
                    GUIRenderer.ReloadBarVisible = true;
                else
                    GUIRenderer.ReloadBarVisible = false;
            }

            HandleInputs();
            CheckSpriteCollision();

            // rockets
            for (int i = 0; i < RocketList.Count; i++)
            {
                RocketList[i].Update(gameTime);
                if (RocketList[i].Collided)
                {
                    if (!RocketList[i].SideOfMapCollision)
                    {
                        // calcualte vector from rocket to player
                        Vector2 direction = PlayerSprite.Physics.GetGlobalCenter() - RocketList[i].RocketSprite.Physics.GetGlobalCenter();
                        // get length of direction and calculate force based on it where the closer the player is the more force is applied
                        float distance = direction.Length();
                        float force;
                        if (distance > 100.0f)
                        {
                            force = 0.0f;
                        }
                        else if (distance < 23.0f)
                        {
                            force = MaxExplosionForce;
                        }
                        else
                        {
                            force = MaxExplosionForce * (1.0f - ((distance - 23.0f) / 100.0f));
                        }
                        // normalize direction
                        direction.Normalize();
                        // add force to player
                        PlayerSprite.Physics.AddTempForce(direction * force * new Vector2(1.0f, 1.2f));
                    }
                    RocketList.RemoveAt(i--);
                }
            }

            // add horizontal movement
            PlayerSprite.AddInputToPhysics(new Vector2(inputMovement, 0.0f), MaxHorizontalSpeed);
            PlayerSprite.Update(gameTime);
            GUIRenderer.Update(gameTime);
        }

        public void UpdateReplay(GameTime gameTime, ReplayData data)
        {
            PlayerSprite.Physics.Position = data.PlayerPosition;

            PlayerSprite.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // flip if necessary
            if (inputMovement < 0)
                PlayerSprite.Effects = SpriteEffects.FlipHorizontally;
            else if (inputMovement > 0)
                PlayerSprite.Effects = SpriteEffects.None;

            // choose and draw right player animation
            if (inputMovement == 0)
            {
                if (PlayerSprite.CurrentAnimationId != "idle")
                    PlayerSprite.ChangeAnimation("idle");
            }
            else
            {
                if (PlayerSprite.CurrentAnimationId != "run")
                    PlayerSprite.ChangeAnimation("run");
            }

            // rockets
            foreach (Rocket rocket in RocketList)
                rocket.Draw(gameTime, spriteBatch);

            PlayerSprite.Draw(gameTime, spriteBatch);

        }

        public void ReloadBazooka()
        {
            AmmoCount = MaxAmmo;
            ReloadTimer = ReloadRate;
        }

        private void HandleInputs()
        {
            // basic moving
            if (GameState.KeyboardState.IsKeyDown(Keys.A) || GameState.KeyboardState.IsKeyDown(Keys.Left))
                inputMovement = -1.0f;
            else if (GameState.KeyboardState.IsKeyDown(Keys.D) || GameState.KeyboardState.IsKeyDown(Keys.Right))
                inputMovement = 1.0f;
            else
                inputMovement = 0.0f;

            // bazooka
            if (HasBazooka)
            {
                Vector2 mousePosition = GameState.MouseState.Position.ToVector2();
                Vector2 screenShootingPosition = gameState.GetScreenPosition(ShootingPosition);
                Vector2 direction = mousePosition - screenShootingPosition;

                direction.Normalize();

                float angle = MathF.Atan2(direction.Y, direction.X);
                Bazooka.Physics.Rotation = angle;

                // if bazooka is facing left, flip it
                if (angle > MathF.PI / 2 || angle < -MathF.PI / 2)
                    Bazooka.Effects = SpriteEffects.FlipVertically;
                else
                    Bazooka.Effects = SpriteEffects.None;

                // shooting
                if (AmmoCount > 0 && FireTimer <= 0 && GameState.MouseState.LeftButton == ButtonState.Pressed)
                {
                    Rocket rocket = new Rocket(ShootingPosition, direction, GameState);
                    ReloadTimer = ReloadRate;
                    RocketList.Add(rocket);
                    FireTimer = FireRate;
                    AmmoCount--;

                    // play sound
                    GameState.SoundEffects["woosh"].CreateInstance().Play();
                }
            }
        }

        private void CheckSpriteCollision()
        {
            foreach (Sprite item in GameState.ItemSprites)
            {
                if (item.Physics.AABB.Intersects(PlayerSprite.Physics.AABB))
                {
                    AddItemToPlayer(item);
                    break;
                }
            }

            foreach (Sprite mapControl in GameState.ControlSprites)
            {
                if (mapControl.Physics.AABB.Intersects(PlayerSprite.Physics.AABB))
                {
                    if (mapControl.Name == "Finish")
                    {
                        GameState.Finished();
                    }
                }
            }
        }

        private void AddItemToPlayer(Sprite item)
        {
            if (item.Name == "Bazooka")
            {
                AmmoCount = 0;
                Bazooka = item;
                HasBazooka = true;
                Bazooka.AttachmentOrigin = new Vector2(PlayerSprite.Physics.Width / 2, Bazooka.AttachmentOffset.Y);
            }

            GameState.ItemSprites.Remove(item);
            Items.Add(item);
            PlayerSprite.AddChild(item);
        }

        public void AddBazookaToPlayer()
        {
            foreach (Sprite item in GameState.ItemSprites)
            {
                if (item.Name == "Bazooka")
                {
                    AddItemToPlayer(item);
                    break;
                }
            }
        }
    }
}