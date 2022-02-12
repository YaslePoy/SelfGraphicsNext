using SelfGraphicsNext.RayGraphics.Graphics2D;
using SFML.Graphics;
using SFML.System;

namespace SelfGraphicsNext.BaseGraphics
{
    public class Point
    {
        public Vector2f getVec2f()
        {
            return new Vector2f((float)X, (float)Y);
        }
        public double Distance { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public Color Color { get; set; } = Color.Yellow;

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Point()
        {
            X = Y = 0;
        }

        public static Point Zero => new Point(0, 0);

        public static Point One => new Point(1, 1);

        public static bool operator ==(Point p1, Point p2)
        {
            if (p1 is null || p2 is null)
                return false;
            return p1.X == p2.X && p1.Y == p2.Y;
        }

        public static bool operator !=(Point p1, Point p2)
        {
            if (p1 is null || p2 is null)
                return false;
            return p1.X != p2.X || p1.Y != p2.Y;
        }

        public static Point operator +(Point p1, Point p2)
        {

            return new Point(p1.X + p2.X, p1.Y + p2.Y) { Color = p1.Color };
        }

        public static Point operator -(Point p1, Point p2)
        {
            return new Point(p1.X - p2.X, p1.Y - p2.Y) { Color = p1.Color };
        }

        public static Point operator *(Point p, double k)
        {
            p.X *= k;
            p.Y *= k;
            return p;
        }

        public static Point operator /(Point p, double k)
        {
            p.X /= k;
            p.Y /= k;
            return p;
        }

        public double GetDistanceTo(Point aim)
        {
            this.Distance = ((aim.X - this.X).Pow() + (aim.Y - this.Y).Pow()).Sqrt();
            return this.Distance;
        }

        public void SetDistanceTo(Point aim)
        {
            this.Distance = ((aim.X - this.X).Pow() + (aim.Y - this.Y).Pow()).Sqrt();
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (!(obj is Point)) return false;
            Point p = (Point)obj;
            return this == p;
        }
        public Point Rounded(int a = 4)
        {
            return new Point(X.Round(a), Y.Round(a)) { Color = Color, Distance = Distance };
        }

        public void Round(int a = 4)
        {
            X = X.Round(a);
            Y = Y.Round(a);
        }

        public override string ToString()
        {
           
            return "{" + X.Round(4) + " : " + Y.Round(4) + "}";
        }

        public virtual Point? Collide(Ray ray)
        {
            if (ray.GetPointByDistance(GetDistanceTo(ray.Position)).Rounded() == this.Rounded()) return this;
            return null;
        }

        public virtual void Draw(RenderWindow win)
        {
            win.Draw(new Vertex[1] { new Vertex(new((float)X, (float)Y), Color), }, PrimitiveType.Points);
        }
    }
}
