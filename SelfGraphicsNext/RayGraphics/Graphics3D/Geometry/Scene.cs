using ManagedCuda;
using ManagedCuda.VectorTypes;
using SelfGraphicsNext.RayGraphics.Graphics3D.Rendering;
using SFML.Graphics;
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
        public Scene(bool useCUDA = false)
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

        public static Scene LoadSceneObj(string path, bool cuda = false)
        {
            Scene scene = new Scene(cuda);
            MtlFile mtl = new MtlFile(path.Split(".")[0] + ".mtl");

            var model = File.ReadAllLines(path).ToList();
            var points = new List<Point3>();
            var normals = new List<Point3>();

            PolygonGroup currentObj = new PolygonGroup();
            Color currentColor = new Color();
            model.RemoveAll(i => i.StartsWith("vt"));
            foreach (var line in model)
            {
                var coms = line.Split(' ');
                switch (coms[0])
                {
                    case "v":
                        points.Add(Point3.ParceVector(coms[1], coms[2], coms[3]));
                        break;
                    case "vn":
                        normals.Add(Point3.ParceVector(coms[1], coms[2], coms[3]));
                        break;
                    case "o":
                        currentObj = new PolygonGroup() { Name = coms[1] };
                        scene.Objects.Add(currentObj);
                        break;
                    case "usemtl":
                        currentColor = mtl[coms[1]];
                        break;
                    case "f":
                        Point3 normal = new Point3();
                        List<Point3> polPoints = new List<Point3>();
                        for (int i = 1; i < 4; i++)
                        {
                            var parts = coms[i].Split("/").Select(i => Int32.Parse(i)).ToList();
                            polPoints.Add(points[parts[0] - 1]);
                            normal = normals[parts[2] - 1];
                        }
                        Polygon localPolygon = new Polygon(polPoints, normal) { Color = currentColor };
                        currentObj.Surface.Add(localPolygon);
                        break;
                }
            }
            return scene;
        }

    }
}
