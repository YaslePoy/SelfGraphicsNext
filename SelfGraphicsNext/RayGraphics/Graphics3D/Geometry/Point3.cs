using SelfGraphicsNext.BaseGraphics;
using SFML.Graphics;
using System.Numerics;

namespace SelfGraphicsNext.RayGraphics.Graphics3D.Geometry
{
    public struct Point3
    {
        public static Point3 Zero = new Point3(Vector3.Zero);
        public static Point3 One = new Point3(Vector3.One);
        public double Lenght => Vector.Length();
        public Vector3 Vector;
        public double X { get => Vector.X; set => Vector.X = (float)value; }
        public double Y { get => Vector.Y; set => Vector.Y = (float)value; }
        public double Z { get => Vector.Z; set => Vector.Z = (float)value; }
        public double Distance;
        public Color Color = Color.White;
        public Point3()
        {
            Vector = new Vector3();
            Distance = 0;
        }


        public Point3(Point xy)
        {
            Vector = new Vector3((float)xy.X, (float)xy.Y, 0);
            Distance = 0;
        }
        public Point3(double x, double y, double z)
        {
            Vector = new Vector3((float)x, (float)y, (float)z);
            Distance = 0;           
        }

        public Point3(double x, double y, double z, Color color) : this(x, y, z)
        {
            Color = color;
        }
        public Point3(Vector3 xyz)
        {
            Vector = xyz;
            Distance = 0;
        }
        public Point3 Rounded(int k = 3)
        {
            return new Point3(X.Round(k), Y.Round(k), Z.Round(k), Color);
        }

        public void Round(int k = 3)
        {
            X.Round(k);
            Y.Round(k);
            Z.Round(k);
        }
        public static Point3 operator +(Point3 point1, Point3 point2) => new Point3(point1.Vector + point2.Vector) { Color = point1.Color};

        public static Point3 operator -(Point3 point1, Point3 point2) => new Point3(point1.Vector - point2.Vector) { Color = point1.Color };

        public static Point3 operator *(Point3 point1, double k) => new Point3(point1.Vector * (float)k) { Color = point1.Color };


        public static bool operator ==(Point3 point1, Point3 point2) => point1.Vector == point2.Vector;

        public static bool operator !=(Point3 point1, Point3 point2) => point1.Vector != point2.Vector;

        public void SetDistanceTo(Point3 p) => Distance = (Vector - p.Vector).Length();

        public double GetDistanceTo(Point3 p) => (Vector - p.Vector).Length();

        public override string ToString()
        {
            return $"[{X.Round()}:{Y.Round()}:{Z.Round()}]";
        }

        public Point3 Normalised()
        {
            var len = Vector.Length();
            return this * (1 / len);
        }

        public double ScalarMul(Point3 vec)
        {
            return X * vec.X + Y * vec.Y + Z * vec.Z;
        }
    }
}
