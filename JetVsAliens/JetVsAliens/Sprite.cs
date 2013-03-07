using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JetVsAliens
{
    abstract class Sprite
    {
        protected Texture2D textureImage;
        protected Point frameSize;
        protected Point currentFrame;
        protected Point sheetSize;
        protected int collisionOffset;
        protected int timeSinceLastFrame = 0;
        protected int millisecondsPerFrame;
        const int defaultMillisecondsPerFrame = 16;
        protected Vector2 speed;
        protected Vector2 position;
        public Vector2 Postion { get { return position; } }

        public abstract Vector2 Direction {get;}

        public Sprite(Texture2D textureImage, Vector2 position, Vector2 speed)
            : this(textureImage, position, speed, defaultMillisecondsPerFrame)
        {
        }

        public Sprite(Texture2D textureImage, Vector2 position, Vector2 speed, int millisecondsPerFrame)
        {
            this.textureImage = textureImage;
            this.position = position;
            this.speed = speed;
            this.millisecondsPerFrame = millisecondsPerFrame;
        }

        //GameTime returns true if it ran, fasle otherwise;
        public virtual bool Update(GameTime gameTime, Rectangle clientBounds)
        {
            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                timeSinceLastFrame = 0;
                ++currentFrame.X;
                if (currentFrame.X >= sheetSize.X)
                {
                    currentFrame.X = 0;
                    //++currentFrame.Y;
                    //if (currentFrame.Y >= sheetSize.Y)
                    //    currentFrame.Y = 0;
                }
                return true;
            }
            else
                return false;
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(textureImage, position, getCurrentRectangle(), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }

        protected Rectangle getCurrentRectangle()
        {
            return new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y);
        }

        public Rectangle collisionRectangle
        {
            get
            {
                return new Rectangle((int)position.X + collisionOffset, (int)position.Y + collisionOffset,
                    (frameSize.X - collisionOffset * 2), (frameSize.Y - collisionOffset * 2));
            }
        }

        public virtual bool detectCollision(Rectangle otherRectangle)
        {
            return otherRectangle.Intersects(this.collisionRectangle);
        }
    }
}
