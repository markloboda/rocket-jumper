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
            // make target sprite center
            Matrix scaleOffset = Matrix.CreateTranslation(
                -targetSprite.Physics.Position.X,
                -targetSprite.Physics.Position.Y,
                0);

            // scale to fit screen
            var scaleAmount = (float)MyGame.ActualWidth / MyGame.VirtualWidth;

            Matrix scale = Matrix.CreateScale(
                scaleAmount,
                scaleAmount,
                1.0f);

            // offset to center vertically
            Matrix offset = Matrix.CreateTranslation(
                0,
                MyGame.ActualHeight / 2,
                0);

            Transform = scaleOffset * scale * offset;







            // Matrix offset = Matrix.CreateTranslation(
            //     0,
            //     MyGame.ActualHeight / 2,
            //     0);

            // Matrix position = Matrix.CreateTranslation(
            //     0,
            //     -targetSprite.Physics.Position.Y - (targetSprite.Physics.Height / 2),
            //     0);


            // var scaleAmount = (float)MyGame.ActualWidth / MyGame.VirtualWidth;

            // Matrix scale = Matrix.CreateScale(
            //     scaleAmount,
            //     scaleAmount,
            //     1.0f);

            // // Transform = position * scale * offset;
            // Transform = position * scale * offset;
        }
    }
}