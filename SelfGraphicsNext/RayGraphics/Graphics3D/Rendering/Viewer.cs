using SelfGraphicsNext.BaseGraphics;
using SelfGraphicsNext.RayGraphics.Graphics3D.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfGraphicsNext.RayGraphics.Graphics3D.Rendering
{
    public class Viewer
    {
        public double FOW;
        public Point3 Position;
        public Direction3 Direction;
        public int Width, Height;
        public double FOWVertical => FOW / Width * Height;
        public Viewer(double fOW, Point3 position, Direction3 direction, int width, int height)
        {
            if (fOW == 0)
                throw new ArgumentException("FOW can't be zero");
            if(width == 0 || height == 0)
                throw new ArgumentException("Any resolution can't be zero");
            FOW = fOW;
            Position = position;
            Direction = direction;
            Width = width;
            Height = height;
        }
        public ViewerSer GetForSer()=> new ViewerSer() { FOW = FOW, Position = Position, Direction = Direction, Height = Height, Width = Width, FOWVertical=FOWVertical };
    }
    public class ViewerSer
    {
        public double FOW { get; set; }
        public Point3 Position { get; set; }
        public Direction3 Direction { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public double FOWVertical { get; set; }
    }
}
