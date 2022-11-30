using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RocketJumper.Classes.MapData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketJumper.Classes
{
    class Physics
    {
        private Level level;

        // forces
        private readonly float fGravity = 2000.0f;

        // movement per frame
        private Vector2 deltaMove;


        public Physics(Vector2 position)
        {
            this.position = position;
        }

        public Physics(Vector2 position, Level level)
        {
            this.position = position;
            this.level = level;
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
        private Rectangle boundingBox;
        private bool isBoundingDef = false;
        public bool IsBoundingBoxVisible
        {
            get { return isBoundingBoxVisible; }
            set { isBoundingBoxVisible = value; }
        }
        private bool isBoundingBoxVisible = false;

        public Texture2D BoundingBoxTexture
        {
            get { return boundingBoxTexture; }
            set { boundingBoxTexture = value; }
        }
        private Texture2D boundingBoxTexture;

        public Vector2 Velocity
        {
            get { return velocity; }
        }
        private Vector2 velocity;

        private Vector2 inputVelocity;

        public Vector2 Position
        {
            get { return position; }
        }
        private Vector2 position;

        public bool IsOnGround
        {
            get { return isOnGround; }
        }
        private bool isOnGround;



        public void Update(GameTime gameTime)
        {
            // vars in function
            List<float> fVerticalList = new List<float>();
            float accelVertical;

            // if bounding is defined, handle collisions
            if (isBoundingDef)
                HandleCollision();

            if (!isOnGround)
                // apply gravity                
                fVerticalList.Add(fGravity);
            else
                if (velocity.Y >= 0)
                // if velocity is down and object isOnGround -> reset velocity.Y
                velocity.Y = 0;



            // calculate resulting vertical force
            float fVerticalRes = 0.0f;
            foreach (float force in fVerticalList)
                fVerticalRes += force;

            // if force is pointing down and object isOnGround -> no movement
            if (isOnGround && fVerticalRes > 0)
                fVerticalRes = 0;

            // apply vertical force (mass is 1)
            accelVertical = fVerticalRes;

            // apply vertical acceleration to speed of object
            velocity.Y = velocity.Y + accelVertical * (float)gameTime.ElapsedGameTime.TotalSeconds;





            // apply velocity to deltaMove
            this.deltaMove = velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // apply inputVelocity to deltaMove
            this.deltaMove += inputVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            boundingBox.Offset(deltaMove);
            position += this.deltaMove;
        }

        public void AddMovement(GameTime gameTime, Vector2 inputSpeed)
        {
            inputVelocity = inputSpeed;
        }

        private void HandleCollision()
        {
            // get surrounding tiles of boundingBox
            int leftTile = (int)Math.Floor((float)boundingBox.Left / level.Map.TileWidth);
            int rightTile = (int)Math.Ceiling(((float)boundingBox.Right / level.Map.TileWidth)) - 1;
            int topTile = (int)Math.Floor((float)boundingBox.Top / level.Map.TileHeight);
            int bottomTile = (int)Math.Ceiling(((float)boundingBox.Bottom / level.Map.TileHeight)) - 1;

            isOnGround = false;

            foreach (Layer layer in level.Map.Layers)
            {
                // skip uncollidable layers
                if (!layer.Collidable)
                    continue;

                // get tile type of surrounding tiles
                for (int y = topTile; y <= bottomTile; y++)
                {
                    for (int x = leftTile; x <= rightTile; x++)
                    {
                        if (x >= 0 && x < level.Map.Width && y >= 0 && y < level.Map.Height)
                        {
                            int tile = layer.GetTileType(x, y);
                            if (tile != 0)
                            {
                                // get tile boundingBox
                                Rectangle tileBoundingBox = new Rectangle(x * level.Map.TileWidth, y * level.Map.TileHeight, level.Map.TileWidth, level.Map.TileHeight);

                                // check if boundingBox intersects with tileBoundingBox
                                if (boundingBox.Intersects(tileBoundingBox))
                                {
                                    // get intersection depth
                                    Vector2 depth = GetRectangleIntersectionDepth(boundingBox, tileBoundingBox);

                                    // if depth is not zero, move boundingBox
                                    if (depth != Vector2.Zero)
                                    {
                                        // if depth.Y is not zero, object isOnGround
                                        if (depth.Y != 0)
                                            isOnGround = true;

                                        // move boundingBox
                                        boundingBox = RectangleOffset(boundingBox, new Vector2(0, -depth.Y));
                                        position = new Vector2(boundingBox.X, boundingBox.Y);
                                    }
                                }
                            }
                        }
                    }
                }
            }
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

        private static Vector2 GetRectangleIntersectionDepth(Rectangle rectangle1, Rectangle rectangle2)
        {
            Rectangle intersection = Rectangle.Intersect(rectangle1, rectangle2);
            if (intersection.IsEmpty)
                return Vector2.Zero;

            return new Vector2(intersection.Width, intersection.Height);
        }

        private static Rectangle RectangleOffset(Rectangle rectangle, Vector2 offset)
        {
            rectangle.X += (int)offset.X;
            rectangle.Y += (int)offset.Y;
            return rectangle;
        }
    }
}
