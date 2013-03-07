using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace JetVsAliens
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Jet jet;
        List<AlienShip> aliens = new List<AlienShip>();
        List<Laser> lasers = new List<Laser>();
        List<Explosion> explosions = new List<Explosion>();

        int alienShipID = 0;
        int lives = 3;
        int score;

        Random random = new Random();

        Texture2D laserBulletTexture;
        Texture2D smallExplosionTexture;
        Texture2D largeExplosionTexture;
        Texture2D jetTexture;
        Texture2D alienShip1Texture;
        Texture2D numbersTexture;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            jetTexture = Content.Load<Texture2D>(@"Images\jetSheet");
            laserBulletTexture = Content.Load<Texture2D>(@"Images\laserBullet");
            smallExplosionTexture = Content.Load<Texture2D>(@"Images\smallExplosion");
            largeExplosionTexture = Content.Load<Texture2D>(@"Images\largeExplosion");
            alienShip1Texture = Content.Load<Texture2D>(@"Images\alien1");
            numbersTexture = Content.Load<Texture2D>(@"Images\numbers");

            CreateShipString(5, new Vector2(200, 100));
            loadJet();

        }

        private void loadJet()
        {
            jet = new Jet(jetTexture, new Vector2(Window.ClientBounds.Width / 2, Window.ClientBounds.Height), new Vector2(5, 5));
        }

        private void CreateShipString(int p, Vector2 v)
        {
            for (int i = 0; i <= p; i++)
            {
                AlienShip alien = new AlienShip(alienShip1Texture, v, new Vector2(1, 1), new Vector2(1, 1), random, alienShipID);
                aliens.Add(alien);
                alien.Explosion += alien_explosion;
                alienShipID++;
                v.X += 20;
                v.Y += 20;
            }  
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (jet != null)
            {
                jet.Update(gameTime, Window.ClientBounds);
            }

            for (int i = aliens.Count - 1; i >= 0; i--)
            {
                aliens[i].Update(gameTime, Window.ClientBounds);
                if (aliens[i].CheckIfFiredShot())
                    lasers.Add(new Laser(laserBulletTexture, aliens[i].GunLocation));
                if (aliens[i].detectCollision(jet.collisionRectangle))
                    aliens[i].OnExplosion(new ExplosionEventArgs(aliens[i].Postion, false));
            }

            for (int i = lasers.Count - 1; i >= 0; i--)
            {
                lasers[i].Update(gameTime, Window.ClientBounds);
                //if (lasers[i].detectCollision(jet.collisionRectangle))
                if (jet.detectCollision(lasers[i].collisionRectangle))
                    jetExplosion();
                if (lasers[i].Postion.Y > Window.ClientBounds.Height)
                    lasers.Remove(lasers[i]);
            }

            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                explosions[i].Update(gameTime, Window.ClientBounds);
                if (explosions[i].remove)
                    explosions.Remove(explosions[i]);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            jet.Draw(gameTime, spriteBatch);
            foreach (AlienShip alien in aliens)
                alien.Draw(gameTime, spriteBatch);
            
            foreach (Laser laser in lasers)
                laser.Draw(gameTime, spriteBatch);

            foreach (Explosion explosion in explosions)
                explosion.Draw(gameTime, spriteBatch);

            spriteBatch.End();            

            base.Draw(gameTime);
        }

        private void alien_explosion(object sender, ExplosionEventArgs e)
        {
            if (sender is AlienShip)
            {
                aliens.Remove(sender as AlienShip);
                explosions.Add(new Explosion(smallExplosionTexture, e.Position, e.BigExplosion));
            }
        }

        private void jetExplosion()
        {
            explosions.Add(new Explosion(largeExplosionTexture, jet.Postion, true));
            lives--;
            loadJet();
        }
    }
}

