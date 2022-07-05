using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfGraphicsNext.RayGraphics.Graphics3D.Geometry
{
    public class ColisionResult
    {
        public Polygon ColidedPoligon { get; set; }
        public Color Color { get; set; }
        public Point3 Colision { get; set; }
        public Point3 RaySource { get; set; }
        public string GroupName { get; set; }

        public bool Codiled { get; set; }
        public ColisionResult(Polygon colidedPoligon, Color color, Point3 colision, Point3 raySource)
        {
            ColidedPoligon = colidedPoligon;
            Color = color;
            Colision = colision;
            RaySource = raySource;
        }

        public ColisionResult()
        {
            ColidedPoligon = new Polygon(new List<Point3>(), new Point3(0, 0, 1));
            Color = Color.White;
            Colision = new Point3(0, 0, 0);
            RaySource = new Point3(0, 0, 0);
        }
    }
}
