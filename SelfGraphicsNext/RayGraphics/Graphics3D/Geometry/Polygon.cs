using SelfGraphicsNext.BaseGraphics;
using SelfGraphicsNext.RayGraphics.Graphics3D.Rendering;
using SFML.Graphics;

namespace SelfGraphicsNext.RayGraphics.Graphics3D.Geometry
{
    public class Polygon
    {
        public Color Color { get; set; }
        public readonly List<Point3> points;
        public readonly Point3 Normal;
        public double DRatio;

        public Polygon(List<Point3> ends, Point3 normal)
        {
            points = ends;
            Normal = normal;
            Color = Color.White;
            if (ends.Count == 3)
                DRatio = (-points.First().X) * Normal.X + (-points.First().Y) * Normal.Y + (-points.First().Z) * Normal.Z;
        }

        public bool Colide(Ray3 ray, out Point3 colision)
        {
            colision = null;
            var mpl = ray.Direction.GetVector();
            var xyz = ray.Position;
            var abc = Normal;
            double upper = DRatio + abc.X * xyz.X + abc.Y * xyz.Y + abc.Z * xyz.Z;
            double lower = abc.X * mpl.X + abc.Y * mpl.Y + abc.Z * mpl.Z;
            if (lower == 0 && upper == 0)
                return false;
            if (upper > 0 && lower == 0)
                return false;
            var tRatio = -(upper / lower);
            if (tRatio < 0)
                return false;
            colision = xyz + (mpl * tRatio);
            colision.Color = Color;
            colision.SetDistanceTo(ray.Position);
            Point[] poins = new Point[4];
            if (Normal.X != 0)
            {
                poins[0] = new Point(colision.Y, colision.Z);
                poins[1] = new Point(points[0].Y, points[0].Z);
                poins[2] = new Point(points[1].Y, points[1].Z);
                poins[3] = new Point(points[2].Y, points[2].Z);
            }    
            else if (Normal.Y != 0)
            {
                poins[0] = new Point(colision.X, colision.Z);
                poins[1] = new Point(points[0].X, points[0].Z);
                poins[2] = new Point(points[1].X, points[1].Z);
                poins[3] = new Point(points[2].X, points[2].Z);
            }
            else
            {
                poins[0] = new Point(colision.X, colision.Y);
                poins[1] = new Point(points[0].X, points[0].Y);
                poins[2] = new Point(points[1].X, points[1].Y);
                poins[3] = new Point(points[2].X, points[2].Y);
            }
            return Utils.PoinInTringle(poins);
        }

    }
}
