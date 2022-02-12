using SelfGraphicsNext.BaseGraphics;
using SelfGraphicsNext.RayGraphics.Graphics3D.Rendering;

namespace SelfGraphicsNext.RayGraphics.Graphics3D.Geometry
{
    public class Polygon : Colideble
    {
        public readonly List<Point3> points;
        public readonly Point3 Normal;
        public double DRatio => (-points.First().X) * Normal.X + (-points.First().Y) * Normal.Y + (-points.First().Z) * Normal.Z;

        public Polygon(List<Point3> ends, Point3 normal)
        {
            points = ends;
            Normal = normal;
        }

        public override bool Colide(Ray3 ray, out Point3 colision)
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
           
            if (Normal.X != 0)
                return Utils.PoinInTringle(points.Select(i => new Point(i.Y, i.Z)).ToList(), new Point(colision.Y, colision.Z));
            else if(Normal.Y != 0)
                return Utils.PoinInTringle(points.Select(i => new Point(i.X, i.Z)).ToList(), new Point(colision.X, colision.Z));
            else
                return Utils.PoinInTringle(points.Select(i => new Point(i.X, i.Y)).ToList(), new Point(colision.X, colision.Y));
        }

    }
}
