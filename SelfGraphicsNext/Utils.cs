using SelfGraphicsNext.BaseGraphics;
using SelfGraphicsNext.RayGraphics.Graphics3D.Geometry;
using SelfGraphicsNext.RayGraphics.Graphics3D.Rendering;
using SFML.Graphics;
using System.Runtime.InteropServices;

namespace SelfGraphicsNext
{
    static class Utils
    {
        public static double Pow(this double a, int r = 2) => Math.Pow(a, r);

        public static double Round(this double a, int r = 4) => Math.Round(a, r);

        public static double Sin(this double a) => Math.Sin(a.ToRadians());

        public static double Cos(this double a) => Math.Cos(a.ToRadians());

        public static double Tan(this double a) => Math.Tan(a.ToRadians());

        public static double Sqrt(this double a) => Math.Sqrt(a);

        public static double Abs(this double a) => Math.Abs(a);

        public static double ToRadians(this double a)
        {
            return ToRads(a);
        }
        public static double ToDegrees(this double radians)
        {
            var pre = radians;
            pre *= 180;
            pre /= Math.PI;
            return pre;
        }

        public static double ToRads(double angle)
        {
            var pre = angle;
            pre /= 180;
            pre *= Math.PI;
            return pre;
        }

        public static double AVG(List<double> nums)
        {
            return nums.Select(n => Convert.ToDouble(n)).Sum() / nums.Count;
        }

        public static List<double[]> DoubleRange(double x, double y, double step = 1)
        {
            List<double[]> f = new List<double[]>();
            for (double i = -x; i < x; i++)
            {
                for (double j = -y; j < y; j++)
                {
                    f.Add(new double[] { i, j });
                }
            }

            return f;
        }

        public static List<double[]> DoubleRangePos(double x, double y, int step = 1)
        {
            List<double[]> f = new List<double[]>();
            for (double i = 0; i < x; i++)
            {
                for (double j = 0; j < y; j++)
                {
                    f.Add(new double[] { i, j });
                }
            }

            return f;
        }

        public static List<double> Range(double s, double f, double step = 1)
        {
            List<double> list = new List<double>();
            for (double i = s; i <= f; i += step)
                list.Add(i);
            return list;
        }

        public static List<int[]> DoubleIndexStartFinish(int xs, int xf, int ys, int yf)
        {
            List<int[]> f = new List<int[]>();
            for (int j = ys; j < yf; j++)
            {
                for (int i = xs; i < xf; i++)
                {
                    f.Add(new int[] { i, j });
                }
            }
            return f;
        }

        public static Direction GetDirectionBetween(Point start, Point end)
        {
            var delta = new Point(end.X - start.X, start.Y - end.Y);
            if (delta.X == 0)
            {
                if (delta.Y > 0)
                    return new Direction(270);
                else
                    return new Direction(90);
            }
            var tang = (start - end).Y / (start - end).X;
            var ang = Math.Atan(tang).ToDegrees();
            Direction final = new Direction(ang);
            switch (final.Quater())
            {
                case 1:
                    if (!(delta.X >= 0 || delta.Y < 0))
                        final += 180;
                    break;
                case 2:
                    if (!(delta.X <= 0 || delta.Y <= 0))
                        final += 180;
                    break;
                case 3:
                    if (!(delta.X >= 0 || delta.Y > 0))
                        final += 180;
                    break;
                case 4:
                    if (!(delta.X > 0 || delta.Y >= 0))
                        final += 180;
                    break;
                case 0:
                    if (delta.X < 0)
                        final += 180;
                    break;

            }
            return final;
        }

        public static bool PoinInTringle(List<Point> trn, Point point)
        {
            var pts = new List<Point>();
            pts.Add(point);
            pts.AddRange(trn);
            double a = (pts[1].X - pts[0].X) * (pts[2].Y - pts[1].Y) - (pts[2].X - pts[1].X) * (pts[1].Y - pts[0].Y);
            double b = (pts[2].X - pts[0].X) * (pts[3].Y - pts[2].Y) - (pts[3].X - pts[2].X) * (pts[2].Y - pts[0].Y);
            double c = (pts[3].X - pts[0].X) * (pts[1].Y - pts[3].Y) - (pts[1].X - pts[3].X) * (pts[3].Y - pts[0].Y);
            return (a >= 0 && b >= 0 && c >= 0) || (a <= 0 && b <= 0 && c <= 0);
       
        }

