using Microsoft.Xna.Framework;
using RocketJumper.Classes.MapData;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace RocketJumper.Classes
{
    public class Physics
    {
        private Level level;

        // forces
        private readonly float gravityAccel = 10.0f;

        // forces that last until removed
        public List<float> forcesY = new();
        public List<float> forcesX = new();

        // temp forces that last 1 frame
        public List<float> tempForcesY = new();
        public List<float> tempForcesX = new();

        // movement per frame
        private Vector2 deltaMove;

        public Rectangle BoundingBox
        {
            get { return boundingBox; }
            set
            {
                boundingBox = value;
            }
        }
        private Rectangle boundingBox;

        public bool IsBoundingBoxVisible = false;

        public Vector2 Velocity;

        private Vector2 inputVelocity;
        public Vector2 Position;
        public float Rotation;
        public Vector2 Origin;
        public Vector2 Size;
        public int Height
        {
            get { return (int)Size.Y; }
        }
        public int Width
        {
            get { return (int)Size.X; }
        }

        public bool TopCollision, BottomCollision, LeftCollision, RightCollision;

        // flags
        public bool GravityEnabled
        {
            get
            {
                return gravityEnabled;
            }
            set
            {
                gravityEnabled = value;
                if (gravityEnabled)
                    EnableGravity();
                else
                    DisableGravity();
            }
        }
        private bool gravityEnabled;

        public Physics(Vector2 Position, Vector2 Size, Level level, bool gravityEnabled, float rotation)
        {
            this.Position = Position;
            this.Size = Size;
            this.level = level;
            Rotation = rotation;
            GravityEnabled = gravityEnabled;
            AddBoundingBox();
        }

        public void Update(GameTime gameTime)
        {
            // calculate resulting vertical force
            float fYRes = 0.0f;
            foreach (float force in forcesY)
                fYRes += force;
            foreach (float force in tempForcesY)
                fYRes += force;

            // calculate resulting horizontal force
            float fXRes = 0.0f;
            foreach (float force in forcesX)
                fXRes += force;
            foreach (float force in tempForcesX)
                fXRes += force;

            // multiply by unit for 1 meter
            fYRes *= 200;
            fXRes *= 200;

            // apply acceleration to speed of object
            Velocity.Y = Velocity.Y + fYRes * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Velocity.X = Velocity.X + fXRes * (float)gameTime.ElapsedGameTime.TotalSeconds;


            // apply Velocity to deltaMove
            this.deltaMove = Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // apply inputVelocity to deltaMove
            this.deltaMove += inputVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // move object in Y direction and check for collision on Y axis
            MoveBy(new Vector2(0, this.deltaMove.Y));
            SetHorizontalCollisionFlags();
            if (BottomCollision && Velocity.Y > 0)
            {
                Velocity.Y = 0;
                MoveBy(new Vector2(0, -this.deltaMove.Y));
            }
            else if (TopCollision && Velocity.Y < 0)
            {
                Velocity.Y = 0;
                MoveBy(new Vector2(0, -this.deltaMove.Y));
            }

            // move object in X direction and check for collision on X axis
            MoveBy(new Vector2(this.deltaMove.X, 0));
            SetVerticalCollisionFlags();


            // clear temp forces
            tempForcesY.Clear();
            tempForcesX.Clear();
        }

        public void EnableGravity()
        {
            forcesY.Add(gravityAccel);
        }

        public void DisableGravity()
        {
            forcesY.Remove(gravityAccel);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (IsBoundingBoxVisible)
                Tools.DrawRectangle(boundingBox, Color.Red, spriteBatch);
        }

        public void AddInputMovement(GameTime gameTime, Vector2 inputSpeed)
        {
            inputVelocity = inputSpeed;
        }


        public void AddForce(Vector2 force)
        {
            forcesX.Add(force.X);
            forcesY.Add(force.Y);
        }

        public void AddTempForce(Vector2 force)
        {
            tempForcesX.Add(force.X);
            tempForcesY.Add(force.Y);
        }

        public void AddBoundingBox()
        {
            BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
        }

        public void MoveTo(Vector2 position)
        {
            Position = position;
            boundingBox = Tools.RectangleMove(boundingBox, Position);
        }

        public void MoveBy(Vector2 move)
        {
            Position += move;
            boundingBox = Tools.RectangleMove(boundingBox, Position);
        }


        private void SetVerticalCollisionFlags()
        {
            // get surrounding tiles of boundingBox
            int leftTile = (int)Math.Floor((float)boundingBox.Left / level.Map.TileWidth);
            int rightTile = (int)Math.Ceiling(((float)boundingBox.Right / level.Map.TileWidth)) - 1;
            int topTile = (int)Math.Floor((float)boundingBox.Top / level.Map.TileHeight);
            int bottomTile = (int)Math.Ceiling(((float)boundingBox.Bottom / level.Map.TileHeight)) - 1;

            // reset collision flags
            TopCollision = false;
            BottomCollision = false;


            foreach (Layer layer in level.Map.Layers)
            {
                // skip uncollidable layers
                if (!layer.Collidable)
                    continue;

                // check for collision with each side
                // top
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    if (layer.GetTileType(x, topTile) != 0)
                    {
                        Rectangle tileBounds = level.Map.GetBounds(x, topTile);
                        if (boundingBox.Intersects(tileBounds))
                            TopCollision = true;
                    }
                }

                // bottom
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    if (layer.GetTileType(x, bottomTile) != 0)
                    {
                        Rectangle tileBounds = level.Map.GetBounds(x, bottomTile);
                        if (boundingBox.Intersects(tileBounds))
                            BottomCollision = true;
                    }
                }
            }
        }

        public void SetHorizontalCollisionFlags()
        {
            // get surrounding tiles of boundingBox
            int leftTile = (int)Math.Floor((float)boundingBox.Left / level.Map.TileWidth);
            int rightTile = (int)Math.Ceiling(((float)boundingBox.Right / level.Map.TileWidth)) - 1;
            int topTile = (int)Math.Floor((float)boundingBox.Top / level.Map.TileHeight);
            int bottomTile = (int)Math.Ceiling(((float)boundingBox.Bottom / level.Map.TileHeight)) - 1;

            // reset collision flags
            LeftCollision = false;
            RightCollision = false;


            foreach (Layer layer in level.Map.Layers)
            {
                // skip uncollidable layers
                if (!layer.Collidable)
                    continue;

                // check for collision with each side
                // left
                for (int y = topTile; y <= bottomTile; ++y)
                {
                    if (layer.GetTileType(leftTile, y) != 0)
                    {
                        Rectangle tileBounds = level.Map.GetBounds(leftTile, y);
                        if (boundingBox.Intersects(tileBounds))
                            LeftCollision = true;
                    }
                }

                // right
                for (int y = topTile; y <= bottomTile; ++y)
                {
                    if (layer.GetTileType(rightTile, y) != 0)
                    {
                        Rectangle tileBounds = level.Map.GetBounds(rightTile, y);
                        if (boundingBox.Intersects(tileBounds))
                            RightCollision = true;
                    }
                }
            }
        }
        public Vector2 GetGlobalCenter()
        {
            return new Vector2(Position.X + (Size.X / 2), Position.Y + (Size.Y / 2));
        }

        public Vector2 GetLocalCenter()
        {
            return new Vector2(Size.X / 2, Size.Y / 2);
        }
    }
}
