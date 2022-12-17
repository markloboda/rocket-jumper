using Microsoft.Xna.Framework;
using RocketJumper.Classes.MapData;
using System;

namespace RocketJumper.Classes
{
    public class Turret
    {
        public MapObject baseMapObject;
        private Level level;
        private MapObject shootingObject;

        public Vector2 ShootingPosition;

        public Turret(MapObject baseMapObject, Level level)
        {
            this.baseMapObject = baseMapObject;
            this.level = level;

            SetTurretTop(baseMapObject.Children[0]);
        }

        public void Update(GameTime gameTime)
        {
            AimAt(level.Player.PlayerSprite.Physics.GetGlobalCenter());
        }

        public void AimAt(Vector2 target)
        {
            // get the rotation of the turret to aim at the target
            Vector2 direction = target - ShootingPosition;
            shootingObject.Rotation = MathF.Atan2(direction.Y, direction.X);
        }

        public void SetTurretTop(MapObject mapObject)
        {
            shootingObject = mapObject;
            ShootingPosition = new Vector2(shootingObject.ObjectSprite.Physics.Position.X + shootingObject.ObjectSprite.Physics.Size.X / 2, shootingObject.ObjectSprite.Physics.Position.Y + shootingObject.ObjectSprite.Physics.Size.Y / 2);
            shootingObject.Origin = shootingObject.ObjectSprite.Physics.GetLocalCenter();
        }
    }
}
