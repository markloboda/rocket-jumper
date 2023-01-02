using Microsoft.Xna.Framework;
using RocketJumper.Classes.States;

namespace RocketJumper.Classes
{
    public class Camera
    {

        public Matrix Transform { get; private set; }
        public Matrix Scale { get; private set; }

        public Camera(int levelWidth)
        {
        }

        public void Follow(Sprite targetSprite)
        {
            Matrix offset = Matrix.CreateTranslation(
                0,
                MyGame.VirtualHeight / 2,
                0);

            Matrix position = Matrix.CreateTranslation(
                0,
                -targetSprite.Physics.Position.Y - (targetSprite.Physics.Height / 2),
                0);


            var scaleAmount = (float)MyGame.ActualWidth / MyGame.VirtualWidth;

            Matrix scale = Matrix.CreateScale(
                scaleAmount,
                scaleAmount,
                1.0f);

            // Transform = position * offset * scale;
            Transform = position * offset;
        }
    }
}