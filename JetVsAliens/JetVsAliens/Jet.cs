using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JetVsAliens
{
    class Jet : Sprite
    {
        public bool Invincible { get; private set; }
        private bool flyingOntoScreen;
        private int age;
        public bool tryingToFire = false;

        public Jet(Texture2D textureImage, Vector2 position, Vector2 speed)
            : base(textureImage, position, speed)
        {
            loadSheet();
            Invincible = true;
            flyingOntoScreen = true;
            currentFrame.Y = 1;
            base.millisecondsPerFrame = 100;
        }

        private void loadSheet()
        {
            base.frameSize = new Point(45, 72);
            base.collisionOffset = 1;
            base.currentFrame = new Point(0, 0);
            base.sheetSize = new Point(3, 0);
        }

        

        public override Vector2 Direction
        {
            get
            {
                Vector2 inputDirection = Vector2.Zero;
                tryingToFire = false;

                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    inputDirection.X -= 1;
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    inputDirection.X += 1;
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    inputDirection.Y -= 1;
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    inputDirection.Y += 1;
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    tryingToFire = true;

                GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);
                if (gamepadState.ThumbSticks.Left.X != 0)
                    inputDirection.X += gamepadState.ThumbSticks.Left.X;
                if (gamepadState.ThumbSticks.Left.Y != 0)
                    inputDirection.Y -= gamepadState.ThumbSticks.Left.Y;
                if (gamepadState.Triggers.Right > 0)
                    tryingToFire = true;

                if ((inputDirection.X != 0 || inputDirection.Y != 0) && !flyingOntoScreen)
                    EndInvincibility();

                return inputDirection * speed;
            }
        }


        public override bool Update(GameTime gameTime, Rectangle clientBounds)
        {
            if (base.Update(gameTime, clientBounds))
            {
                if (flyingOntoScreen)
                {
                    position.Y -= 3;
                    if (position.Y <= clientBounds.Height - frameSize.Y)
                        flyingOntoScreen = false;
                }
                else
                {
                    if ((position.X + Direction.X) <= (clientBounds.Width - frameSize.X) && (position.X + Direction.X) >= 0)
                        position.X += Direction.X;

                    if ((position.Y + Direction.Y) <= (clientBounds.Height - frameSize.Y) && (position.Y + Direction.Y) >= 0)
                        position.Y += Direction.Y;

                    if (tryingToFire)
                        return true;
                }

                age++;
                if (age > 1000)
                    EndInvincibility();

                if (tryingToFire)
                    return true;
            }
                return false;
        }

        private void EndInvincibility()
        {
            Invincible = false;
            millisecondsPerFrame = 16;
            currentFrame.Y = 0;
        }

        public override bool detectCollision(Rectangle otherRectangle)
        {
            if (Invincible)
                return false;
            else
                return base.detectCollision(otherRectangle);
        }
    }
}
