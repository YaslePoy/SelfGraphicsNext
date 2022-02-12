using SelfGraphicsNext.RayGraphics.Graphics3D.Rendering;

namespace SelfGraphicsNext.RayGraphics.Graphics3D.Geometry
{
    public class PolygonGroup : Colideble
    {
        public List<Polygon> Surface;

        public string Name;

        public PolygonGroup(List<Polygon> surface)
        {
            Surface = surface;
        }

        public PolygonGroup() => Surface = new List<Polygon>();
        public override bool Colide(Ray3 ray, out Point3 colision)
        {
            List<Point3> results = new List<Point3>();
            //var tasks = Surface.Select(Surf => Task.Run(() => CollideByRes(ray, Surf))).ToList();
            //while (tasks.Any(i => !i.IsCompleted))
            //    continue;
            //results = tasks.Where(i => !(i.Result.Collision is null)).Select(i => i.Result.Collision).ToList();
            foreach (Polygon polygon in Surface)
                if (polygon.Colide(ray, out Point3 col))
                    results.Add(col);
            if (results.Count > 0)
            {
                colision = results.MinBy(i => i.Distance);
                return true;
            }
            colision = null;
            return false;
        }

        public CollisionResult CollideByRes(Ray3 ray, Polygon polygon)
        {
            CollisionResult result = new CollisionResult();
            result.IsColided = polygon.Colide(ray, out Point3 col);
            result.Collision = ray.Aim;
            return result;
        }
    }
    public class CollisionResult
    {
        public bool IsColided;

        public Point3 Collision;
    }
}
