using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace Rope_n_Fly
{
    internal class Node
    {
        public PointF Position { get; set; }
        public PointF Velocity { get; set; }
        public PointF Force { get; set; }
        public bool IsFixed { get; set; }

        public Node(float x, float y, bool isFixed = false)
        {
            Position = new PointF(x, y);
            Velocity = new PointF(0, 0);
            Force = new PointF(0, 0);
            IsFixed = isFixed;
        }

        public void ApplyForce(PointF force)
        {
            Force = new PointF(Force.X + force.X, Force.Y + force.Y);
        }

        public void Update(float deltaTime)
        {
            if (IsFixed)
                return;

            // Apply gravity
            ApplyForce(new PointF(15f, 35f));
            ApplyForce(new PointF(0, 9.81f));


            // Update velocity with damping
            Velocity = new PointF(
                (Velocity.X + Force.X * deltaTime) * 0.99f,
                (Velocity.Y + Force.Y * deltaTime) * 0.99f
            );

            // Update position
            Position = new PointF(
                Position.X + Velocity.X * deltaTime,
                Position.Y + Velocity.Y * deltaTime
            );

            // Reset force after update
            Force = new PointF(0, 0);
        }
    }
}