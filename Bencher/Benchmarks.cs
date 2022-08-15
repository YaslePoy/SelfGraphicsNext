﻿using BenchmarkDotNet.Attributes;
using System;
using System.Drawing;
using System.Numerics;

namespace Bencher
{
    public class Data
    {
        public int Prop { get; set; }
        public int Field;
    }
    public static class Utils
    {
        public static double Round(this double a, int r = 4) => Math.Round(a, r);

        public static double Pow(this double a, int r = 2) => Math.Pow(a, r);
        public static double Sqrt(this double a) => Math.Sqrt(a);
    }
    public class Point3
    {
        public static Point3 Zero = new Point3(0, 0, 0);
        public static Point3 One = new Point3(1, 1, 1);
        public double Lenght => data.Length();
        Vector3 data;
        public double X { get => data.X; set => data.X = (float)value; }
        public double Y { get => data.Y; set => data.Y = (float)value; }
        public double Z { get => data.Z; set => data.Z = (float)value; }
        public double Distance { get; set; }
        public Color Color { get; set; } = Color.White;
        public Point3()
        {
            data = new Vector3();
        }


        public Point3(Point xy)
        {
            data = new Vector3((float)xy.X, (float)xy.Y, 0);
        }
        public Point3(double x, double y, double z)
        {
            data = new Vector3((float)x, (float)y, (float)z);
        }

        public Point3(double x, double y, double z, Color color) : this(x, y, z)
        {
            Color = color;
        }
        public Point3(Vector3 xyz)
        {
            data = xyz;
        }
        public Point3 Rounded(int k = 4)
        {
            return new Point3(X.Round(k), Y.Round(k), Z.Round(k), Color);
        }

        public void Round(int k = 4)
        {
            X.Round(k);
            Y.Round(k);
            Z.Round(k);
        }
        public static Point3 operator +(Point3 point1, Point3 point2) => new Point3(point1.data + point2.data) { Color = point1.Color };

        public static Point3 operator -(Point3 point1, Point3 point2) => new Point3(point1.data - point2.data) { Color = point1.Color };

        public static Point3 operator *(Point3 point1, double k) => new Point3(point1.data * (float)k) { Color = point1.Color };

        public static bool operator ==(Point3 point1, Point3 point2) => point1.data == point2.data;

        public static bool operator !=(Point3 point1, Point3 point2) => point1.data != point2.data;

        public void SetDistanceTo(Point3 p) => Distance = (this - p).Lenght;

        public double GetDistanceTo(Point3 p) => (this - p).Lenght;

        public override string ToString()
        {
            return $"[{X.Round()}:{Y.Round()}:{Z.Round()}]";
        }

        public override bool Equals(object? obj)
        {
            if (obj == null && this is null)
                return true;
            if (obj == null)
                return false;
            if (!(obj is Point3))
                return false;
            Point3 p = (Point3)obj;
            return data == p.data;
        }
        public Point3 Normalised()
        {
            var len = this.GetDistanceTo(Point3.Zero);
            return this * (1 / len);
        }

        public double ScalarMul(Point3 vec)
        {
            return X * vec.X + Y * vec.Y + Z * vec.Z;
        }
    }
    public class Benchmarks
    {
        //public Vector3 DotNetVecA;
        //public Vector3 DotNetVecB;
        //public Point3 MyVecA;
        //public Point3 MyVecB;
        public Data a;
        public Data b;

        public Benchmarks()
        {
            //DotNetVecA = new Vector3(1, 2, 3);
            //MyVecA = new Point3(1, 2, 3);
            //DotNetVecB = new Vector3(4, 5, 6);
            //MyVecB = new Point3(4, 5, 6);
             a = new Data();
            b = new Data();
        }
        [Benchmark]
        public void Property()
        {
            a.Prop = 5;
            a.Prop = 6;
            a.Prop = 7;
        }

        [Benchmark]
        public void Field()
        {
            b.Field = 5;
            b.Field = 6;
            b.Field = 7;

        }
    }
}
