using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JetVsAliens
{
    class Writer : Sprite
    {
        public override Vector2 Direction {get { return Vector2.Zero;}}

        public Writer(Texture2D textureImage, Vector2 position, bool bigWriter)
            : base(textureImage, position, new Vector2(0, 5), 100)
        {
            if (bigWriter)
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
    }
}
