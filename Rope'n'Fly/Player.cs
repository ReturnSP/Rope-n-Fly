using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rope_n_Fly
{
    internal class Player
    {
        public float x, y, xSpeed, ySpeed;
        public float size = 15;
        public static int mass = 3;
        public static float jumpSpeed = 20f;

        public Color colour;

        public static Random rand = new Random();
        public Player(float x, float y, float _xSpeed, float _ySpeed)
        {
            this.x = x;
            this.y = y;
            this.xSpeed = _xSpeed;
            this.ySpeed = _ySpeed;
        }

        public void Gravity()
        {
            y += 10;
        }

        public void AirResistance()
        {
            //x -= 5;
        }
        public void WallCollision(int formWidth, int formHeight)
        {
            //Collision with left wall
            if(x < 0)
            {
                x = 0;
            }
            //Collision with right wall
            if(x > formWidth - size)
            {
                x = formWidth - size;
            }
            //Collision with top wall
            if(y < 0+25)
            {
                y = 0+25;
            }
            //Collision with bottom wall
            if(y > formHeight - size -25)
            {
                y = formHeight - size -25;
            }
        }
    }
}
