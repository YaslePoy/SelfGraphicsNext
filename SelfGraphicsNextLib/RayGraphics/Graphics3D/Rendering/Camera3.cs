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
        public Image RenderSceneMulti(Scene scene, uint w, uint h, int k = 2)
        {
            Image outImg = new Image(w, h);
            var FOWV = FOW / w * h;
            var step = FOW / w;
            var startfx = Direction.Horisontal.AngleGrads - (FOW / 2);
            var startfy = Direction.Vertical.AngleGrads + (FOWV / 2);
            Direction3 currentDir = new Direction3(-startfx, startfy);
            List<Ray3> rays = new List<Ray3>();
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    rays.Add(new Ray3() { ImagePosition = new Point(i, j), Direction = currentDir });
                    currentDir.Horisontal -= step;
                }
                currentDir.Vertical -= step;
                currentDir.Horisontal += FOW;
            }
            void RenderPool(Ray3[] rays)
            {
                foreach (var colider in rays)
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
            var groups = rays.Chunk((int)Math.Floor((decimal)(w * h / k))).ToList();
            var res = Parallel.ForEach(groups, RenderPool);
            while (!res.IsCompleted)
                continue;
            return outImg;
        }
        //public void RenderSceneNonOut(Scene scene, uint w, uint h)
        //{

        //  Run.rend = new Image(w, h);
        //    var FOWV = FOW / w * h;
        //    var step = FOW / w;
        //    var startfx = Direction.Horisontal.AngleGrads - (FOW / 2);
        //    var startfy = Direction.Vertical.AngleGrads + (FOWV / 2);
        //    Ray3 colider = new Ray3(new Direction3(-startfx, startfy), Position);

        //    for (uint i = 0; i < h; i += 1)
        //    {

        //        for (uint j = 0; j < w; j += 1)
        //        {
        //            if (colider.CollideInScene(scene))
        //            {
        //                try
        //                {

        //                    Run.rend.SetPixel(j, i, colider.Aim.Color);

        //                }
        //                catch (AccessViolationException ex)
        //                {
        //                    Console.Error.WriteLine(ex.Message);
        //                    Console.WriteLine($"Render exept {i} {j} pixel");
        //                }
        //            }
        //            else
        //            {
        //                //Console.WriteLine($"direcrion : {colider.Direction}, pixel : ({i}:{j})");
        //                if (colider.Direction.Vertical.AngleGrads is > 0 and < 180)
        //                    Run.rend.SetPixel(j, i, new Color(0, 255, 255));
        //                else
        //                    Run.rend.SetPixel(j, i, new Color(0, 66, 66));

        //            }
        //            colider.Direction.Horisontal -= step;
        //        }
        //        colider.Direction.Vertical -= step;
        //        colider.Direction.Horisontal += FOW;
        //    }
        //}
        //public void RendImgAsync(Scene scene, uint w, uint h)
        //{
        //    Task.Run(() => RenderSceneNonOut(scene, w, h));
        //}
    }
}
