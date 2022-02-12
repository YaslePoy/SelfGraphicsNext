using SelfGraphicsNext.BaseGraphics;

namespace SelfGraphicsNext.RayGraphics.Graphics2D
{
    public class Camera
    {
        public readonly double FOW;

        public int Resolution;

        public Point Position;

        public Direction Direction;
        public Camera()
        {
            Position = Point.Zero;
            FOW = 100;
            Resolution = 50;
        }

        public Camera(Point position, double fow, int res)
        {
            Position = position;
            FOW = fow;
            Resolution = res;
        }

        public List<Point> Render(Grid map, bool drawResults = false)
        {
            List<Point> result = new List<Point>();
            if (drawResults)
            {
                if (map.Layers.Count < 3)
                    map.AddLayer();
                map.AddDrawable(new Line(Position.X, Position.Y) { Distance = 50, Direction = Direction - (FOW / 2) }, 2);
                map.AddDrawable(new Line(Position.X, Position.Y) { Distance = 50, Direction = Direction + (FOW / 2) }, 2);
            }
            Ray colider = new Ray(Direction - (FOW / 2), Position);
            var step = FOW / Resolution;
            for (double i = 0; i <= FOW; i += step)
            {
                colider.UpdateAim(map);
                if (!(colider.Aim is null))
                {
                    colider.Aim.Distance *= (Direction - colider.Direction).AngleGrads.Cos();
                    result.Add(colider.Aim);
                    if (drawResults)
                    {

                        map.AddDrawable(new Circle(colider.Aim.X, colider.Aim.Y, 3) { Color = SFML.Graphics.Color.White }, 2);
                        map.AddDrawable(new Line(Position, colider.Aim) { Color = SFML.Graphics.Color.White }, 2);
                    }
                }
                else if (drawResults)
                {
                    map.AddDrawable(new Line(Position, colider.GetPointByDistance(50)) { Color = SFML.Graphics.Color.White }, 2);

                }
                colider.Direction += step;

            }
            return result;
        }
    }
}
