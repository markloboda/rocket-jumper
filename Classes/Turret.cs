using Microsoft.Xna.Framework;
using RocketJumper.Classes.MapData;
using System;

namespace RocketJumper.Classes
{
    public class Turret
    {
        public Sprite baseSprite;
        private Level level;
        private Sprite shootingSprite;

        public Vector2 ShootingPosition;

        public Turret(Sprite baseSprite, Level level)
        {
            this.baseSprite = baseSprite;
            this.level = level;

            SetTurretTop(baseSprite.Children[0]);
        }

        public void Update(GameTime gameTime)
        {
            AimAt(level.Player.PlayerSprite.Physics.GetGlobalCenter());
        }

        public void AimAt(Vector2 target)
        {
            // get the rotation of the turret to aim at the target
            Vector2 direction = target - ShootingPosition;
            shootingSprite.Physics.Rotation = MathF.Atan2(direction.Y, direction.X);
        }

        public void SetTurretTop(Sprite mapObject)
        {
            shootingSprite = mapObject;
            ShootingPosition = new Vector2(shootingSprite.Physics.Position.X + shootingSprite.Physics.Size.X / 2, shootingSprite.Physics.Position.Y + shootingSprite.Physics.Size.Y / 2);
            shootingSprite.AttachmentOrigin = shootingSprite.Physics.GetLocalCenter();
        }
    }
}
