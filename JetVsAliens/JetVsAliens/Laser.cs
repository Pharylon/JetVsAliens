using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JetVsAliens
{
    

    class Laser : Sprite
    {

        public Laser(Texture2D textureImage, Vector2 position)
            : base(textureImage, position, new Vector2(0, 3))
        {
            frameSize = new Point(6, 14);
            collisionOffset = 1;
            currentFrame = new Point(0, 0);
            sheetSize = new Point(0, 0);
        }

        private Vector2 direction = new Vector2(0, 1);

        public override Vector2 Direction
        {
            get { return direction; }
        }

        public override bool Update(GameTime gameTime, Rectangle clientBounds)
        {
            position += (Direction * speed);
            return base.Update(gameTime, clientBounds);
        }
    }
}
