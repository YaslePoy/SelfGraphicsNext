using SelfGraphicsNext.BaseGraphics;
using SelfGraphicsNext.RayGraphics.Graphics3D.Geometry;
using SFML.Graphics;
using System.Collections.Generic;

namespace SelfGraphicsNext.RayGraphics.Graphics3D.Rendering
{
    public class Camera3
    {
        public Viewer ViewState;
        public RenderData Rendering;
        public Camera3(Viewer view)
        {
            ViewState = view;
            Rendering = new RenderData(view.Width, view.Height);
        }
        public void RenderSceneMulti(Scene scene, int k = 2, bool wait = false)
        {
            if (Rendering.State == RenderState.Active)
                Rendering.Stop();
            if (Rendering.State == RenderState.Ready || Rendering.State == RenderState.Stopped)
                Rendering.Clear();
            var FOWV = ViewState.FOWVertical;
            var step = ViewState.FOW / ViewState.Width;
            var startfx = ViewState.Direction.Horisontal.AngleGrads - (ViewState.FOW / 2);
            var startfy = ViewState.Direction.Vertical.AngleGrads + (FOWV / 2);
            var currentDir = new Direction3(startfx, startfy);
            var fowHalf = ViewState.FOW / 2;
            var matrixSize = Utils.Tan(fowHalf);
            var pixStep = matrixSize / (ViewState.Width / 2);
            List<Ray3> rays = new List<Ray3>();
            var dirs = ViewState.Direction.GetDirectionsByResolution((int)ViewState.Width, (int)ViewState.Height, ViewState.FOW, FOWV);
            int gID = 0;
            List<List<Ray3>> renderGroups = new List<List<Ray3>>();
            for (int i = 0; i < ViewState.Width; i++)
            {
                for (int j = 0; j < ViewState.Height; j++)
                {
                    if(renderGroups.Count == gID)
                        renderGroups.Add(new List<Ray3>());
                        renderGroups[gID].Add(new Ray3(dirs[i, j], ViewState.Position) { ImagePosition = new Point(i, j) });
                    gID++;
                    if (gID == k)
                        gID = 0;

                }
            }
            //if (ViewState.Width % 10 == 0 && ViewState.Height % 10 == 0)
            //{
            //    for (int i = 0; i < ViewState.Height / 10; i++)
            //    {
            //        for (int j = 0; j < ViewState.Width / 10; j++)
            //        {
            //            var inds = Utils.DoubleIndexStartFinish(j * 10, j * 10 + 10, i * 10, i * 10 + 10);
            //            var localRays = new List<Ray3>();
            //            foreach (var index in inds)
            //            {
            //                localRays.Add(rays[index[0], index[1]]);
            //            }
            //            renderRecs.Add(localRays.ToArray());
            //        }
            //    }
            //}
            
            void renderPool(List<Ray3> rays)
            {
                for (int i = 0; i < rays.Count; i++)
                {
                    if (Rendering.State == RenderState.Stopped)
                        break;
                    var colider = rays[i];
                    //}
                    //foreach (Ray3 colider in rays)
                    //{
                    var result = colider.CollideInScene(scene);
                    if (result.Colided)
                    {
                        try
                        {
                            Color finalColor = colider.Aim.Color;
                            if (scene.Light != new Point3())
                            {
                                var norm = result.ColidedPoligon.Normal;
                                var toLight = scene.Light - result.Colision;
                                double kRatio = norm.ScalarMul(toLight);
                                kRatio /= norm.Lenght * toLight.Lenght;
                                if (kRatio < 0)
                                {
                                    //kRatio = norm.ScalarMul(new Point3(0, 0, -1)) / 2;
                                    //if (norm.Z < 0)
                                    //    kRatio = 0;
                                    kRatio = 0;
                                }
                                Ray3 shadowRay = new Ray3(result.Colision);
                                shadowRay.Direction.SetDirection(toLight);
                                var shadowRes = shadowRay.CollideInSceneIns(scene, result.ColidedPoligon);
                                if (shadowRes.Colided)
                                {
                                    kRatio =0;
                                }
                                kRatio = Math.Pow(kRatio, 0.45);
                                finalColor = Utils.Mult(finalColor, kRatio.Abs());
                            }
                            Rendering.SetPixel(colider.ImagePosition, finalColor);
                        }
                        catch (AccessViolationException ex)
                        {
                            Console.Error.WriteLine(ex.Message);
                            Console.WriteLine($"Render exept {colider.ImagePosition.X} {colider.ImagePosition.Y} pixel");
                        }
                    }
                    else
                    {
                        Rendering.SetPixel(colider.ImagePosition, new Color(16, 16, 16));
                    }
                }
            }
            Rendering.Start();
            var renderTask =Task.Run(() => Parallel.ForEach(renderGroups, new ParallelOptions() { MaxDegreeOfParallelism = k}, renderPool));
            if (wait)
                while (!renderTask.IsCompleted) ;

            
        }
    }
}
