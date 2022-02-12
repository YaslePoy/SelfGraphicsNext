using SelfGraphicsNext.RayGraphics.Graphics2D;
using SFML.Graphics;

namespace SelfGraphicsNext.BaseGraphics
{
    public class Line : Point
    {
        private Point end;
        private Direction direction;
        private double distance;

        public double KRatio => direction.Tan;
        public Point End
        {
            get
            {
                return end;
            }
            set
            {
                end = value;
                direction = Utils.GetDirectionBetween(this, value);
                SetDistanceTo(value);
            }
        }

        public Direction Direction
        {
            get
            {
                return direction;
            }
            set
            {
                direction = value;
                end = new Point(X, Y) + new Point(value.Cos * distance, value.Sin * distance);
            }
        }

        public double Distance
        {
            get
            {
                return distance;
            }
            set
            {
                distance = value;
                end = new Point(X, Y) + new Point(direction.Cos * value, direction.Sin * value);
            }
        }

        public Line()
        {
            this.X = 0;
            this.Y = 0;
            Distance = 1;
            Direction = new Direction();
        }

        public Line(double x, double y) : base(x, y)
        {
            this.X = x;
            this.Y = y;
            Distance = 1;
            Direction = new Direction();
        }

        public Line(Point start, Point end)
        {
            X = start.X;
            Y = start.Y;
            Color = start.Color;
            End = end;
        }

        public override void Draw(RenderWindow win)
        {
            win.Draw(new Vertex[2] { new Vertex(base.getVec2f(), Color), new Vertex(end.getVec2f(), Color) },
                PrimitiveType.Lines);
        }

        public static Line operator +(Line line, Point vector)
        {
            line.X += vector.X;
            line.Y += vector.Y;
            line.end += vector;
            return line;
        }

        public override Point? Collide(Ray ray2D)
        {
            try
            {
                decimal x1, x2, x3, x4, y1, y2, y3, y4 = Decimal.Zero;
                x1 = (decimal)X;
                x2 = (decimal)end.X;
                x3 = (decimal)ray2D.Position.X;
                x4 = (decimal)(ray2D.Position.X + ray2D.Direction.Cos * 3);
                y1 = (decimal)Y;
                y2 = (decimal)end.Y;
                y3 = (decimal)ray2D.Position.Y;
                y4 = (decimal)(ray2D.Position.Y + ray2D.Direction.Sin * 3);
                decimal upper = Decimal.Zero;
                upper = (x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4);
                decimal lower = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
                if (lower == 0) return null;
                upper = upper / lower;
                Point final = new Point((double)upper, 0);
                upper = (x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4);
                final.Y = (double)(upper / lower);
                final.Color = Color;
                List<double> xs = new List<double>() { X, end.X };
                List<double> ys = new List<double>() { Y, end.Y };
                final.Color = Color;
                if (final.X >= xs.Min() && final.X <= xs.Max() && final.Y >= ys.Min() && final.Y <= ys.Max()) return final;
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}
