using SelfGraphicsNext.RayGraphics.Graphics3D.Geometry;
using SelfGraphicsNext.RayGraphics.Graphics3D.Rendering;
using SelfGraphicsNext;
using SFML.Graphics;
using System.Numerics;
using System;

namespace SelfFunctionsTester
{
    internal class Program
    {
        static Random random = new Random(5);
        public static int localRand() => random.Next(-10, 10);

        static void Main(string[] args)
        {
            int N = 5;
            Ray3[] rays = new Ray3[N];
            TestPolygon[] testPolygons = new TestPolygon[N];
            for (int i = 0; i < N; i++)
            {
                rays[i] = new Ray3(new Vector3(localRand(), localRand(), localRand()), new Point3(localRand(), localRand(), localRand()));
                testPolygons[i] = new TestPolygon(new Point3(localRand(), localRand(), localRand()), localRand());
            }
            var ress = new List<float>();
            for (int i = 0; i < N; i++)
            {
                ress.Add(testPolygons[i].Colide(rays[i]));
            }
            Console.WriteLine(String.Join(", ", ress));

        }
    }

    public class TestPolygon
    {
        public Color Color;
        public readonly Point3[] points;
        public readonly Point3 Normal;
        public readonly float DRatio;
        public Plane PlanaData;

        public TestPolygon(Point3 normal, float d)
        {
            DRatio = d;
            Normal = normal;
        }

        public float Colide(Ray3 ray)
        {
            var mpl = ray.Direction;
            var xyz = ray.Position.Vector;
            var abc = Normal.Vector;
            float upper = DRatio + abc.X * xyz.X + abc.Y * xyz.Y + abc.Z * xyz.Z;
            float lower = abc.X * mpl.X + abc.Y * mpl.Y + abc.Z * mpl.Z;
            if (lower == 0 && upper == 0)
                return 0;
            if (upper > 0 && lower == 0)
                return 0;
            var tRatio = -(upper / lower);
            return tRatio;
            //if (tRatio < 0)
            //    return false;
            //colision = new Point3(xyz + (mpl * (float)tRatio));
            //colision.Color = Color;
            //colision.Distance = Vector3.Distance(colision.Vector, xyz);
            //Vector2[] poins = new Vector2[4];
            //if (Normal.Vector.X != 0)
            //{
            //    poins[0] = new Vector2(colision.Vector.Y, colision.Vector.Z);
            //    poins[1] = new Vector2(points[0].Vector.Y, points[0].Vector.Z);
            //    poins[2] = new Vector2(points[1].Vector.Y, points[1].Vector.Z);
            //    poins[3] = new Vector2(points[2].Vector.Y, points[2].Vector.Z);
            //}
            //else if (Normal.Y != 0)
            //{
            //    poins[0] = new Vector2(colision.Vector.X, colision.Vector.Z);
            //    poins[1] = new Vector2(points[0].Vector.X, points[0].Vector.Z);
            //    poins[2] = new Vector2(points[1].Vector.X, points[1].Vector.Z);
            //    poins[3] = new Vector2(points[2].Vector.X, points[2].Vector.Z);
            //}
            //else
            //{
            //    poins[0] = new Vector2(colision.Vector.X, colision.Vector.Y);
            //    poins[1] = new Vector2(points[0].Vector.X, points[0].Vector.Y);
            //    poins[2] = new Vector2(points[1].Vector.X, points[1].Vector.Y);
            //    poins[3] = new Vector2(points[2].Vector.X, points[2].Vector.Y);
            //}
            //return Utils.PoinInTringle(poins);
        }
    }
}