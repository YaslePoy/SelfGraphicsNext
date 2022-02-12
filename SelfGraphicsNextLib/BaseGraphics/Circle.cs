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
    public class Circle : Point
    {

        public double Radius { get; set; }
        public Circle(double x, double y, double radius)
        {
            X = x;
            Y = y;
            this.Radius = radius;
        }

        public override void Draw(RenderWindow win)
        {
            CircleShape circle = new CircleShape((float)Radius);
            circle.Position = base.getVec2f() - new Vector2f((float)Radius, (float)Radius);
            circle.OutlineColor = Color;
            circle.OutlineThickness = 1;
            circle.FillColor = Color.Transparent;
            win.Draw(circle);
        }

        public override Point? Collide(Ray ray)
        {
            var k = ray.Direction.Tan;
            var w = X - ray.Position.X;
            var h = Y - ray.Position.Y;
            var aRatio = k + 1;
            var bRatio = 2 * (w + h * k);
            var cRatio = w.Pow() + h.Pow() - Radius.Pow();
            var diskr = bRatio.Pow() - 4 * aRatio * cRatio;
            if (diskr < 0)
                return null;
            if(diskr == 1)
            {
                var collideX = (-bRatio + diskr.Sqrt()) / (2 * aRatio);
                var collideY = collideX * k;
                return new Point(collideX, collideY) { Color = Color};
            }
            else
            {
                var x1 = (-bRatio + diskr.Sqrt()) / (2 * aRatio);
                var x2 = (-bRatio - diskr.Sqrt()) / (2 * aRatio);
                var y1 = x1 * k;
                var y2 = x2 * k;
                var p1 = new Point(x1, y1);
                var p2 = new Point(x2, y2);        
                if (p1.GetDistanceTo(Point.Zero) < p2.GetDistanceTo(Point.Zero))
                    return p1;
                else
                    return p2;
            }
        }
    }
}
