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
        public List<PolygonCUDA> GetPolyons()
        {
            List<PolygonCUDA> plgns = new List<PolygonCUDA>();
            for (int i = 0; i < Objects.Count; i++)
            {
                var group = Objects[i];
                for (int j = 0; j < group.Surface.Count; j++)
                {
                    PolygonCUDA pg = Objects[i].Surface[j].GetCUDA();
                    pg.objId = i;
                    plgns.Add(pg);
                }
            }
            return plgns;
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
                cudaDevice.BlockDimensions = new dim3(8, 8);
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
