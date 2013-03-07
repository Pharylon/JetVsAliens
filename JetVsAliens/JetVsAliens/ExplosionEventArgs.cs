using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JetVsAliens
{
    class ExplosionEventArgs : EventArgs
    {
        public bool BigExplosion { get; private set; }
        public Vector2 Position { get; private set; }

        public ExplosionEventArgs(Vector2 position, bool bigExplosion)
        {
            this.BigExplosion = bigExplosion;
            this.Position = position;
        }
    }
}
