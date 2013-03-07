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
        Vector2 startingPostion;

        public Writer(Texture2D textureImage, Vector2 position)
            : base(textureImage, position, new Vector2(0, 5), 100)
        {
            startingPostion = position;
            base.frameSize = new Point(14, 20);
            base.collisionOffset = 1;
            base.currentFrame = new Point(0, 0);
            base.sheetSize = new Point(5, 0);
        }

        //Turns a number into a string of characters, then turns each one back into ints
        //And then draws the right number. Seems like a terrible way to do it, but I
        //don't know a better one that accepts an arbitrary number.
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, int writeInt)
        {
            char[] intToCharArray = writeInt.ToString().ToCharArray();
            foreach (char c in intToCharArray)
            {
                currentFrame.X = (int)char.GetNumericValue(c);
                base.Draw(gameTime, spriteBatch);
                position.X += 20;
            }
            position = startingPostion; //Resets the draw position for the next frame.

            // Old logic for single digit below. Not sure why I'm not deleting it yet. :)
            //currentFrame.X = writeInt;              
            //base.Draw(gameTime, spriteBatch);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, string writeString)
        {
            //To do: Create version that takes text.
        }
    }
}
