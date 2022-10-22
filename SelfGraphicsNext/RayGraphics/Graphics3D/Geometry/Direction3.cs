using SelfGraphicsNext.BaseGraphics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace SelfGraphicsNext.RayGraphics.Graphics3D.Geometry
{
    public struct Direction3 
    {
        public Direction Horisontal;
        public Direction Vertical;

        public Direction3(double horisontal, double vertical)
        {
            Horisontal = new Direction(horisontal);
            Vertical = new Direction(vertical);
        }

        public Direction3()
        {
            Horisontal = new Direction(0);
            Vertical = new Direction(0);
        }

        public Point3 GetVector()
        {
            var vertRatio = Math.Cos(Vertical.AngleGrads * Utils.ToRad);
            return new Point3(Math.Cos(Horisontal.AngleGrads * Utils.ToRad) * vertRatio, Math.Sin(Horisontal.AngleGrads * Utils.ToRad) * vertRatio, Math.Sin(Vertical.AngleGrads * Utils.ToRad));
        }

        public void SetDirection(Point3 vector)
        {
            var vec = vector.Normalised();
            Horisontal = new Direction(Math.Atan(vec.Vector.Y / vec.Vector.X).ToDegrees());
            if (vec.Vector.X > 0 && vec.Vector.Y > 0)
                Vertical = new Direction(Math.Asin(vec.Vector.Z).ToDegrees());
            else
            {
                double finalH = 0;
                var preH = new Direction(Math.Asin(vec.Vector.Z).ToDegrees()).AngleGrads;
                if (preH is > 0 and < 180)
                    finalH = 90 + (90 - preH);
                else
                    finalH = 180 + (360 - preH);
                Vertical = new Direction(finalH);

            }
        }

        public override string ToString()
        {
            return $"H:{Horisontal.ToString()}; V:{Vertical.ToString()}";
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (!(obj is Direction3))
                return false;
            var dir = (Direction3)obj;
            return Horisontal == dir.Horisontal && Vertical == dir.Vertical;
        }

        public void Round(int k = 2)
        {
            Horisontal = new Direction(Horisontal.AngleGrads.Round(k));
            Vertical = new Direction(Vertical.AngleGrads.Round(k));
        }

        public Direction3 Rounded(int k = 2)
        {
            return new Direction3(Horisontal.Rounded.AngleGrads, Vertical.Rounded.AngleGrads);
        }

        public static Direction3 operator +(Direction3 d1, Direction3 d2)
        {
            return new Direction3(d1.Horisontal.AngleGrads + d2.Horisontal.AngleGrads, d1.Vertical.AngleGrads + d2.Vertical.AngleGrads);
        }
        public static Direction3 operator -(Direction3 d1, Direction3 d2)
        {
            return new Direction3(d1.Horisontal.AngleGrads - d2.Horisontal.AngleGrads, d1.Vertical.AngleGrads - d2.Vertical.AngleGrads);
        }
        public void AddHorisontal(double angle)
        {
            Horisontal.AddDegrees(angle);
        }
        public void AddVertical(double angle)
        {
            Vertical.AddDegrees(angle);
        }
        public Direction3[,] GetDirectionsByResolution(int xRes, int yRes, double xFow, double yFow)
        {
            Direction3[,] result = new Direction3[xRes, yRes];
            var hors = Horisontal.GetDirectionsByCount(xRes, xFow);
            var vers = Vertical.GetDirectionsByCount(yRes, yFow);
            vers.Reverse();
            for (int i = 0; i < yRes; i++)
                for (int j = 0; j < xRes; j++)
                    result[j, i]=new Direction3(hors[j].AngleGrads, vers[i].AngleGrads);
            
            return result;
        }
    }
}
