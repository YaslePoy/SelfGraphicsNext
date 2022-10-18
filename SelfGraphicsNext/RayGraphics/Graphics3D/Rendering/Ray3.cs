using SelfGraphicsNext.BaseGraphics;
using SelfGraphicsNext.RayGraphics.Graphics3D.Geometry;
using System.Numerics;

namespace SelfGraphicsNext.RayGraphics.Graphics3D.Rendering
{
    public class Ray3
    {
        public Vector3 Direction;
        public Point3 Position;

        public Point ImagePosition;
        public Ray3(Direction3 direction, Point3 position)
        {
            Direction = direction.GetVector().Vector;
            Position = position;
        }
        public Ray3(Vector3 direction, Point3 position)
        {
            Direction = direction;
            Position = position;
        }
        public Ray3(Point3 position)
        {
            Position = position;
            Direction = Vector3.UnitX;
        }

        public Ray3()
        {
            Position = new Point3(0, 0, 0);
            Direction = Vector3.UnitX;

        }

        public ColisionResult CollideInScene(Scene scene)
        {
            return CollideInSceneIns(scene, null);
        }

        public ColisionResult CollideInSceneIns(Scene scene, Polygon pol)
        {
            List<ColisionResult> results = new List<ColisionResult>();
            for (int i = 0; i < scene.Objects.Count; i++)
            {
                PolygonGroup groups = scene.Objects[i];
                var res = groups.ColideIns(this, pol);
                if (res.Colided)
                    results.Add(res);
            }
            if (results.Count > 0)
            {
                var current = results.MinBy(i => i.Colision.Distance);
                return current;
            }
            return new ColisionResult() { Colided = false };
        }
        public override string ToString()
        {
            return $"Ray 3d[Position : {Position.ToString()}, Direction : {Direction.ToString()}]";
        }
    }
}
