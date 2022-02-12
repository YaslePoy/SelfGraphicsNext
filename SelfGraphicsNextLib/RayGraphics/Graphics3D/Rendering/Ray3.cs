using SelfGraphicsNext.BaseGraphics;
using SelfGraphicsNext.RayGraphics.Graphics3D.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfGraphicsNext.RayGraphics.Graphics3D.Rendering
{
    public class Ray3
    {
        public Direction3 Direction;
        public Point3 Position { get; set; }
        public Point3 Aim { get; set; }
        public Point ImagePosition { get; internal set; }

        public Ray3(Direction3 direction, Point3 position)
        {
            Direction = direction;
            Position = position;
        }

        public Ray3(Point3 position)
        {
            Position = position;
            Direction = new Direction3();
        }

        public Ray3()
        {
            Position = new Point3(0, 0, 0);
            Direction = new Direction3();
        }

        public bool CollideInScene(Scene scene)
        {
            List<Point3> results = new List<Point3>();
            foreach (PolygonGroup polygon in scene.Objects)
                if (polygon.Colide(this, out Point3 col))
                    results.Add(col);
            if (results.Count > 0)
            {
                Aim = results.MinBy(i => i.Distance);
                return true;
            }
            return false;
        }
    }
}
