using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JetVsAliens
{
    class Explosion : Sprite
    {
        public override Vector2 Direction { get { return new Vector2(0, 0); } }
        int age = 0;
        public bool remove = false;

        public Explosion(Texture2D textureImage, Vector2 position, bool bigExplosion)
            : base(textureImage, position, new Vector2(0, 5), 100)
        {
            if (bigExplosion)
            {
                base.frameSize = new Point(50, 50);
                base.collisionOffset = 1;
                base.currentFrame = new Point(0, 0);
                base.sheetSize = new Point(5, 0);
            }
            else
            {
                base.frameSize = new Point(20, 20);
                base.collisionOffset = 1;
                base.currentFrame = new Point(0, 0);
                base.sheetSize = new Point(5, 0);
            }
        }

        //Animate explosion frames.
        public override bool Update(GameTime gameTime, Rectangle clientBounds)
        {
            if (base.Update(gameTime, clientBounds))
            {
                age++;
                if (age >= sheetSize.X)
                    remove = true;
                return true;
            }
            else return false;
        }
    }
}