        public static double AngleBetweenVecs(Point3 v1, Point3 v2)
        {
            double upper = v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
            double lower = v1.GetDistanceTo(Point3.Zero) * v2.GetDistanceTo(Point3.Zero);
            if (lower == 0)
                return 0;
            return upper / lower;
        }

        public static Color Mult(this Color color, double k)
        {
            return new Color((byte)(color.R * k), (byte)(color.G * k), (byte)(color.B * k), color.A);
        }

        public static List<T> Merge<T>(List<T[]> muliList)
        {
            var final = new List<T>();
            foreach (var item in muliList)
            {
                final.AddRange(item);
            }
            return final;
        }

        //public static int CircleColision(Circle c1, Circle c2, out List<Point> cols)
        //{
        //    cols = new List<Point>();
        //    if (c1.Radius + c2.Radius > c1.GetDistanceTo(c2) || Math.Abs(c1.Radius - c2.Radius) < c1.GetDistanceTo(c2))
        //        return 0;
        //    if (c1.Radius + c2.Radius == c1.GetDistanceTo(c2))
        //    {
        //        return 1;
        //    }
        //    if (c1.Radius + c2.Radius < c1.GetDistanceTo(c2))
        //    {
        //        double hp = (c1.Radius + c2.Radius + c1.GetDistanceTo(c2)) / 2;
        //        var area = Math.Sqrt(hp * (hp - c1.Radius) * (hp - c1.Radius) * (hp - c1.GetDistanceTo(c2)));
        //        var h = area * 2 / c1.GetDistanceTo(c2);

        //        return 2;
        //    }
        //}
        [DllImport("SelfGToolsLib.dll")]
        public static extern IntPtr CalcColision(IntPtr p, IntPtr r);

        public struct Polygon3C
        {
            public Point3C Normal;
            public Point3C p1;
            public Point3C p2;
            public Point3C p3;
            public double DRatio;
            public Polygon3C(Polygon polygon)
            {
                Normal = new Point3C(polygon.Normal);
                p1 = new Point3C(polygon.points[0]);
                p2 = new Point3C(polygon.points[2]);
                p3 = new Point3C(polygon.points[3]);
                DRatio = polygon.DRatio;

            }
        }

        public struct Point3C
        {
           public double X, Y, Z;
            public Point3C(Point3 p)
            {
                X = p.X;
                Y = p.Y;
                Z = p.Z;
            }
            public Point3 ToDefault()
            {
                return new Point3(X, Y, Z);
            }
        }
        public struct Ray3C
        {
            public Point3C Direction;
            public Point3C Position;
            public Ray3C(Ray3 ray)
            {
                Direction = new Point3C(ray.Direction.GetVector());
                Position = new Point3C(ray.Position);
            }
        }
        public struct ColisionResultC
        {
            public bool isColided;
            public Point3C colPoint;
            public ColisionResult GetOk()
            {
                ColisionResult res = new ColisionResult();
                res.Codiled = isColided;
                if (isColided)
                    res.Colision = colPoint.ToDefault();
                return res;
            }
        }

        public static ColisionResult EasyColisionCalc(Polygon polygon, Ray3 ray)
        {
            var pol = new Polygon3C(polygon);
            var r = new Ray3C(ray);
            IntPtr polInt = Marshal.AllocHGlobal(Marshal.SizeOf(pol));
            Marshal.StructureToPtr(polInt, polInt, false);
            IntPtr rInt = Marshal.AllocHGlobal(Marshal.SizeOf(r));
            Marshal.StructureToPtr(r, rInt, false);
            IntPtr resInt = CalcColision(polInt, rInt);
            ColisionResultC resC = Marshal.PtrToStructure<ColisionResultC>(resInt);
            ColisionResult final = resC.GetOk();
            return final;
        }
    }
}
