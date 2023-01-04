using ManagedCuda;
using ManagedCuda.VectorTypes;
using SelfGraphicsNext.RayGraphics.Graphics3D.Rendering;
using System.Globalization;

namespace SelfGraphicsNext.RayGraphics.Graphics3D.Geometry
{
    public class Scene
    {
        public CudaKernel cudaDevice;
        public Point3 Light;
        public List<PolygonGroup> Objects;
        public void LoadModel()
        {
            var totalCount = Objects.Sum(i => i.Surface.Count);
            cudaDevice.SetConstantVariable("CountOfTrns", totalCount);
            float3[] PolPoint1 = new float3[totalCount];
            float3[] PolPoint2 = new float3[totalCount];
            float3[] PolPoint3 = new float3[totalCount];
            float3[] normals = new float3[totalCount];
            float3[] colorsOfTrns = new float3[totalCount];
            float[] dratios = new float[totalCount];
            int index = 0;
            for (int i = 0; i < Objects.Count; i++)
            {
                var group = Objects[i];
                for (int j = 0; j < group.Surface.Count; j++)
                {
                    var polygon = group.Surface[j];
                    PolPoint1[index] = polygon.points[0].GetFloat3();
                    PolPoint2[index] = polygon.points[1].GetFloat3();
                    PolPoint3[index] = polygon.points[2].GetFloat3();
                    dratios[index] = (float)polygon.PlanaData.D;
                    normals[index] = polygon.Normal.GetFloat3();
                    colorsOfTrns[index] = new float3(polygon.Color.R, polygon.Color.G, polygon.Color.B);
                    index++;
                }
            }
            cudaDevice.SetConstantVariable("trPoint1", PolPoint1);
            cudaDevice.SetConstantVariable("trPoint2", PolPoint2);
            cudaDevice.SetConstantVariable("trPoint3", PolPoint3);
            cudaDevice.SetConstantVariable("normals", normals);
            cudaDevice.SetConstantVariable("dRatios", dratios);
            cudaDevice.SetConstantVariable("colors", colorsOfTrns);
        }
        public List<PolyginCUDA> GetPolyons()
        {
            List<PolyginCUDA> plgns = new List<PolyginCUDA>();
            for (int i = 0; i < Objects.Count; i++)
            {
                var group = Objects[i];
                for (int j = 0; j < group.Surface.Count; j++)
                {
                    plgns.Add(Objects[i].Surface[j].GetCUDA());
                }
            }
            return plgns;
        }
        public (int total, float3[] pt1, float3[] pt2, float3[] pt3, float3[] nor, float[] drs, float3[] col) GetLoadData()
        {
            var totalCount = Objects.Sum(i => i.Surface.Count);
            
            float3[] PolPoint1 = new float3[totalCount];
            float3[] PolPoint2 = new float3[totalCount];
            float3[] PolPoint3 = new float3[totalCount];
            float3[] normals = new float3[totalCount];
            float3[] colorsOfTrns = new float3[totalCount];
            float[] dratios = new float[totalCount];
            int index = 0;
            for (int i = 0; i < Objects.Count; i++)
            {
                var group = Objects[i];
                for (int j = 0; j < group.Surface.Count; j++)
                {
                    var polygon = group.Surface[j];
                    PolPoint1[index] = polygon.points[0].GetFloat3();
                    PolPoint2[index] = polygon.points[1].GetFloat3();
                    PolPoint3[index] = polygon.points[2].GetFloat3();
                    dratios[index] = (float)polygon.PlanaData.D;
                    normals[index] = polygon.Normal.GetFloat3();
                    colorsOfTrns[index] = new float3(polygon.Color.R, polygon.Color.G, polygon.Color.B);
                    index++;
                }
            }
            return (totalCount, PolPoint1, PolPoint2, PolPoint3, normals, dratios, colorsOfTrns);
        }
        public Scene(List<PolygonGroup> objects)
        {
            Objects = objects ?? throw new ArgumentNullException(nameof(objects));
        }
        public Scene(bool useCUDA=false)
        {
            Objects = new List<PolygonGroup>();
            if (useCUDA)
            {
                Utils.UpdateCudaContex();
                cudaDevice = Utils.ctx.LoadKernel("rayWork.ptx", "resultPixel");
                cudaDevice.BlockDimensions = new dim3(16, 16);
            }
        }

        public void Clear() => Objects.Clear();

        public static Scene LoadSceneObj(string path, bool cuda=false)
        {
            Scene scene = new Scene(cuda);
            var model = File.ReadAllLines(path).ToList();
            var vertexs = new List<Point3>();
            var normals = new List<Point3>();
            {
                model.RemoveAll(i => i.StartsWith("vt"));
                foreach (var line in model)
                {
                    var coms = line.Split(' ');
                    if (coms[0] == "v")
                    {
                        double x = double.Parse(coms[1], CultureInfo.InvariantCulture);
                        double y = double.Parse(coms[2], CultureInfo.InvariantCulture);
                        double z = double.Parse(coms[3], CultureInfo.InvariantCulture);
                        vertexs.Add(new Point3(x, y, z));
                    }
                    if (coms[0] == "vn")
                    {
                        double x = double.Parse(coms[1], CultureInfo.InvariantCulture);
                        double y = double.Parse(coms[2], CultureInfo.InvariantCulture);
                        double z = double.Parse(coms[3], CultureInfo.InvariantCulture);
                        normals.Add(new Point3(x, y, z));
                    }
                }
                model.RemoveAll(i => i.StartsWith("v") || i.StartsWith("vn"));
                foreach (var line in model)
                {
                    var coms = line.Split(' ').ToList();
                    if (coms[0] == "o")
                    {
                        scene.Objects.Add( new PolygonGroup() { Name = coms[1]});
                    }
                    if (coms[0] == "usemtl")
                        scene.Objects.Last().Color = MtlReader.GetColor(path.Split(".")[0] + ".mtl", coms[1]);
                    if(coms[0] == "f")
                    {
                        coms.RemoveAt(0);
                        List<Point3> points = new List<Point3>();
                        foreach(var i in coms)
                        {
                            var pol = i.Split('/');
                            points.Add(vertexs[int.Parse(pol[0]) - 1]);
                        }
                        Polygon localPol = new Polygon(points, normals[int.Parse(coms[0].Split('/').Last()) - 1]) { Color = scene.Objects.Last().Color};
                        scene.Objects.Last().Surface.Add(localPol);
                    }

                }
            }
            return scene;
        }

    }
}
