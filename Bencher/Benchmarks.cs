using BenchmarkDotNet;
using System.Numerics;
using BenchmarkDotNet.Attributes;
using System;

namespace Bencher
{
    public static class Utils
    {
        public static double Pow(this double a, int r = 2) => Math.Pow(a, r);
        public static double Sqrt(this double a) => Math.Sqrt(a);
    }
    public class Point3
    {
        public double Lenght => (X.Pow() + Y.Pow() + Z.Pow()).Sqrt();
        double X, Y, Z;
        public Point3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
    public class Benchmarks
    {
        public Vector3 DotNetVec;
        public Point3 MyVec;
        public Benchmarks()
        {
            DotNetVec = new Vector3(1, 2, 3);
            MyVec = new Point3(1, 2, 3);
        }
        [Benchmark(Baseline = true)]
        public void DotNetLen()
        {
            var len = DotNetVec.Length();
        }

        [Benchmark]
        public void MyLen()
        {
            var len = MyVec.Lenght;
        }
    }
}
