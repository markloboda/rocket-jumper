using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketJumper.Classes
{



    class Physics
    {
        private Tile[,] tiles;

        public Physics(Vector2 position)
        {
            this.position = position;
        }

        public Physics(Vector2 position, Tile[,] tiles)
        {
            this.position = position;
            this.tiles = tiles;
        }

        public Rectangle BoundingBox
        {
            get { return boundingBox; }
            set
            {
                boundingBox = value;
                isBoundingDef = true;
            }
        }
        Rectangle boundingBox;
        bool isBoundingDef = false;
        public bool IsBoundingBoxVisible
        {
            get { return isBoundingBoxVisible; }
            set { isBoundingBoxVisible = value; }
        }
        bool isBoundingBoxVisible = false;

        public Texture2D BoundingBoxTexture
        {
            get { return boundingBoxTexture; }
            set { boundingBoxTexture = value; }
        }
        Texture2D boundingBoxTexture;

        public Vector2 Velocity
        {
            get { return velocity; }
        }
        Vector2 velocity;

        public Vector2 Position
        {
            get { return position; }
        }
        Vector2 position;

        public bool IsOnGround
        {
            get { return isOnGround; }
        }
        bool isOnGround;

        public void AddMovement(GameTime gameTime, Vector2 speed)
        {
            // apply input movement
            Vector2 deltaMove = speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            position = position + deltaMove;
        }

        public void DrawBoundingBox(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (isBoundingDef)
            {
                if (boundingBoxTexture == null)
                {
                    // setup Texture2D for bounding box
                    boundingBoxTexture = new Texture2D(spriteBatch.GraphicsDevice, boundingBox.Width, boundingBox.Height);
                    Color[] data = new Color[boundingBox.Width * boundingBox.Height];
                    for (int i = 0; i < boundingBox.Width; ++i)
                    {
                        data[i] = Color.Red;
                        data[(boundingBox.Height - 1) * boundingBox.Width + i] = Color.Red;
                    }
                    for (int i = 0; i < boundingBox.Height; ++i)
                    {
                        data[i * boundingBox.Width] = Color.Red;
                        data[i * boundingBox.Width + boundingBox.Width - 1] = Color.Red;
                    }
                    boundingBoxTexture.SetData(data);
                }
                spriteBatch.Draw(boundingBoxTexture, Position, Color.White);
            }
        }

        public void Update(GameTime gameTime)
        {
            // apply velocity to deltaMove
            Vector2 deltaMove = velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (isBoundingDef)
            {
                // move bounding box to new position
                boundingBox.Offset(deltaMove);

                // check if player is on ground
                if (this.tiles != null)
                {
                    foreach (Tile tile in this.tiles)
                    {
                        if (tile.Collision == TileCollision.Passable)
                        {
                            continue;
                        }
                        if (tile.BoundingBox.Intersects(boundingBox))
                        {
                            isOnGround = true;
                            break;
                        }
                        else
                        {
                            isOnGround = false;
                        }
                    }
                }
            }

            // apply gravity to speed
            if (!isOnGround)
            {
                velocity.Y = velocity.Y + 9.81f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                velocity.Y = 0.0f;
            }

            position = position + deltaMove;
        }
    }
}
