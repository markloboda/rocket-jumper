using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using RocketJumper.Classes.States;
using System.Diagnostics;
using RocketJumper.Classes.MapData;

namespace RocketJumper.Classes
{
    public class Physics
    {
        private GameState gameState;

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

        public string BoundingBoxType
        {
            get;
            private set;
        }
        public Rectangle AABB
        {
            get { return Tools.RectangleMoveTo(aabb, Position - Origin); }
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

        private Vector2 horizontalInputSpeed;
        private bool isOnIce = false;
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

        public bool Collided, SideOfMapCollision, IsOnGround;

        public int NormalFriction = 100;
        public int IceFriction = 1;

        public Physics(Vector2 Position, Vector2 Size, GameState gameState, bool gravityEnabled, float rotation, string boundingBoxType = "AABB")
        {
            this.Position = Position;
            this.Size = Size;
            this.gameState = gameState;
            Rotation = rotation;
            GravityEnabled = gravityEnabled;
            AddBoundingBox(boundingBoxType);
        }

        public void Update(GameTime gameTime)
        {
            deltaMove = Vector2.Zero;

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
            Velocity.Y += fYRes * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Velocity.X += fXRes * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // apply horizontalInputSpeed to deltaMove
            if (IsOnGround)
            {
                if (isOnIce)
                {
                    Velocity.X += horizontalInputSpeed.X * 0.05f;
                }
                else if (fXRes == 0)
                {
                    Velocity.X = horizontalInputSpeed.X;
                } else {
                    deltaMove.X += horizontalInputSpeed.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }
            else
                Velocity.X += horizontalInputSpeed.X * 0.02f;

            // apply Velocity to deltaMove
            deltaMove += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // COLLISION DETECTION
            // get surrounding tiles of BoundingBox
            int leftTile = (int)Math.Floor((float)AABB.Left / gameState.Map.TileWidth) - 1;
            int rightTile = (int)Math.Ceiling((float)AABB.Right / gameState.Map.TileWidth);
            int topTile = (int)Math.Floor((float)AABB.Top / gameState.Map.TileHeight) - 1;
            int bottomTile = (int)Math.Ceiling((float)AABB.Bottom / gameState.Map.TileHeight);

            // reset collision flags
            Collided = false;
            SideOfMapCollision = false;
            IsOnGround = false;

            if (BoundingBoxType == "AABB")
                // for each potentially colliding tile
                for (int y = topTile; y <= bottomTile; ++y)
                {
                    for (int x = leftTile; x <= rightTile; ++x)
                    {
                        // if this tile is collidable
                        int tileGID = gameState.Map.GetTileId(x, y);
                        if (tileGID != 0)
                        {
                            // determine collision depth (with direction) and magnitude
                            Rectangle tileBounds = gameState.Map.GetBounds(x, y);

                            if (deltaMove.Y >= 0 && AABBIsTouchingTop(tileBounds))
                            {
                                // check tileset
                                TileSet tileSet = gameState.Map.GetTileSet(tileGID);
                                isOnIce = tileSet.IsIce;

                                IsOnGround = true;
                                Collided = true;
                                deltaMove.Y = 0;
                                Velocity.Y = 0;

                                Position.Y = tileBounds.Top - AABB.Height;

                                // add friction
                                int friction;
                                if (isOnIce)
                                    friction = IceFriction;
                                else
                                    friction = NormalFriction;

                                if (Math.Abs(Velocity.X) > friction)
                                {
                                    if (Velocity.X > 0)
                                        Velocity.X -= friction;
                                    else
                                        Velocity.X += friction;
                                }
                                else
                                    Velocity.X = 0;
                            }
                            if (deltaMove.Y < 0 && AABBIsTouchingBottom(tileBounds))
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
                            // left right collision with tiles
                            if (deltaMove.X > 0 && AABBIsTouchingLeft(tileBounds) ||
                                deltaMove.X < 0 && AABBIsTouchingRight(tileBounds))
                            {
                                // calculate distance to tileBounds
                                float distance = 0;
                                if (deltaMove.X > 0)
                                    distance = tileBounds.Left - AABB.Right;
                                else
                                    distance = tileBounds.Right - AABB.Left;

                                Collided = true;
                                deltaMove.X = distance;
                                Velocity.X = 0;
                            }
                        }
                    }
                }

            // side of map collision
            if (deltaMove.X > 0 && AABB.Right + deltaMove.X > gameState.Map.WidthInPixels ||
                deltaMove.X < 0 && AABB.Left + deltaMove.X < 0)
            {
                // calculate distance to edge of map
                float distance = 0;
                if (deltaMove.X > 0)
                    distance = gameState.Map.WidthInPixels - AABB.Right;
                else
                    distance = 0 - AABB.Left;


                Collided = true;
                SideOfMapCollision = true;
                deltaMove.X = distance;
                Velocity.X = 0;
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

        public void AddInputMovement(Vector2 horizontalMovement, float maxSpeed)
        {
            horizontalInputSpeed = horizontalMovement * maxSpeed;
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

        public bool AABBIsTouchingLeft(Rectangle other)
        {
            return AABB.Right + deltaMove.X >= other.Left &&
                   AABB.Left < other.Left &&
                   AABB.Bottom > other.Top &&
                   AABB.Top < other.Bottom;
        }

        public bool AABBIsTouchingRight(Rectangle other)
        {
            return AABB.Left + deltaMove.X <= other.Right &&
                   AABB.Right > other.Right &&
                   AABB.Bottom > other.Top &&
                   AABB.Top < other.Bottom;
        }

        public bool AABBIsTouchingTop(Rectangle other)
        {
            return AABB.Bottom + deltaMove.Y >= other.Top &&
                   AABB.Top < other.Top &&
                   AABB.Right > other.Left &&
                   AABB.Left < other.Right;
        }

        public bool AABBIsTouchingBottom(Rectangle other)
        {
            return AABB.Top + deltaMove.Y <= other.Bottom &&
                   AABB.Bottom > other.Bottom &&
                   AABB.Right > other.Left &&
                   AABB.Left < other.Right;
        }

        public void AddBoundingBox(string type)
        {
            BoundingBoxType = type;
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
            return new Vector2(Size.X / 2, Size.Y / 2) - Origin;
        }
    }
}
