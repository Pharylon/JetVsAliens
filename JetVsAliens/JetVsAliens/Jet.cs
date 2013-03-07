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
        bool flyingOntoScreen;
        int age;
        bool tryingToFire = false;
        int timeSinceLastShot = 0;
        const int timeBetweenShots = 5;
        bool firingLeftSide = true;
        


        public Jet(Texture2D textureImage, Vector2 position, Vector2 speed)
            : base(textureImage, position, speed)
        {
            loadSheet();
            Invincible = true;
            flyingOntoScreen = true;
            currentFrame.Y = 1;
            base.millisecondsPerFrame = 100;
        }

        //This alternates shots on the left and right side
        public Vector2 GunLocation
        {
            get
            {
                if (firingLeftSide)
                    return new Vector2(position.X + collisionRectangle.Width / 2 + 20, position.Y + collisionRectangle.Height / 3);
                else
                    return new Vector2(position.X + collisionRectangle.Width / 2 - 20, position.Y + collisionRectangle.Height / 3);
            }
        }

        private void loadSheet()
        {
            base.frameSize = new Point(45, 72); //Each frame of jet animation is 45 x 72 pixels.
            base.collisionOffset = 1; //Collison sone is not offset.
            base.currentFrame = new Point(0, 0); //Always start at 0, 0 for currentFrame. This is updated in the base Sprite.Update method
            base.sheetSize = new Point(3, 0); // Total number of frames, left to right. Row 0 is main animation, alternate animation (ie, flashign for invincibility) on other rows.
        }

        

        public override Vector2 Direction
        {
            get
            {
                Vector2 inputDirection = Vector2.Zero;
                tryingToFire = false;

                //Get Keybaord Input
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

                //For Xbox Controllers
                GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);
                if (gamepadState.ThumbSticks.Left.X != 0)
                    inputDirection.X += gamepadState.ThumbSticks.Left.X;
                if (gamepadState.ThumbSticks.Left.Y != 0)
                    inputDirection.Y -= gamepadState.ThumbSticks.Left.Y;
                if (gamepadState.Triggers.Right > 0)
                    tryingToFire = true;

                //Breaks "Invincibility" period when the player attempts to move or fire after coming on screen.
                if ((inputDirection.X != 0 || inputDirection.Y != 0 || tryingToFire) && !flyingOntoScreen)
                    EndInvincibility();

                return inputDirection * speed;
            }
        }


        public override bool Update(GameTime gameTime, Rectangle clientBounds)
        {
            if (base.Update(gameTime, clientBounds))
            {
                //If (flyingOntScreen) Jet flys onto screen when new jet is generated, not in player control, otherwise takes 
                //keyboard/controller state gathered earlier and updates state.
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

                    if (tryingToFire && timeSinceLastShot > timeBetweenShots)
                    {
                        timeSinceLastShot = 0; //reset time since last shot
                        firingLeftSide = !firingLeftSide; //toggle which side of plane bullets are coming out of.
                        return true; //returns true if shot fired.
                    }
                    else
                        timeSinceLastShot++; //if it's too soon for another shot, advance the counter.
                }

                age++;
                if (age > 1000)
                    EndInvincibility(); //You don't get to keep the starting invincibliity forever, even if you don't move or fire.
            }
                return false; //returns false if did not fire.
        }

        private void EndInvincibility() //Ends invincibility period.
        {
            Invincible = false;
            millisecondsPerFrame = 16;
            currentFrame.Y = 0;
        }

        //TODO: Make collision area more precise. Put smaller rectangles inside main CollisionRectangle (wing, fuselage, other wing). If 
        //collsions is detected on main rectangle, check smaller ones. Return false if these aren't tripped. This will give a tighter 
        //collision zone.
        public override bool detectCollision(Rectangle otherRectangle) 
        {
            if (Invincible)
                return false;
            else
                return base.detectCollision(otherRectangle);
        }
    }
}
