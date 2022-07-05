using SelfGraphicsNext.BaseGraphics;
using SelfGraphicsNext.RayGraphics.Graphics3D.Geometry;
using SFML.Graphics;

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
            for (double i = 0; i <= f; i += step)
                list.Add(i);
            return list;
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
            List<double> results = new List<double>();
            var delta = trn[1] - trn.First();
            var pointDelta = point - trn.First();
            results.Add(delta.X * pointDelta.Y - delta.Y * pointDelta.X);
            delta = trn.Last() - trn[1];
            pointDelta = point - trn[1];
            results.Add(delta.X * pointDelta.Y - delta.Y * pointDelta.X);
            delta = trn.First() - trn.Last();
            pointDelta = point - trn[2];
            results.Add(delta.X * pointDelta.Y - delta.Y * pointDelta.X);
            return !(results.Any(x => x > 0) && results.Any(x => x < 0));
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
    }
}
