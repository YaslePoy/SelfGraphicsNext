using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using SelfGraphicsNext.RayGraphics.Graphics3D.Geometry;
using System.Numerics;
using System.Security.Cryptography;
//namespace Bench
//{
//    public class Bench
//    {
//        Vector3 vecDotNet;
//        Point3 VecMy;
//        public Bench()
//        {
//            vecDotNet = new Vector3(1, 2, 3);
//            VecMy = new Point3(1, 2, 3);
//        }
//        [Benchmark]
//        public double DotNetLen() => vecDotNet.Length();
//        [Benchmark]
//        public double MyLen() => VecMy.Lenght;
//    }
//    internal class Program
//    {
//        static void Main(string[] args)
//        {
//            var sum = BenchmarkRunner.Run<Bench>();
//        }
//    }
//}
namespace MyBenchmarks
{
    public class Md5VsSha256
    {
        private const int N = 10000;
        private readonly byte[] data;

        private readonly SHA256 sha256 = SHA256.Create();
        private readonly MD5 md5 = MD5.Create();

        public Md5VsSha256()
        {
            data = new byte[N];
            new Random(42).NextBytes(data);
        }

        [Benchmark]
        public byte[] Sha256() => sha256.ComputeHash(data);

        [Benchmark]
        public byte[] Md5() => md5.ComputeHash(data);
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Md5VsSha256>();
        }
    }
}