using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JetVsAliens
{
    class AlienShip : Sprite
    {

        public override Vector2 Direction { get { return direction; } }
        private Vector2 direction = Vector2.Zero;
        Random random;
        public int ID { get; private set; }
        private int frame = 0;
        public int PointsWorth { get; private set; }
        List<AlienShip> formation;

        public event EventHandler<ExplosionEventArgs> Explosion;

        //Where the shots come from.
        public Vector2 GunLocation
        {
            get
            {
                return new Vector2(position.X + collisionRectangle.Width / 2, position.Y + collisionRectangle.Height);
            }
        }

        public AlienShip(Texture2D textureImage, Vector2 position, Vector2 speed, Vector2 direction, Random random, List<AlienShip> formation)
            : base(textureImage, position, speed)
        {
            this.direction = direction;
            this.random = random;
            base.millisecondsPerFrame = 300;
            loadSheet();
            PointsWorth = 10;
            this.formation = formation;
        }

        private void loadSheet()
        {
            frameSize = new Point(29, 26);
            collisionOffset = 1;
            currentFrame = new Point(0, 0);
            sheetSize = new Point(4, 0);
        }

        public override bool Update(GameTime gameTime, Rectangle clientBounds)
        {
            //TODO: Some better AI, at least so it loops around in circles and
            //shit instead of bouncing off the corner of the screen.
            //Eventually, maybe even more in-depth than that.

            //float acceleration = random.Next(100);
            //acceleration = acceleration / 100;

            position += (Direction * speed);

            float turn = .01f;

            if ((position.X + Direction.X) >= (clientBounds.Width - frameSize.X) || (position.X + Direction.X) <= 0)
                direction.X *= -1;
            else
                direction.X += turn;

            if ((position.Y + Direction.Y) >= (clientBounds.Height / 2) || (position.Y + Direction.Y) <= 0)
                direction.Y *= -1;
            else
                direction.Y += turn;

            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                timeSinceLastFrame = 0;
                frame++;
            if (frame >= 6)
                frame = 0;
            switch (frame)
            {
                case 0: currentFrame.X = 0; break;  //This code advances the frames and once
                case 1: currentFrame.X = 1; break;  //it hits the end, reverses back down
                case 2: currentFrame.X = 2; break;  //the sheet.
                case 3: currentFrame.X = 3; break; 
                case 4: currentFrame.X = 2; break;  
                case 5: currentFrame.X = 1; break;
                default: currentFrame.X = 0; break;
            }
                return true;
            }
            else
                return false;
        }

        //Roll the dice to see if it fired a shot.
        public bool CheckIfFiredShot()
        {
            if (random.Next(100) == 0)
                return true;
            else
                return false;
        }


        //Not being used yet. Will use this once I add more logic. Allows the
        //ship to move toward the specified location.
        private bool MoveTowardsPosition(Vector2 destination)
        {
            if (Math.Abs(destination.X - position.X) <= speed.X && Math.Abs(destination.Y - position.Y) <= speed.Y)
                return true;

            float xDif = destination.X - position.X;
            float yDif = destination.Y - position.Y;

            if (xDif > 3)
                position.X += speed.X;
            else if (xDif < -3)
                position.X -= speed.Y;
            else
                position.X += xDif;

            if (yDif > 3)
                position.Y += speed.X;
            else if (yDif < -3)
                position.Y -= speed.Y;
            else position.Y += yDif;

            return false;
        }

        //Raises explosion event. Duh.
        public void OnExplosion(ExplosionEventArgs e)
        {
            EventHandler<ExplosionEventArgs> explosion = Explosion;
            if (explosion != null)
                explosion(this, e);
        }
    }
}
