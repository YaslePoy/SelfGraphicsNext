using SelfGraphicsNext.RayGraphics.Graphics3D.Rendering;
using SFML.Graphics;

namespace SelfGraphicsNext.RayGraphics.Graphics3D.Geometry
{
    public class PolygonGroup
    {
        public Color Color { get; set; }

        public List<Polygon> Surface;

        public string Name;

        public PolygonGroup(List<Polygon> surface)
        {
            Surface = surface;
        }

        public PolygonGroup() => Surface = new List<Polygon>();
        public ColisionResult Colide(Ray3 ray)
        {
            Point3 colision = new Point3();
            ColisionResult colResult = new ColisionResult();
            List<ColisionResult> results = new List<ColisionResult>();
            //var tasks = Surface.Select(Surf => Task.Run(() => CollideByRes(ray, Surf))).ToList();
            //while (tasks.Any(i => !i.IsCompleted))
            //    continue;
            //results = tasks.Where(i => !(i.Result.Collision is null)).Select(i => i.Result.Collision).ToList();
            //Dictionary<Point3, Polygon> ppp = new Dictionary<Point3, Polygon>();
            for (int i = 0; i < Surface.Count; i++)
            {
                Polygon polygon = Surface[i];
                if (polygon.Colide(ray, out Point3 col))
                {
                    results.Add(new ColisionResult() { ColidedPoligon = polygon, Colision = col});
                    //ppp.Add(col, polygon);
                }
            }                
            if (results.Count > 0)
            {
                colResult = results.MinBy(i => i.Colision.Distance);
                //colResult.ColidedPoligon = ppp[colision];
                colResult.GroupName = Name;
                colResult.RaySource = ray.Position;
                colResult.Color = Color;
                colResult.Codiled = true;
                return colResult;
            }
            colResult.Codiled = false;
            return colResult;
        }
       
    }
}
