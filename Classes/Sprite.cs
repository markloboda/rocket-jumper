using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketJumper.Classes
{
    public interface Sprite
    {
        public List<Sprite> Children { get; set; }           // list of mapobject that draw onto the player
        public SpriteEffects Effects { get; set; }
        public Physics Physics { get; set; }

        // if Sprite attached
        public Vector2 AttachmentOffset { get; internal set; }
        public Vector2 AttachmentOrigin { get; internal set; }


        public void Update(GameTime gameTime);
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        public void AddAttachmentOffset();
        public void AddChild(Sprite child);
    }
}
