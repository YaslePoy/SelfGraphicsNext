using SelfGraphicsNext.RayGraphics.Graphics2D;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfGraphicsNext.BaseGraphics
{
    public class Reclangle : Point
    {
        public double XSide, YSide;
        public Reclangle()
        {
            X = 0;
            Y = 0;
            XSide = YSide = 10;
        }
        public Reclangle(double x, double y, double width, double height)
        {
            X = x; 
            Y = y;
            XSide = width;
            YSide = height;
        }

        public override void Draw(RenderWindow win)
        {
            List<Vertex> verts = new List<Vertex>();
            verts.Add(new Vertex(base.getVec2f(), Color));
            verts.Add(new Vertex(base.getVec2f() + new Vector2f((float)XSide, 0), Color));
            verts.Add(new Vertex(base.getVec2f() + new Vector2f((float)XSide, (float)YSide), Color));
            verts.Add(new Vertex(base.getVec2f() + new Vector2f(0, (float)YSide), Color));
            //verts.Add(new Vertex(base.getVec2f(), Color));
            win.Draw(verts.ToArray(), PrimitiveType.Quads);
        }
        public override Point? Collide(Ray ray)
        {
            List<Line> lines = new List<Line>();
            Point start = new Point(X, Y);
            lines.Add(new Line(start, start + new Point(XSide, 0)));
            lines.Add(new Line(start + new Point(XSide, 0), start + new Point(XSide, YSide)));
            lines.Add(new Line(start + new Point(XSide, YSide), start + new Point(0, YSide)));
            lines.Add(new Line(start + new Point(0, YSide), start));
            var collides = lines.Select(i => i.Collide(ray)).ToList();
            collides = collides.Where(i => !(i is null)).ToList();
            var notNullCollides = new List<Point>();
            foreach (var collide in collides)
            {
                var direct = Utils.GetDirectionBetween(ray.Position, collide).Rounded;
                if (direct == ray.Direction.Rounded) notNullCollides.Add(collide);
            }
            notNullCollides.ForEach(i => i.SetDistanceTo(ray.Position));
            notNullCollides.ForEach(i => i.Color = Color);
            return notNullCollides.MinBy(i => i.Distance);
        }
    }
}
