using SelfGraphicsNext.BaseGraphics;
using SelfGraphicsNext.RayGraphics.Graphics3D.Rendering;
using SFML.Graphics;

namespace SelfGraphicsNext.RayGraphics.Graphics3D.Geometry
{
    public class Polygon
    {
        public Color Color;
        public readonly List<Point3> points;
        public readonly Point3 Normal;
        public double DRatio;

        public Polygon(List<Point3> ends, Point3 normal)
        {
            points = ends;
            Normal = normal;
            Color = Color.White;
            if (ends.Count == 3)
                DRatio = (-points[0].Vector.X) * Normal.Vector.X + (-points[0].Vector.Y) * Normal.Vector.Y + (-points[0].Vector.Z) * Normal.Vector.Z;
            else
                DRatio = 0;
        }

        public bool Colide(Ray3 ray, out Point3 colision)
        {
            colision = new Point3();
            var mpl = ray.Direction.GetVector();
            var xyz = ray.Position;
            var abc = Normal;
            double upper = DRatio + abc.Vector.X * xyz.Vector.X + abc.Vector.Y * xyz.Vector.Y + abc.Vector.Z * xyz.Vector.Z;
            double lower = abc.Vector.X * mpl.Vector.X + abc.Vector.Y * mpl.Vector.Y + abc.Vector.Z * mpl.Vector.Z;
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
            if (Normal.Vector.X != 0)
            {
                poins[0] = new Point(colision.Vector.Y, colision.Vector.Z);
                poins[1] = new Point(points[0].Vector.Y, points[0].Vector.Z);
                poins[2] = new Point(points[1].Vector.Y, points[1].Vector.Z);
                poins[3] = new Point(points[2].Vector.Y, points[2].Vector.Z);
            }    
            else if (Normal.Y != 0)
            {
                poins[0] = new Point(colision.Vector.X, colision.Vector.Z);
                poins[1] = new Point(points[0].Vector.X, points[0].Vector.Z);
                poins[2] = new Point(points[1].Vector.X, points[1].Vector.Z);
                poins[3] = new Point(points[2].Vector.X, points[2].Vector.Z);
            }
            else
            {
                poins[0] = new Point(colision.Vector.X, colision.Vector.Y);
                poins[1] = new Point(points[0].Vector.X, points[0].Vector.Y);
                poins[2] = new Point(points[1].Vector.X, points[1].Vector.Y);
                poins[3] = new Point(points[2].Vector.X, points[2].Vector.Y);
            }
            return Utils.PoinInTringle(poins);
        }

        //public static bool operator ==(Polygon p1, Polygon p2) => p1.DRatio == p2.DRatio;
        //public static bool operator !=(Polygon p1, Polygon p2) => p1.DRatio != p2.DRatio;

    }
}
