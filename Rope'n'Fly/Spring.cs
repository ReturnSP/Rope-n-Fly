using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rope_n_Fly
{
    internal class Spring
    {
        public Node Node1 { get; }
        public Node Node2 { get; }
        public float RestLength { get; }
        public float Stiffness { get; }

        public Spring(Node node1, Node node2, float stiffness, float restLength)
        {
            Node1 = node1;
            Node2 = node2;
            Stiffness = stiffness;
            RestLength = restLength;
        }

        public void ApplyForce()
        {
            var dx = Node2.Position.X - Node1.Position.X;
            var dy = Node2.Position.Y - Node1.Position.Y;

            var distance = (float)Math.Sqrt(dx * dx + dy * dy);

            var forceMagnitude = Stiffness * (distance - RestLength);
            var forceX = forceMagnitude * (dx / distance);
            var forceY = forceMagnitude * (dy / distance);

            Node1.ApplyForce(new PointF(forceX, forceY));
            Node2.ApplyForce(new PointF(-forceX, -forceY));
        }
    }
}
