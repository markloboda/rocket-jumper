using Microsoft.Xna.Framework;

namespace RocketJumper.Classes
{
    public class Camera
    {

        public Matrix Transform { get; private set; }

        public Camera(int levelWidth)
        {
            // scale the camera to the level width
            Transform = Matrix.CreateScale(new Vector3(levelWidth / MyGame.ScreenWidth, 1, 0));
        }

        public void Follow(Sprite targetSprite)
        {
            Matrix offset = Matrix.CreateTranslation(
                0,
                MyGame.ScreenHeight / 2,
                0);

            Matrix position = Matrix.CreateTranslation(
                0,
                -targetSprite.Physics.Position.Y - (targetSprite.Physics.Height / 2),
                0);

            Transform = position * offset;
        }
    }
}