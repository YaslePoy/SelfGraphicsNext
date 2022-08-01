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
        public Image LiveRenderInage;
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
            var currentDir = new Direction3(startfx, startfy);
            var fowHalf = FOW / 2;
            var matrixSize = Utils.Tan(fowHalf);
            var pixStep = matrixSize / (w / 2);
            Ray3 colider = new Ray3(Direction, Position);
            for (uint i = 0; i < h; i++)
            {
                var yabs = -(i - h / 2);
                var ylen = yabs * pixStep;
                var curV = Math.Atan(ylen).ToDegrees();
                for (uint j = 0; j < w; j++)
                {
                    var abs = j - w / 2;

                    double curLen = abs * pixStep;

                    var delta = new Direction3(Math.Atan(curLen).ToDegrees(), curV);
                    colider.Direction = Direction + delta;
                    var result = colider.CollideInScene(scene);
                    if (result.Codiled)
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
                }

            }          
            return outImg;
        }
        public Task RenderSceneMulti(Scene scene, uint w, uint h, int k = 2)
        {
            LiveRenderInage = new Image(w, h);
            var FOWV = FOW / w * h;
            var step = FOW / w;
            var startfx = Direction.Horisontal.AngleGrads - (FOW / 2);
            var startfy = Direction.Vertical.AngleGrads + (FOWV / 2);
            var currentDir = new Direction3(startfx, startfy);
            var fowHalf = FOW / 2;
            var matrixSize = Utils.Tan(fowHalf);
            var pixStep = matrixSize / (w / 2);
            Ray3[,] rays = new Ray3[w, h];
            var dirs = Direction.GetDirectionsByResolution((int)w, (int)h, FOW, FOWV);
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    rays[i, j] = new Ray3(dirs[i, j], Position) { ImagePosition = new Point(i, j) };
                }
            }
            List<Ray3[]> renderRecs = new List<Ray3[]>();
            if (w % 10 == 0 && h % 10 == 0)
            {
                for (int i = 0; i < h / 10; i++)
                {
                    for (int j = 0; j < w / 10; j++)
                    {
                        var inds = Utils.DoubleIndexStartFinish(j * 10, j * 10 + 10, i * 10, i * 10 + 10);
                        var localRays = new List<Ray3>();
                        foreach (var index in inds)
                        {
                            localRays.Add(rays[index[0], index[1]]);
                        }
                        renderRecs.Add(localRays.ToArray());
                    }
                }
            }

            void renderPool(List<Ray3> rays)
            {
                for (int i = 0; i < rays.Count; i++)
                {
                    var colider = rays[i];
                //}
                //foreach (Ray3 colider in rays)
                //{
                    var result = colider.CollideInScene(scene);
                    if (result.Codiled)
                    {
                        try
                        {
                            Color finalColor = colider.Aim.Color;
                            if (scene.Light != null)
                            {
                                var norm = result.ColidedPoligon.Normal;
                                var toLight = scene.Light - result.Colision;
                                double kRatio = norm.ScalarMul(toLight);
                                kRatio /= norm.Lenght * toLight.Lenght;
                                if (kRatio < 0)
                                {
                                    kRatio = norm.ScalarMul(new Point3(0, 0, -1)) / 2;
                                    if (norm.Z < 0)
                                        kRatio /= 2;
                                }
                                Ray3 shadowRay = new Ray3(result.Colision);
                                shadowRay.Direction.SetDirection(toLight);
                                var shadowRes = shadowRay.CollideInSceneIns(scene, result.ColidedPoligon);
                                if (shadowRes.Codiled)
                                {
                                    kRatio /= 2;
                                }
                                finalColor = Utils.Mult(finalColor, kRatio.Abs());
                            }
                            LiveRenderInage.SetPixel((uint)colider.ImagePosition.X, (uint)colider.ImagePosition.Y, finalColor);
                        }
                        catch (AccessViolationException ex)
                        {
                            Console.Error.WriteLine(ex.Message);
                            Console.WriteLine($"Render exept {colider.ImagePosition.X} {colider.ImagePosition.Y} pixel");
                        }
                    }
                    else
                    {
                        if (colider.Direction.Vertical.AngleGrads is > 0 and < 180)
                            LiveRenderInage.SetPixel((uint)colider.ImagePosition.X, (uint)colider.ImagePosition.Y, new Color(0, 255, 255));
                        else
                            LiveRenderInage.SetPixel((uint)colider.ImagePosition.X, (uint)colider.ImagePosition.Y, new Color(0, 66, 66));
                    }
                }
            }
            var rendGroups = renderRecs.GroupBy(i => renderRecs.IndexOf(i) % k).ToList().Select(i => Utils.Merge(i.ToList())).ToList();
            var process = Task.Run(() => Parallel.ForEach(rendGroups, renderPool));
            while (!process.IsCompleted) ;
            return process;
        }
    }
}
