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

namespace GameProject
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        GameConstants height = new GameConstants();
        

        // game objects.
        Burger burger;
        List<TeddyBear> bears = new List<TeddyBear>();
        static List<Projectile> projectiles = new List<Projectile>();
        List<Explosion> explosions = new List<Explosion>();
        
        

        // projectile and explosion sprites. Saved so they don't have to
        // be loaded every time projectiles or explosions are created
        static Texture2D frenchFriesSprite;
        static Texture2D teddyBearProjectileSprite;
        static Texture2D explosionSpriteStrip;

        // scoring support
        int score = 0;
        string scoreString = GameConstants.SCORE_PREFIX+0;

        // health support
        string healthString = GameConstants.HEALTH_PREFIX + 
            GameConstants.BURGER_INITIAL_HEALTH;
        bool burgerDead = false;

        // text display support
        SpriteFont font;

         //sound effects
        SoundEffect burgerDamage;
        SoundEffect burgerDeath;
        SoundEffect burgerShot;
        SoundEffect explosion;
        SoundEffect teddyBounce;
        SoundEffect teddyShot;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // set resolution // BE CAREFUL HERE since the GameConstants WINDOW_WIDTH AND WINDOW HEIGHT WILL BE USED TO DECLARE WHERE THE ITEMS WILL APPEAR
            graphics.PreferredBackBufferWidth = GameConstants.WINDOW_WIDTH;
            graphics.PreferredBackBufferHeight = GameConstants.WINDOW_HEIGHT;
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            RandomNumberGenerator.Initialize();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // load audio content
            burgerShot = Content.Load<SoundEffect>("BurgerShot");
            burgerDamage = Content.Load<SoundEffect>("BurgerDamage");
            burgerDeath = Content.Load<SoundEffect>("BurgerDeath");
            explosion = Content.Load<SoundEffect>("EXPLOSION3");
            teddyBounce = Content.Load<SoundEffect>("TeddyBounce");
            teddyShot = Content.Load<SoundEffect>("TeddyShot");


            // load sprite font
            explosionSpriteStrip = Content.Load<Texture2D>("explosion");

            // load projectile and explosion sprites
            teddyBearProjectileSprite = Content.Load < Texture2D >( "teddybearprojectile");
            frenchFriesSprite = Content.Load<Texture2D>("frenchfries"); // if you place your cursor above .Load it says what it misses and how to load it

            // add initial game objects
            for (int i = 0; i < GameConstants.MAX_BEARS; i++)
            {
                SpawnBear();
            }
            

            //BE CAREFUL HERE SINCE IT MUST BE PLACED FROM THE BOTTOM 1/8 AND NOT THE TOP THAT S WHY YOU DO * 7/8
            burger = new Burger(Content, "burger", GameConstants.WINDOW_WIDTH / 2, GameConstants.WINDOW_HEIGHT*7/8, burgerShot);
                
                 
            // set initial health and score strings
            font = Content.Load<SpriteFont>("Arial20");
            healthString = GameConstants.HEALTH_PREFIX + burger.Health;
            scoreString = GameConstants.SCORE_PREFIX+ score;
           

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
       
        protected override void Update(GameTime gameTime )
        {
            KeyboardState keyboard = Keyboard.GetState();
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            // get current mouse state and update burger
           // MouseState mouse = Mouse.GetState();// If we don't write this line for mouse state it won t find it and tell the name mouse doesn't exist
           burger.Update(gameTime, keyboard);

          

            // update other game objects
            foreach (TeddyBear bear in bears)
            {
                bear.Update(gameTime);
            }
            foreach (Projectile projectile in projectiles)
            {
                projectile.Update(gameTime);
            }
            foreach (Explosion explosion in explosions)
            {
                explosion.Update(gameTime);
            }

            // check and resolve collisions between teddy bears
            for (int i = 0; i < bears.Count - 1; i++)
            {
                for (int j = i + 1; j < bears.Count; j++)
                {
                    if (bears[i].Active &&
                        bears[j].Active)
                    {
                        CollisionResolutionInfo teddyCollision = CollisionUtils.CheckCollision
                 (
                    gameTime.ElapsedGameTime.Milliseconds,
                    GameConstants.WINDOW_WIDTH,
                    GameConstants.WINDOW_HEIGHT,
                    bears[i].Velocity,
                    bears[i].DrawRectangle,
                    bears[j].Velocity,
                    bears[j].DrawRectangle

                    );
                        if (teddyCollision != null)
                        {
                            {
                                teddyBounce.Play();
                            }
                            if (teddyCollision.FirstOutOfBounds)
                            {

                                bears[i].Active = false;
                                
                            }

                           if (teddyCollision.FirstOutOfBounds == false)
                            {

                                bears[i].Velocity = teddyCollision.FirstVelocity;

                                bears[i].DrawRectangle = teddyCollision.FirstDrawRectangle;

                            }

                           if (teddyCollision.SecondOutOfBounds)
                           {

                               bears[j].Active = false;

                           }

                           else if(teddyCollision.SecondOutOfBounds == false)
                            {

                                bears[j].Velocity = teddyCollision.SecondVelocity;

                                bears[j].DrawRectangle = teddyCollision.SecondDrawRectangle;
                            }
                        }
                    }
                }
            }
                // check and resolve collisions between burger and teddy bears
            for (int i = 0; i < bears.Count; i++)
            foreach(TeddyBear bear in bears )
            {
                if(bears[i].Active && bears[i].CollisionRectangle.Intersects(burger.CollisionRectangle))//Be careful here to use correct the [i] else you can't access the active and the other properties
                {
                    burger.Health = burger.Health - GameConstants.BEAR_DAMAGE;
                    {
                        burgerDamage.Play();
                    }
                    burger.Health =burger.Health- GameConstants.BEAR_DAMAGE ;
                    healthString = GameConstants.HEALTH_PREFIX + burger.Health;
                    CheckBurgerKill();
                    bears[i].Active = false;
                    explosions.Add(new Explosion(explosionSpriteStrip, bears[i].CollisionRectangle.Center.X,
                        bears[i].CollisionRectangle.Center.Y,explosion));
                }
            }
                // check and resolve collisions between burger and projectiles
            foreach (Projectile bearProjectile in projectiles)
            {
                //Don t forget the type 
                if (bearProjectile.Type == ProjectileType.TeddyBear &&
                    bearProjectile.Active == true && bearProjectile.CollisionRectangle.Intersects(burger.CollisionRectangle))
                    {
                        {
                            burgerDamage.Play();
                        }

                        burger.Health -= 5;//Another way of reducing the burger's health since the projectile bears do 5 damage
                        burger.Health = burger.Health-GameConstants.TEDDY_BEAR_PROJECTILE_DAMAGE;
                        healthString = GameConstants.HEALTH_PREFIX + burger.Health;
                        CheckBurgerKill();
                        bearProjectile.Active = false;
                    }
            }

                // check and resolve collisions between teddy bears and projectiles
                foreach (TeddyBear bigTeddyBear in bears)
                    foreach (Projectile projectile in projectiles)
                        if (projectile.Type == ProjectileType.FrenchFries && bigTeddyBear.Active && projectile.Active &&
                            bigTeddyBear.CollisionRectangle.Intersects(projectile.CollisionRectangle))
                        {
                            bigTeddyBear.Active = false;
                            projectile.Active = false;
                            score = score + GameConstants.BEAR_POINTS; // here we give the score variable the extra damage
                            scoreString =  GameConstants.SCORE_PREFIX+score ; //and here we say what the scoreString to appear in the screen as a string if we place the + score first then it appears first in screen
                            //Also aboe if we don t had the GameConstants as string it would take the score since it's an int
                            explosions.Add(new Explosion (explosionSpriteStrip, bigTeddyBear.CollisionRectangle.Center.X, bigTeddyBear.CollisionRectangle.Center.Y,explosion));
                        }
          
            // clean out inactive teddy bears and add new ones as necessary
            for (int i = bears.Count - 1; i >= 0; i--)
            {
                if (bears[i].Active== false)// Same as (!bears[i].Active)
                {
                    
                    bears.RemoveAt(i);
                    //This method is already created so we just call it here
                    SpawnBear();
                }
            }
            // clean out inactive projectiles
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                if (!projectiles[i].Active)
                {
                    projectiles.RemoveAt(i);
                }
            }
            // clean out finished explosions
            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                if (explosions[i].Finished== true)
                {
                    explosions.RemoveAt(i);
                }
            }

                base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Azure);

            spriteBatch.Begin();

            // draw game objects
            burger.Draw(spriteBatch);

            foreach (TeddyBear bear in bears)
            {
                bear.Draw(spriteBatch);
            }
            foreach (Projectile projectile in projectiles)
            {
                projectile.Draw(spriteBatch);
            }
            foreach (Explosion explosion in explosions)
            {
                explosion.Draw(spriteBatch);
            }

            // draw score and health
            //Here we draw the text we want that's why we put Drawstring//It needs 4 arguments like below
            spriteBatch.DrawString(font, scoreString, GameConstants.SCORE_LOCATION, Color.Black);
            spriteBatch.DrawString(font, healthString, GameConstants.HEALTH_LOCATION, Color.Blue);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        #region Public methods

        /// <summary>
        /// Gets the projectile sprite for the given projectile type
        /// </summary>
        /// <param name="type">the projectile type</param>
        /// <returns>the projectile sprite for the type</returns>
        public static Texture2D GetProjectileSprite(ProjectileType type)
        {
            // replace with code to return correct projectile sprite based on projectile type
            if (ProjectileType.FrenchFries == type)
            {

                return frenchFriesSprite;
            }
            if (ProjectileType.TeddyBear == type)
            {
                return teddyBearProjectileSprite;
            }
            else
            { 
                return null; 
            }
        }

        /// <summary>
        /// Adds the given projectile to the game
        /// </summary>
        /// <param name="projectile">the projectile to add</param>
        public static void AddProjectile(Projectile projectile)
        {
            projectiles.Add(projectile);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Spawns a new teddy bear at a random location
        /// </summary>
        private void SpawnBear()
        {
            // generate random location
            int x = GetRandomLocation(GameConstants.SPAWN_BORDER_SIZE, GameConstants.WINDOW_WIDTH - (2 * GameConstants.SPAWN_BORDER_SIZE));
            int y = GetRandomLocation(GameConstants.SPAWN_BORDER_SIZE, GameConstants.WINDOW_HEIGHT - (2 * GameConstants.SPAWN_BORDER_SIZE));


            // generate random velocity
            float speed = GameConstants.MIN_BEAR_SPEED + RandomNumberGenerator.NextFloat(GameConstants.BEAR_SPEED_RANGE);

            float angle = RandomNumberGenerator.NextFloat(2 * (float)Math.PI);
            Vector2 velocity = new Vector2((speed * (float)Math.Cos(angle)), (speed * (float)Math.Sin(angle)));


            // create new bear
            TeddyBear newBear = new TeddyBear(Content, "teddybear", x, y, velocity, teddyBounce, teddyShot);
            // make sure we don't spawn into a collision
            List<Rectangle> collisionRectangles = GetCollisionRectangles();
            while ((CollisionUtils.IsCollisionFree(newBear.DrawRectangle, collisionRectangles))==false)
            {
                newBear.X = GetRandomLocation(GameConstants.SPAWN_BORDER_SIZE,
                    graphics.PreferredBackBufferWidth - 2 * GameConstants.SPAWN_BORDER_SIZE);
                newBear.Y = GetRandomLocation(GameConstants.SPAWN_BORDER_SIZE,
                    graphics.PreferredBackBufferHeight - 2 * GameConstants.SPAWN_BORDER_SIZE);
            }


            // add new bear to list
            bears.Add(newBear);
        }

        /// <summary>
        /// Gets a random location using the given min and range
        /// </summary>
        /// <param name="min">the minimum</param>
        /// <param name="range">the range</param>
        /// <returns>the random location</returns>
        private int GetRandomLocation(int min, int range)
        {
            return min + RandomNumberGenerator.Next(range);
        }

        /// <summary>
        /// Gets a list of collision rectangles for all the objects in the game world
        /// </summary>
        /// <returns>the list of collision rectangles</returns>
        private List<Rectangle> GetCollisionRectangles()
        {
            List<Rectangle> collisionRectangles = new List<Rectangle>();
      //    collisionRectangles.Add(burger.CollisionRectangle);
            foreach (TeddyBear bear in bears)
            {
                collisionRectangles.Add(bear.CollisionRectangle);
            }
            foreach (Projectile projectile in projectiles)
            {
                collisionRectangles.Add(projectile.CollisionRectangle);
            }
            foreach (Explosion explosion in explosions)
            {
                collisionRectangles.Add(explosion.CollisionRectangle);
            }
            return collisionRectangles;
        }

        /// <summary>
        /// Checks to see if the burger has just been killed
        /// </summary>
        private void CheckBurgerKill()
        {
            if (burger.Health <=0 && burgerDead ==false)
            {
                burgerDead = true;
                burgerDeath.Play();
               
            }
        }

        #endregion
    }
}
