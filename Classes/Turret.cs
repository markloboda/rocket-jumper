using Microsoft.Xna.Framework;
using RocketJumper.Classes.MapData;
using System;

namespace RocketJumper.Classes
{
    public class Turret
    {
        public Sprite baseSprite;
        private Gameplay level;
        private Sprite shootingSprite;

        public Vector2 ShootingPosition;

        private float fireTimer = fireRate;
        private Vector2 shootingDirection;
        private bool hasLineOfSightNow;

        // parameters
        private const float fireRate = 1000.0f;

        public Turret(Sprite baseSprite, Gameplay level)
        {
            this.baseSprite = baseSprite;
            this.level = level;

            SetTurretTop(baseSprite.Children[0]);
        }

        public void Update(GameTime gameTime)
        {
            AimAt(level.Player.PlayerSprite.Physics.GetGlobalCenter());
            if (!hasLineOfSightNow)
                return;

            // shoot if possible
            fireTimer -= gameTime.ElapsedGameTime.Milliseconds;
            if (fireTimer <= 0)
            {
                level.Player.RocketList.Add(new Rocket(ShootingPosition, shootingDirection, level, level.Player.PlayerSprite, hitsPlayer: true));
                fireTimer = fireRate;
            }
        }

        public void AimAt(Vector2 target)
        {
            // get the rotation of the turret to aim at the target
            Vector2 direction = target - ShootingPosition;
            shootingDirection = Vector2.Normalize(direction);

            hasLineOfSightNow = HasLineOfSight(target);

            if (!hasLineOfSightNow)
                return;

            shootingSprite.Physics.Rotation = MathF.Atan2(shootingDirection.Y, shootingDirection.X);
        }

        public bool HasLineOfSight(Vector2 target)
        {
            Vector3 shootingPositionVec3 = new Vector3(ShootingPosition.X, ShootingPosition.Y, 0);
            Vector3 shootingDirectionVec3 = new Vector3(shootingDirection.X, shootingDirection.Y, 0);
            Ray ray = new Ray(shootingPositionVec3, shootingDirectionVec3);

            Vector2 distanceVec2 = ShootingPosition - target;
            float distance = distanceVec2.Length();

            // check if ray intersects with tile layer
            foreach (Layer layer in level.Map.Layers)
            {
                // skip uncollidable layers
                if (!layer.Collidable)
                    continue;

                // check on each point of the ray if it intersects with a tile
                Vector3 point;
                int i = 0;
                while (i * level.Map.TileHeight <= distance)
                {
                    point = ray.Position + ray.Direction * i * level.Map.TileHeight;
                    // get tile at point
                    int tileX = (int)(point.X / level.Map.TileWidth);
                    int tileY = (int)(point.Y / level.Map.TileHeight);

                    // check if point is inside tile
                    if (tileX >= 0 && tileX < layer.Width && tileY >= 0 && tileY < layer.Height)
                    {
                        int tileID = layer.GetTileTypeFromTile(tileX, tileY);

                        // check if tile is collidable
                        if (tileID != 0)
                            return false;
                    }
                    i++;
                }
            }
            return true;
        }

        public void SetTurretTop(Sprite sprite)
        {
            shootingSprite = sprite;
            shootingSprite.AttachmentOrigin = shootingSprite.Physics.GetLocalCenter();
            shootingSprite.Physics.Origin = shootingSprite.AttachmentOrigin;
            shootingSprite.AddOriginOffset();

            ShootingPosition = shootingSprite.Physics.Position;
        }
    }
}
