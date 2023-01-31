using Microsoft.Xna.Framework;

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

        public void AddVerticalOffset(float yOffsetAmount)
        {
            Matrix offset = Matrix.CreateTranslation(
                0,
                yOffsetAmount,
                0);
            Transform *= offset;
        }

        public Rectangle GetCameraRectangle()
        {
            Vector2 cameraPosition = Vector2.Transform(Vector2.Zero, Matrix.Invert(Transform));

            Rectangle cameraRectangle = new Rectangle(
                (int)cameraPosition.X,
                (int)cameraPosition.Y,
                MyGame.ActualWidth,
                MyGame.ActualHeight);

            return cameraRectangle;
        }

        public Vector2 GetScreenPosition(Vector2 worldPosition)
        {
            Vector2 screenPosition = Vector2.Transform(worldPosition, Transform);

            return screenPosition;
        }
    }
}