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
        public Polygon ColidedPoligon;
        public Color Color;
        public Point3 Colision;
        public Point3 RaySource;
        public string GroupName;

        public bool Colided;
        public ColisionResult(Polygon colidedPoligon, Color color, Point3 colision, Point3 raySource)
        {
            ColidedPoligon = colidedPoligon;
            Color = color;
            Colision = colision;
            RaySource = raySource;
            //GroupName = "NoName";
            //Colided = false;
        }

        public ColisionResult()
        {
            ColidedPoligon = new Polygon(new List<Point3>(), new Point3(0, 0, 1));
            Color = Color.White;
            Colision = new Point3(0, 0, 0);
            RaySource = new Point3(0, 0, 0);
            //GroupName = "NoName";
            //Colided = false;
        }
    }
}
