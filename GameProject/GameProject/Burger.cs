using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameProject
{
    /// <summary>
    /// A class for a burger
    /// </summary>
    public class Burger
    {
        #region Fields

        // graphic and drawing info
        Texture2D sprite;
        Rectangle drawRectangle;

        // burger stats
        int health = 100;
        bool active = true;

        // shooting support
        bool canShoot = false;
        int elapsedCooldownTime = 0;

        // sound effect
        SoundEffect shootSound;
        private string p;

        #endregion

        #region Constructors

        /// <summary>
        ///  Constructs a burger
        /// </summary>
        /// <param name="contentManager">the content manager for loading content</param>
        /// <param name="spriteName">the sprite name</param>
        /// <param name="x">the x location of the center of the burger</param>
        /// <param name="y">the y location of the center of the burger</param>
        /// <param name="shootSound">the sound the burger plays when shooting</param>
        public Burger(ContentManager contentManager, string spriteName, int x, int y,
            SoundEffect shootSound)
        {
            LoadContent(contentManager, spriteName, x, y);
            this.shootSound = shootSound;
        }

        public Burger(string p)
        {
            // TODO: Complete member initialization
            this.p = p;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the collision rectangle for the burger
        /// </summary>
        public Rectangle CollisionRectangle
        {
            get { return drawRectangle; }
        }

       
        //damage properties 
        public int Health //you can also make a method which will do the same like this
            //public void Health(int damage)
        //{
        //    health -= damage;
        //    if (health <= 0)
        //    {
        //        health = 0;
        //        active = false;
        //    }
        //}
        {

            get { return health; }

            set
            {

                health = value;

                if (health < 0) health = 0;

                if (health > GameConstants.BURGER_INITIAL_HEALTH) health = GameConstants.BURGER_INITIAL_HEALTH;

            }

        }
       public bool Active
       {
           get { return active; }
           set { active = value; }
       }
        #endregion

       #region Private properties

       /// <summary>
        /// Gets and sets the x location of the center of the burger
        /// </summary>
        private int X
        {
            get { return drawRectangle.Center.X; }
            set
            {
                drawRectangle.X = value - drawRectangle.Width / 2;

                // clamp to keep in range
                if (drawRectangle.X < 0)
                {
                    drawRectangle.X = 0;
                }
                else if (drawRectangle.X > GameConstants.WINDOW_WIDTH - drawRectangle.Width)
                {
                    drawRectangle.X = GameConstants.WINDOW_WIDTH - drawRectangle.Width;
                }
            }
        }

        /// <summary>
        /// Gets and sets the y location of the center of the burger
        /// </summary>
        private int Y
        {
            get { return drawRectangle.Center.Y; }
            set
            {
                drawRectangle.Y = value - drawRectangle.Height / 2;

                // clamp to keep in range
                if (drawRectangle.Y < 0)
                {
                    drawRectangle.Y = 0;
                }
                else if (drawRectangle.Y > GameConstants.WINDOW_HEIGHT - drawRectangle.Height)
                {
                    drawRectangle.Y = GameConstants.WINDOW_HEIGHT - drawRectangle.Height;
                }
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Updates the burger's location based on mouse. Also fires 
        /// french fries as appropriate
        /// </summary>
        /// <param name="gameTime">game time</param>
        /// <param name="mouse">the current state of the mouse</param>
        public void Update(GameTime gameTime, KeyboardState keyboard)
        {
            // burger should only respond to input if it still has health
            if (health > 0)
            {
                //move the burger using keyboard
                if (Keyboard.GetState().IsKeyDown(Keys.D)) //This is the way of saying when a button is pressed do this 
                {
                    X += 5;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.A))
                {
                    X = X - 5;//aNOTHER WAY OF SAYING -=
                }
                if(Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    Y += 5;
                }
                 if(Keyboard.GetState().IsKeyDown(Keys.W))
                 {
                     Y = Y - 5;
                 }
                // move burger using mouse

                //We are doing this below so the character doesn't spin around and stays as it is centered
                //drawRectangle.X = mouse.X - sprite.Width / 2;
              //  drawRectangle.Y = mouse.Y - sprite.Height / 2;
            }
                // clamp burger in window
            if (drawRectangle.Left < 0) // Be careful here to use the .Left else it won't compile
            {
                drawRectangle.X = 0;

            }
            if (drawRectangle.Right > GameConstants.WINDOW_WIDTH) // Here we limit the burger move ,to not be greater than window width right
            {                                                                       // So it won't leave  right 
                drawRectangle.X = GameConstants.WINDOW_WIDTH - drawRectangle.Width;
            }
            if (drawRectangle.Top < 0) //Same as we did above but now for the Y axis which has top and bottom 
            {
                drawRectangle.Y = 0;
            }
            if (drawRectangle.Bottom > GameConstants.WINDOW_HEIGHT)
            {
                drawRectangle.Y = GameConstants.WINDOW_HEIGHT - drawRectangle.Height;
            }

                // update shooting allowed
            if (!canShoot)
            {
                elapsedCooldownTime += gameTime.ElapsedGameTime.Milliseconds;
                if (elapsedCooldownTime >= GameConstants.BURGER_COOLDOWN_MILLISECONDS ||(Keyboard.GetState().IsKeyUp(Keys.Space)))
                {

                    elapsedCooldownTime = 0;
                    canShoot = true;

                }
            }
            // timer concept (for animations) introduced in Chapter 7
            //shoot if appropriate
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && canShoot == true)//if you don't add the && canshoot == true and then canShoot = false; the projectile fires too fast
            {
                canShoot = false;
                Projectile frenchFriesProjectile = new Projectile //Here we need to call all these values and classes from projectile Class
                    (ProjectileType.FrenchFries, Game1.GetProjectileSprite //Total 5 one for ProjectileType type,
                    //one for the Texture2D sprite,int x and y,and the float yvelocity we have created at GameConstants
                    (ProjectileType.FrenchFries), drawRectangle.Center.X, drawRectangle.Y , GameConstants.FRENCH_FRIES_PROJECTILE_SPEED);
                shootSound.Play();
                Game1.AddProjectile(frenchFriesProjectile);
            }
                
           


        }

        /// <summary>
        /// Draws the burger
        /// </summary>
        /// <param name="spriteBatch">the sprite batch to use</param>
        //Here it draws the burger be calling the sprite in location drawreCtangle and what color
        
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite,drawRectangle, Color.White);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Loads the content for the burger
        /// </summary>
        /// <param name="contentManager">the content manager to use</param>
        /// <param name="spriteName">the name of the sprite for the burger</param>
        /// <param name="x">the x location of the center of the burger</param>
        /// <param name="y">the y location of the center of the burger</param>
        private void LoadContent(ContentManager contentManager, string spriteName,
            int x, int y)
        {
            // load content and set remainder of draw rectangle
            sprite = contentManager.Load<Texture2D>(spriteName);
            drawRectangle = new Rectangle(x - sprite.Width / 2,
                y - sprite.Height / 2, sprite.Width,
                sprite.Height);
        }

        #endregion
    }
}
