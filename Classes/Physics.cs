using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace RocketJumper.Classes
{
    public class Physics
    {
        private Gameplay level;

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

        public Rectangle AABB
        {
            get { return Tools.RectangleMoveTo(aabb, Position); }
            private set { aabb = value; }
        }
        private Rectangle aabb;

        public RotatedRectangle RBB
        {
            get { return rbb.MoveTo(Position); }
            private set { rbb = value; }
        }
        private RotatedRectangle rbb;

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

        public bool Collided, IsOnGround;

        public Physics(Vector2 Position, Vector2 Size, Gameplay level, bool gravityEnabled, float rotation, string boundingBoxType = "AABB")
        {
            this.Position = Position;
            this.Size = Size;
            this.level = level;
            Rotation = rotation;
            GravityEnabled = gravityEnabled;
            AddBoundingBox(boundingBoxType);
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
            deltaMove = Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // apply inputVelocity to deltaMove
            deltaMove += inputVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // COLLISION DETECTION
            // get surrounding tiles of BoundingBox
            int leftTile = (int)Math.Floor((float)AABB.Left / level.Map.TileWidth) - 1;
            int rightTile = (int)Math.Ceiling((float)AABB.Right / level.Map.TileWidth);
            int topTile = (int)Math.Floor((float)AABB.Top / level.Map.TileHeight) - 1;
            int bottomTile = (int)Math.Ceiling((float)AABB.Bottom / level.Map.TileHeight);

            // reset collision flags
            Collided = false;
            IsOnGround = false;

            // for each potentially colliding tile
            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    // if this tile is collidable
                    if (level.Map.GetTileId(x, y) != 0)
                    {
                        // determine collision depth (with direction) and magnitude
                        Rectangle tileBounds = level.Map.GetBounds(x, y);

                        if (deltaMove.Y > 0 && IsTouchingTop(tileBounds))
                        {
                            IsOnGround = true;
                            Collided = true;
                            deltaMove.Y = 0;
                            Velocity.Y = 0;

                            // check if player clipped into ground
                            if (AABB.Bottom > tileBounds.Top)
                            {
                                Position.Y = tileBounds.Top - AABB.Height;
                            }
                        }
                        if (deltaMove.Y < 0 && IsTouchingBottom(tileBounds))
                        {
                            Collided = true;
                            deltaMove.Y = 0;
                            Velocity.Y = 0;

                            // check if player clipped into ceiling
                            if (AABB.Top < tileBounds.Bottom)
                            {
                                Position.Y = tileBounds.Bottom;
                            }
                        }
                        if (deltaMove.X > 0 && IsTouchingLeft(tileBounds) ||
                            deltaMove.X < 0 && IsTouchingRight(tileBounds))
                        {
                            Collided = true;
                            deltaMove.X = 0;
                            Velocity.X = 0;
                        }
                    }
                }
            }

            // apply deltaMove to position
            Position += deltaMove;

            // clear temp forces
            tempForcesY.Clear();
            tempForcesX.Clear();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (IsBoundingBoxVisible)
                Tools.DrawRectangle(AABB, Color.Red, spriteBatch);
        }


        public void EnableGravity()
        {
            forcesY.Add(gravityAccel);
        }

        public void DisableGravity()
        {
            forcesY.Remove(gravityAccel);
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

        public bool IsTouchingLeft(Rectangle other)
        {
            return AABB.Right + deltaMove.X > other.Left &&
                   AABB.Left < other.Left &&
                   AABB.Bottom > other.Top &&
                   AABB.Top < other.Bottom;
        }

        public bool IsTouchingRight(Rectangle other)
        {
            return AABB.Left + deltaMove.X < other.Right &&
                   AABB.Right > other.Right &&
                   AABB.Bottom > other.Top &&
                   AABB.Top < other.Bottom;
        }

        public bool IsTouchingTop(Rectangle other)
        {
            return AABB.Bottom + deltaMove.Y > other.Top &&
                   AABB.Top < other.Top &&
                   AABB.Right > other.Left &&
                   AABB.Left < other.Right;
        }

        public bool IsTouchingBottom(Rectangle other)
        {
            return AABB.Top + deltaMove.Y < other.Bottom &&
                   AABB.Bottom > other.Bottom &&
                   AABB.Right > other.Left &&
                   AABB.Left < other.Right;
        }

        public void AddBoundingBox(string type)
        {
            if (type == "AABB")
                AABB = new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
            else if (type == "RBB")
                RBB = new RotatedRectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y, (int)Rotation);
        }

        public void MoveTo(Vector2 position)
        {
            Position = position;
        }

        public void MoveBy(Vector2 move)
        {
            Position += move;
        }

        public Vector2 GetGlobalCenter()
        {
            return Position + GetLocalCenter();
        }

        public Vector2 GetLocalCenter()
        {
            return new Vector2(Size.X / 2, Size.Y / 2);
        }
    }
}
