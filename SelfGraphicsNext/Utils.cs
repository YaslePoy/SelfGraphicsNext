using SelfGraphicsNext.BaseGraphics;
using SelfGraphicsNext.RayGraphics.Graphics3D.Geometry;
using SelfGraphicsNext.RayGraphics.Graphics3D.Rendering;
using SFML.Graphics;
using System.Numerics;
using System.Runtime.InteropServices;
using ManagedCuda;

namespace SelfGraphicsNext
{
    static class Utils
    {
        public const double ToRad = 0.017453292519943295769236907684886;
        public const double ToDeg = 57.295779513082320876798154814105;
        public const float ToRadF = 0.017453292519943295769236907684886f;
        public const float ToDegF = 57.295779513082320876798154814105f;
        public static double Pow(this double a, int r = 2) => Math.Pow(a, r);

        public static double Round(this double a, int r = 4) => Math.Round(a, r);

        public static double Sin(this double a) => Math.Sin(a * ToRad);

        public static double Cos(this double a) => Math.Cos(a * ToRad);

        public static double Tan(this double a) => Math.Tan(a * ToRad);

        public static double Sqrt(this double a) => Math.Sqrt(a);

        public static double Abs(this double a) => Math.Abs(a);

        public static double ToRadians(this double a)
        {
            return a * ToRad;
        }
        public static double ToDegrees(this double radians)
        {
            return radians * ToDeg;
        }

        public static double ToRads(double angle)
        {

            return angle * ToRad;
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
        public static bool PoinInTringle(Vector2[] pts)
        {
            float a = (pts[1].X - pts[0].X) * (pts[2].Y - pts[1].Y) - (pts[2].X - pts[1].X) * (pts[1].Y - pts[0].Y);
            float b = (pts[2].X - pts[0].X) * (pts[3].Y - pts[2].Y) - (pts[3].X - pts[2].X) * (pts[2].Y - pts[0].Y);
            float c = (pts[3].X - pts[0].X) * (pts[1].Y - pts[3].Y) - (pts[1].X - pts[3].X) * (pts[3].Y - pts[0].Y);
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
        public static CudaContext ctx;
        public static void UpdateCudaContex()
        {
            ctx = new CudaContext(0);
        }
    }
}
