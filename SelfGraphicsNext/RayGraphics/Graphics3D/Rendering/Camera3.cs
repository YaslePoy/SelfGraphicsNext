using SelfGraphicsNext.BaseGraphics;
using SelfGraphicsNext.RayGraphics.Graphics3D.Geometry;
using SFML.Graphics;

namespace SelfGraphicsNext.RayGraphics.Graphics3D.Rendering
{
    public class Camera3
    {
        public readonly double FOW;
        public Point3 Position { get; set; }
        public Direction3 Direction;

        public Camera3(Point3 position, Direction3 direction)
        {
            Position = position;
            Direction = direction;
            FOW = 100;
        }

        public Camera3(double fOV, Point3 position, Direction3 direction)
        {
            FOW = fOV;
            Position = position;
            Direction = direction;
        }

        public Image RenderScene(Scene scene, uint w, uint h)
        {

            Image outImg = new Image(w, h);
            var FOWV = FOW / w * h;
            var step = FOW / w;
            var startfx = Direction.Horisontal.AngleGrads - (FOW / 2);
            var startfy = Direction.Vertical.AngleGrads + (FOWV / 2);
            Ray3 colider = new Ray3(new Direction3(-startfx, startfy), Position);

            for (uint i = 0; i < h; i += 1)
            {

                for (uint j = 0; j < w; j += 1)
                {
                    if (colider.CollideInScene(scene))
                    {
                        try
                        {

                            var col = colider.Aim;

                            outImg.SetPixel(j, i, colider.Aim.Color);

                        }
                        catch (AccessViolationException ex)
                        {
                            Console.Error.WriteLine(ex.Message);
                            Console.WriteLine($"Render exept {i} {j} pixel");
                        }
                    }
                    else
                    {
                        //Console.WriteLine($"direcrion : {colider.Direction}, pixel : ({i}:{j})");
                        if (colider.Direction.Vertical.AngleGrads is > 0 and < 180)
                            outImg.SetPixel(j, i, new Color(0, 255, 255));
                        else
                            outImg.SetPixel(j, i, new Color(0, 66, 66));

                    }
                    colider.Direction.Horisontal -= step;
                }
                colider.Direction.Vertical -= step;
                colider.Direction.Horisontal += FOW;
            }

            return outImg;
        }

        public void RenderSceneNonOut(Scene scene, uint w, uint h)
        {

            Run.rend = new Image(w, h);
            var FOWV = FOW / w * h;
            var step = FOW / w;
            var startfx = Direction.Horisontal.AngleGrads - (FOW / 2);
            var startfy = Direction.Vertical.AngleGrads + (FOWV / 2);
            Ray3 colider = new Ray3(new Direction3(-startfx, startfy), Position);

            for (uint i = 0; i < h; i += 1)
            {

                for (uint j = 0; j < w; j += 1)
                {
                    if (colider.CollideInScene(scene))
                    {
                        try
                        {

                            Run.rend.SetPixel(j, i, colider.Aim.Color);

                        }
                        catch (AccessViolationException ex)
                        {
                            Console.Error.WriteLine(ex.Message);
                            Console.WriteLine($"Render exept {i} {j} pixel");
                        }
                    }
                    else
                    {
                        //Console.WriteLine($"direcrion : {colider.Direction}, pixel : ({i}:{j})");
                        if (colider.Direction.Vertical.AngleGrads is > 0 and < 180)
                            Run.rend.SetPixel(j, i, new Color(0, 255, 255));
                        else
                            Run.rend.SetPixel(j, i, new Color(0, 66, 66));

                    }
                    colider.Direction.Horisontal -= step;
                }
                colider.Direction.Vertical -= step;
                colider.Direction.Horisontal += FOW;
            }
        }

        public Image RenderSceneMulti(Scene scene, uint w, uint h, int k = 2)
        {

            Image outImg = new Image(w, h);
            var FOWV = FOW / w * h;
            var step = FOW / w;
            var startfx = Direction.Horisontal.AngleGrads - (FOW / 2);
            var startfy = Direction.Vertical.AngleGrads + (FOWV / 2);
            var currentDir = new Direction3(startfx, startfy);
            var fowHalf = FOW / 2;
            var matrixSize = Utils.Tan(fowHalf);
            var pixStep = matrixSize / (w / 2);
            List<Ray3> rays = new List<Ray3>();
            for (int i = 0; i < h; i++)
            {

                for (int j = 0; j < w; j++)
                {
                    var abs = j - w / 2;

                    double curLen = abs * pixStep;

                    //var delta = new Direction3(Math.Atan(curLen).ToDegrees(), -step * (i - h / 2));
                    rays.Add(new Ray3(currentDir, Position) { ImagePosition = new Point(j, i) });
                    currentDir.Horisontal += step;
                }
                currentDir.Vertical -= step;
                currentDir.Horisontal -= FOW;
            }
            var chunks = rays.Chunk((int)Math.Floor((decimal)(w * h / k)));
            void renderPool(Ray3[] rays)
            {
                foreach (Ray3 colider in rays)
                {
                    if (colider.CollideInScene(scene))
                    {
                        try
                        {

                            outImg.SetPixel((uint)colider.ImagePosition.X, (uint)colider.ImagePosition.Y, colider.Aim.Color);

                        }
                        catch (AccessViolationException ex)
                        {
                            Console.Error.WriteLine(ex.Message);
                            Console.WriteLine($"Render exept {colider.ImagePosition.X} {colider.ImagePosition.Y} pixel");
                        }
                    }
                    else
                    {
                        //Console.WriteLine($"direcrion : {colider.Direction}, pixel : ({i}:{j})");
                        if (colider.Direction.Vertical.AngleGrads is > 0 and < 180)
                            outImg.SetPixel((uint)colider.ImagePosition.X, (uint)colider.ImagePosition.Y, new Color(0, 255, 255));
                        else
                            outImg.SetPixel((uint)colider.ImagePosition.X, (uint)colider.ImagePosition.Y, new Color(0, 66, 66));

                    }
                }
            }
            var res = Parallel.ForEach(chunks, renderPool);
            return outImg;
        }

        public void RendImgAsync(Scene scene, uint w, uint h)
        {
            Task.Run(() => RenderSceneNonOut(scene, w, h));
        }
    }
}
