using SelfGraphicsNext.BaseGraphics;

namespace SelfGraphicsNext.RayGraphics.Graphics2D
{
    public class Ray
    {
        public Point? Aim;
        public Direction Direction { get; set; }

        public Point Position { get; set; }
        public Ray(double direction, Point position)
        {
            Direction = new Direction(direction);
            Position = position;
        }

        public Ray(Direction direction, Point position)
        {
            Direction = direction;
            Position = position;
        }
        public void UpdateAim(Grid space)
        {
            var colls = new List<Point>();
            foreach (var colObj in space.Layers[1])
            {
                Point mbColl = colObj.Collide(this);
                if (!(mbColl is null))
                {
                    mbColl.SetDistanceTo(Position);
                    colls.Add(mbColl);
                }
            }
            Aim = colls.MinBy(i => i.Distance);
        }

        public Point GetPointByDistance(double distance)
        {

            return Position + new Point(Direction.Cos * distance, Direction.Sin * distance);
        }
    }
}
