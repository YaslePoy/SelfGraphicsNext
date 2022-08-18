using SelfGraphicsNext.RayGraphics.Graphics3D.Rendering;
using SFML.Graphics;
using System.Numerics;

namespace SelfGraphicsNext.RayGraphics.Graphics3D.Geometry
{
    public class Polygon
    {
        public Color Color;
        public readonly Point3[] points;
        public readonly Point3 Normal;
        public readonly double DRatio;
        public Plane PlanaData;

        public Polygon(List<Point3> ends, Point3 normal)
        {
            points = ends.ToArray();
            Normal = normal;
            Color = Color.White;
            if (ends.Count == 3)
                DRatio = Vector3.Dot(-points[0].Vector, Normal.Vector);
            //PlanaData = Plane.CreateFromVertices(points[0].Vector, points[1].Vector, points[2].Vector);
            else
                DRatio = 0;
        }

        public bool Colide(Ray3 ray, out Point3 colision)
        {
            colision = new Point3();
            var vertRatio = Math.Cos(ray.Direction.Vertical.AngleGrads * Utils.ToRad);
            var dirHor = ray.Direction.Horisontal.AngleGrads * Utils.ToRad;
            var mpl = new Vector3((float)(Math.Cos(dirHor) * vertRatio), (float)(Math.Sin(dirHor) * vertRatio), (float)Math.Sin(ray.Direction.Vertical.AngleGrads * Utils.ToRad));
            var xyz = ray.Position.Vector;
            var abc = Normal.Vector;
            double upper = DRatio + abc.X * xyz.X + abc.Y * xyz.Y + abc.Z * xyz.Z/*Plane.DotCoordinate(PlanaData, xyz)*/;
            float lower = abc.X * mpl.X + abc.Y * mpl.Y + abc.Z * mpl.Z/*Plane.DotNormal(PlanaData, mpl)*/;
            if (lower == 0 && upper == 0)
                return false;
            if (upper > 0 && lower == 0)
                return false;
            var tRatio = -(upper / lower);
            if (tRatio < 0)
                return false;
            colision = new Point3(xyz + (mpl * (float)tRatio));
            colision.Color = Color;
            colision.Distance = /*(colision.Vector - ray.Position.Vector).Length()*/Vector3.Distance(colision.Vector, xyz);
            Vector2[] poins = new Vector2[4];
            if (Normal.Vector.X != 0)
            {
                poins[0] = new Vector2(colision.Vector.Y, colision.Vector.Z);
                poins[1] = new Vector2(points[0].Vector.Y, points[0].Vector.Z);
                poins[2] = new Vector2(points[1].Vector.Y, points[1].Vector.Z);
                poins[3] = new Vector2(points[2].Vector.Y, points[2].Vector.Z);
            }
            else if (Normal.Y != 0)
            {
                poins[0] = new Vector2(colision.Vector.X, colision.Vector.Z);
                poins[1] = new Vector2(points[0].Vector.X, points[0].Vector.Z);
                poins[2] = new Vector2(points[1].Vector.X, points[1].Vector.Z);
                poins[3] = new Vector2(points[2].Vector.X, points[2].Vector.Z);
            }
            else
            {
                poins[0] = new Vector2(colision.Vector.X, colision.Vector.Y);
                poins[1] = new Vector2(points[0].Vector.X, points[0].Vector.Y);
                poins[2] = new Vector2(points[1].Vector.X, points[1].Vector.Y);
                poins[3] = new Vector2(points[2].Vector.X, points[2].Vector.Y);
            }
            return Utils.PoinInTringle(poins);
        }

        //public static bool operator ==(Polygon p1, Polygon p2) => p1.DRatio == p2.DRatio;
        //public static bool operator !=(Polygon p1, Polygon p2) => p1.DRatio != p2.DRatio;

    }
}
