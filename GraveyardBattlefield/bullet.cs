using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraveyardBattlefield
{
    internal class bullet
    {
        //FIELDS
        private int width;
        private int height;
        private int bulletSpeed;
        protected Rectangle position;
        protected Texture2D texture;
        private string shootAngle;
        private bool hitted;

        //PROPERTIES
        public Rectangle Position
        {
            get { return position; }
            set { position = value; }
        }
        public int BulletSpeed
        {
            get { return bulletSpeed; }
        }
        public bool Hitted
        {
            get { return hitted; }
            set { hitted = value; }
        }


        //CONSTRUCTOR
        public bullet(int width, int height, Rectangle position, Texture2D texture, string shootAngle)
        { 
            this.width = width;
            this.height = height;
            this.shootAngle = shootAngle;
            this.position = position;
            this.texture = texture;
            bulletSpeed = 25;
            hitted = false;
        }

        //METHODS
        //Bullet shooting
        public void shootBullet()
        {
            //Upward
            if (shootAngle == "up")
            {
                position.Y -= bulletSpeed; //(where it is shot position +- speed of bullet) will give us the direction of bullet projectile movement
            }
            //Upleft
            else if(shootAngle == "upleft")
            {
                position.Y -= bulletSpeed;
                position.X -= bulletSpeed;
            }
            else if(shootAngle == "upright")
            {
                position.Y -= bulletSpeed;
                position.X += bulletSpeed;
            }
            //Down
            else if (shootAngle == "down")
            {
                position.Y += bulletSpeed;
            }
            //downleft
            else if (shootAngle == "downleft")
            {
                position.Y += bulletSpeed;
                position.X -= bulletSpeed;
            }
            else if (shootAngle == "downright")
            {
                position.Y += bulletSpeed;
                position.X += bulletSpeed;
            }
            //Right
            else if (shootAngle == "right")
            {
                position.X += bulletSpeed;
            }
            //Left
            else if (shootAngle == "left")
            {
                position.X -= bulletSpeed;
            }
        }

        //Draw bullet
        public void Draw(SpriteBatch sb)
        {
            //Rectangle zombieRect = new Rectangle(position.X, position.Y, 20, 20);
            sb.Draw(texture, new Rectangle(position.X, position.Y, 20, 20), Color.White);
        }
    }
}
