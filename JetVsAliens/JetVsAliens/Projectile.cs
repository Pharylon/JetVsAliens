using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JetVsAliens
{
    

    class Projectile : Sprite
    {
        protected bool travelUp;

        public Projectile(Texture2D textureImage, Point frameSize, Vector2 position, bool travelUp, Vector2 speed)
            : base(textureImage, position, speed)
        {
            collisionOffset = 1;
            currentFrame = new Point(0, 0);
            sheetSize = new Point(0, 0);
            this.travelUp = travelUp;
            this.frameSize = frameSize;
        }

        public Projectile(Texture2D textureImage, Point frameSize, Vector2 position, bool travelUp)
            : base(textureImage, position, new Vector2(0, 3))
        {
            collisionOffset = 1;
            currentFrame = new Point(0, 0);
            sheetSize = new Point(0, 0);
            this.travelUp = travelUp;
            this.frameSize = frameSize;
        }

        private Vector2 direction = new Vector2(0, 1);

        public override Vector2 Direction
        {
            get { return direction; }
        }

        public override bool Update(GameTime gameTime, Rectangle clientBounds)
        {
            if (travelUp)
                position -= (Direction * speed);
            else
                position += (Direction * speed);

            return base.Update(gameTime, clientBounds);
        }
    }
}
