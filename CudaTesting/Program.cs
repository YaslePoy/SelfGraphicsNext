using ManagedCuda;
using ManagedCuda.BasicTypes;
using ManagedCuda.VectorTypes;
using SelfGraphicsNext.BaseGraphics;
using SelfGraphicsNext.RayGraphics.Graphics3D.Geometry;
using System.Xml.Linq;

namespace CudaTesting
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int mode = 2;
            if(mode == 1)
            {
            CudaContext ctx = new CudaContext(0);
            CudaKernel kernel = ctx.LoadKernel("code.ptx", "func");
            kernel.GridDimensions = new ManagedCuda.VectorTypes.dim3(3, 3);
            kernel.BlockDimensions = new ManagedCuda.VectorTypes.dim3(3, 3);
            CudaDeviceVariable<double3> out_cuda = new CudaDeviceVariable<double3>(81);
            kernel.Run(out_cuda.DevicePointer, 90d, new double2(130, 70));
            double3[] host = out_cuda;
            LogCuda(host.ToList());
            }
            else if(mode == 2)
            {
                var realDir = new Direction3(-45, 45);
                realDir.AddHorisontal(130);
                realDir.AddVertical(70);
                var vec = realDir.GetVector();
            }
            else if (mode == 3)
            {
                var realDirs = new Direction3().GetDirectionsByResolution(9, 9, 90, 90);
                List<Point3> pts = new List<Point3>();

                for (int i = 0; i < realDirs.GetLength(1); i++)
                {
                    for (int j = 0; j < realDirs.GetLength(0); j++)
                    {
                        pts.Add(realDirs[i, j].GetVector());
                    }
                }
                LogCuda(pts);
            }


            
        }
        public static void LogCuda<T>(List<T> vals)
        {
            var lists = vals.Chunk((int)Math.Sqrt(vals.Count));
            foreach (var item in lists)
            {
                Console.WriteLine(string.Join(", ", item.ToList().Select(i => i.ToString())));
                Console.WriteLine("--====--");
            }

        }
    }
}