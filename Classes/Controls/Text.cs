using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RocketJumper.Classes.Controls
{
    public class TextComponent : Component
    {
        public Vector2 Position;
        public string Text;
        private SpriteFont font;

        public Vector2 CenterOffset
        {
            get { return -font.MeasureString(Text) / 2; }
        }

        public TextComponent(SpriteFont font)
        {
            this.font = font;
        }

        public override void Update(GameTime gameTime)
        {
            // Placeholder
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, Text, Position + CenterOffset, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);
        }
    }
}
