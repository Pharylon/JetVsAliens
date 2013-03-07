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
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Jet jet;
        List<AlienShip> aliens = new List<AlienShip>();
        List<Projectile> enemyProjectiles = new List<Projectile>();
        List<Projectile> playerProjectiles = new List<Projectile>();
        List<Explosion> explosions = new List<Explosion>();

        int alienShipID = 0;
        Vector2 alienShipSpeed = new Vector2(1, 1);
        int lives = 3;
        int score = 0;

        Writer scoreWriter;

        Random random = new Random();

        Texture2D laserBulletTexture;
        Texture2D smallExplosionTexture;
        Texture2D largeExplosionTexture;
        Texture2D jetTexture;
        Texture2D alienShip1Texture;
        Texture2D numbersTexture;
        Texture2D bulletTexture;

        public Game()
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
            bulletTexture = Content.Load<Texture2D>(@"Images\bullet");

            //Load initial enemy ships, player jet and class for writing score
            CreateShipString(5, new Vector2(200, 100));
            loadJet();
            scoreWriter = new Writer(numbersTexture, Vector2.Zero);

        }

        private void loadJet()
        {
            jet = new Jet(jetTexture, new Vector2(Window.ClientBounds.Width / 2, Window.ClientBounds.Height), new Vector2(5, 5));
        }

        //To do: completely revamp this method to allow for calling all kinds of different ships in different circumstances. Only have the base five now.
        private void CreateShipString(int numberToCreate, Vector2 position)
        {
            for (int i = 0; i <= numberToCreate; i++)
            {
                AlienShip alien = new AlienShip(alienShip1Texture, position, alienShipSpeed, new Vector2(1, 1), random, alienShipID);
                aliens.Add(alien);
                alien.Explosion += alien_explosion;
                alienShipID++;
                position.X += 20;
                position.Y += 20;
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

            //Runs jet.Update and if returns true, fires a bullet. Will eventaully need to modify this so it can
            //fire missles and other types of projectiles.
            if (jet.Update(gameTime, Window.ClientBounds))
                playerProjectiles.Add(new Projectile(bulletTexture, new Point(1, 4), jet.GunLocation, true, new Vector2(0, 20)));

            //Loops through all AlienShip.Updates, checks if they have been destroyed and fires shots if AlienShip.Update returns true.
            for (int i = aliens.Count - 1; i >= 0; i--)
            {
                aliens[i].Update(gameTime, Window.ClientBounds);
                if (aliens[i].CheckIfFiredShot())
                    enemyProjectiles.Add(new Projectile(laserBulletTexture, new Point(6, 14), aliens[i].GunLocation, false));
                if (aliens[i].detectCollision(jet.collisionRectangle))
                {
                    aliens[i].OnExplosion(new ExplosionEventArgs(aliens[i].Postion, aliens[i].PointsWorth, false));
                    jetExplosion();
                }
            }

            //Loops through enemy projectiles to see if they hit hte jet. Removes them if they go off screen.
            for (int i = enemyProjectiles.Count - 1; i >= 0; i--)
            {
                enemyProjectiles[i].Update(gameTime, Window.ClientBounds);
                if (jet.detectCollision(enemyProjectiles[i].collisionRectangle))
                    jetExplosion();
                if (enemyProjectiles[i].Postion.Y > Window.ClientBounds.Height || enemyProjectiles[i].Postion.Y < 0)
                    enemyProjectiles.Remove(enemyProjectiles[i]);
            }

            //Loops through player projectiles to see if they hit an enemy. Removes them if they go off screen.
            for (int i = playerProjectiles.Count - 1; i >= 0; i--)
            {
                for (int n = aliens.Count - 1; n >= 0; n--)
                {
                    if (aliens[n].detectCollision(playerProjectiles[i].collisionRectangle))
                    {
                        aliens[n].OnExplosion(new ExplosionEventArgs(aliens[n].Postion, aliens[n].PointsWorth, false));
                        playerProjectiles.Remove(playerProjectiles[i]);
                        continue; //Bullet has been destroyed, so can't check it against any other alien ships. Continue breaks us out of the loop.
                    }
                }
                playerProjectiles[i].Update(gameTime, Window.ClientBounds);
                if (playerProjectiles[i].Postion.Y > Window.ClientBounds.Height || playerProjectiles[i].Postion.Y < 0)
                    playerProjectiles.Remove(playerProjectiles[i]);
            }

            //Updates all explosions.
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
            //Clear the window, draw background. Star the spriteBatch and pass it to each object so it can draw itself.

            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend); 

            jet.Draw(gameTime, spriteBatch);

            foreach (AlienShip alien in aliens)
                alien.Draw(gameTime, spriteBatch);
            
            foreach (Projectile laser in enemyProjectiles)
                laser.Draw(gameTime, spriteBatch);

            foreach (Projectile bullet in playerProjectiles)
                bullet.Draw(gameTime, spriteBatch);

            foreach (Explosion explosion in explosions)
                explosion.Draw(gameTime, spriteBatch);

            scoreWriter.Draw(gameTime, spriteBatch, score);

            spriteBatch.End();            

            base.Draw(gameTime);
        }

        //When an alien explodes, it must be removed and an explosion generated
        private void alien_explosion(object sender, ExplosionEventArgs e)
        {
            if (sender is AlienShip)
            {
                aliens.Remove(sender as AlienShip);
                explosions.Add(new Explosion(smallExplosionTexture, e.Position, e.BigExplosion));
                score += e.Points;
            }
        }

        //Destroys current jet, brings on new one. 
        //TODO: Make it pay attention to lives.
        private void jetExplosion()
        {
            explosions.Add(new Explosion(largeExplosionTexture, jet.Postion, true));
            lives--;
            loadJet();
        }
    }
}

