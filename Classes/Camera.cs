using Microsoft.Xna.Framework;
using RocketJumper.Classes.States;

namespace RocketJumper.Classes
{
    public class Camera
    {

        public Matrix Transform { get; private set; }
        public Matrix Scale { get; private set; }

        public Camera()
        {
        }

        public void Follow(Sprite targetSprite)
        {
            Matrix position = Matrix.CreateTranslation(
                0,
                -targetSprite.Physics.Position.Y - (targetSprite.Physics.Height / 2),
                0);

            Matrix offset = Matrix.CreateTranslation(
                0,
                MyGame.ActualHeight / 1.5f,
                0);

            float scaleAmount = (float)MyGame.ActualWidth / MyGame.VirtualWidth;

            Matrix scale = Matrix.CreateScale(
                scaleAmount,
                scaleAmount,
                1);

            Transform = position * scale * offset;
        }

        public Rectangle GetVisibleArea()
        {
            float scaleAmount = (float)MyGame.ActualWidth / MyGame.VirtualWidth;

            Rectangle visibleArea = new(
                0,
                (int)(-MyGame.ActualHeight / (2 * scaleAmount)),
                MyGame.VirtualWidth,
                MyGame.VirtualHeight);

            return visibleArea;
        }

        public Vector2 GetScreenPosition(Vector2 worldPosition)
        {
            Vector2 screenPosition = Vector2.Transform(worldPosition, Transform);

            return screenPosition;
        }
    }
}