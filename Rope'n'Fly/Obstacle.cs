using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Deployment.Application;

namespace Rope_n_Fly
{
    internal class Obstacle
    {
        public float x, y, width, height;

        public Obstacle(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public void Update(float playerSpeed)
        {
            // Move the building left based on the player's speed
            x -= 5;
        }
    }
}
