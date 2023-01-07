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
            Console.WriteLine(("test str 1", "test str 2"));
            int N = 5;
            Vector3[] rays = new Vector3[N];
            Vector3[] normals = new Vector3[N];
            for (int i = 0; i < N; i++)
            {
                rays[i] = Vector3.Normalize(new Vector3(localRand(), localRand(), localRand()));
                normals[i] = Vector3.Normalize(new Vector3(localRand(), localRand(), localRand()));
            }
            var trueReflection = new List<Vector3>();
            var myReflection = new List<Vector3>();

            for (int i = 0; i < N; i++)
            {
                trueReflection.Add(Vector3.Reflect(rays[i], normals[i]));
                myReflection.Add(Reflect(rays[i], normals[i]));
            }
            for (int i = 0; i < N; i++)
            {
                Console.WriteLine($"True: {trueReflection[i]}, My: {myReflection[i]}");
            }

        }

        public static Vector3 Reflect(Vector3 ray, Vector3 normal)
        {
            float u = MathF.Abs(Vector3.Dot(ray, normal));
            var neVec = ray + normal * (u * 2);
            return neVec;
        }
    }
}