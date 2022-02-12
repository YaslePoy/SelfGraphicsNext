using SelfGraphicsNext.RayGraphics.Graphics3D.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfGraphicsNext.RayGraphics.Graphics3D.Rendering
{
    public class LightSouce
    {
        public Point3 Position { get; set; }

        public double LightForce;

        public LightSouce(Point3 position, double lightForce)
        {
            Position = position;
            LightForce = lightForce;
        }
        public LightSouce()
        {
            Position = Point3.Zero;
            LightForce = 1;
        }
    }
}
