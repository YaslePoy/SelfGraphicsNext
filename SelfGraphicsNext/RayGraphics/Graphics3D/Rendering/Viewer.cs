using ManagedCuda.VectorTypes;
using SelfGraphicsNext.RayGraphics.Graphics3D.Geometry;

namespace SelfGraphicsNext.RayGraphics.Graphics3D.Rendering
{
    public class Viewer
    {
        float3[] totalDirs;

        public void UpdateDirectionList()
        {
            return;
            var FOWV = FOWVertical;
            var step = FOW / width;
            var startfx = Direction.Horisontal.AngleGrads - (FOW / 2);
            var startfy = Direction.Vertical.AngleGrads + (FOWV / 2);
            var currentDir = new Direction3(startfx, startfy);
            var fowHalf = FOW / 2;
            var matrixSize = Utils.Tan(fowHalf);
            var pixStep = matrixSize / (Width / 2);
            List<Ray3> rays = new List<Ray3>();
            var dirs = Direction.GetDirectionsByResolution((int)width, (int)height, fow, FOWV);
            totalDirs = new float3[width * height];
            int index = 0;
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    totalDirs[index] = dirs[j, i].GetVector().GetFloat3();
                    index++;
                }
            }
        }
        double fow;
        Direction3 viewDir;
        int width, height;
        public double FOW
        {
            get => fow; 
            set
            {
                fow = value;
                UpdateDirectionList();
            }
        }
        public Point3 Position;
        public Direction3 Direction;
        public int Width { get => width; 
            set
            {
                width = value;
                UpdateDirectionList();
            }
        }
        public int Height { get => height; 
            set
            {
                height = value;
                UpdateDirectionList();
            }
        }
        public double FOWVertical => FOW / Width * Height;
        public Viewer(double fOW, Point3 position, Direction3 direction, int width, int height)
        {
            if (fOW == 0)
                throw new ArgumentException("FOW can't be zero");
            if (width == 0 || height == 0)
                throw new ArgumentException("Any resolution can't be zero");
            fow = fOW;
            Position = position;
            viewDir = direction;
            this.width = width;
            this.height = height;
            UpdateDirectionList();
        }
        public float3[] RenderDirections => totalDirs;
        public ViewerSer GetForSer() => new ViewerSer() { FOW = FOW, Position = Position, Direction = Direction, Height = Height, Width = Width, FOWVertical = FOWVertical };
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
