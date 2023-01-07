using ManagedCuda;
using ManagedCuda.VectorTypes;
using SelfGraphicsNext.BaseGraphics;
using SelfGraphicsNext.RayGraphics.Graphics3D.Geometry;
using SFML.Graphics;
using System.Numerics;
using System.Threading;

namespace SelfGraphicsNext.RayGraphics.Graphics3D.Rendering
{
    public class Camera3
    {
        CudaDeviceVariable<float3> d_pos;
        CudaDeviceVariable<float3> d_out;
        CudaDeviceVariable<PolygonCUDA> d_plgs;
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
            Rendering.Start();

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
                    if (renderGroups.Count == gID)
                        renderGroups.Add(new List<Ray3>());
                    renderGroups[gID].Add(new Ray3(dirs[i, j], ViewState.Position) { ImagePosition = new Point(i, j) });
                    gID++;
                    if (gID == k)
                        gID = 0;

                }
            }
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
                            Color finalColor = result.Colision.Color;
                            if (scene.Light != new Point3())
                            {
                                var norm = /*Vector3.Reflect(colider.Direction,  result.ColidedPoligon.Normal.Vector)*/result.ColidedPoligon.Normal.Vector;
                                var toLight = scene.Light.Vector - result.Colision.Vector;
                                double kRatio = Vector3.Dot(norm, toLight);
                                Ray3 shadowRay = new Ray3(Vector3.Normalize(toLight), result.Colision);
                                var shadowRes = shadowRay.CollideInSceneIns(scene, result.ColidedPoligon);
                                if (shadowRes.Colided)
                                {
                                    kRatio = 0;
                                }
                                kRatio = Math.Pow(kRatio.Abs(), 0.3);
                                finalColor = Utils.Mult(finalColor, kRatio);
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
            if (wait)
                Parallel.ForEach(renderGroups, new ParallelOptions() { MaxDegreeOfParallelism = k }, renderPool);
            else
                Task.Run(() => Parallel.ForEach(renderGroups, new ParallelOptions() { MaxDegreeOfParallelism = k }, renderPool));
        }
        public void RenderSceneCUDA(Scene scene, bool wait = false)
        {
            if (Rendering.State == RenderState.Active)
                Rendering.Stop();
            if (Rendering.State == RenderState.Ready || Rendering.State == RenderState.Stopped)
                Rendering.Clear();
            Rendering.Start();
            var position = ViewState.Position.GetFloat3();
            var light = scene.Light.GetFloat3();
            var allPgs = scene.GetPolyons().ToArray();
            var x = scene.cudaDevice.GetOccupancyMaxPotentialBlockSize();
            d_plgs = allPgs;
            d_pos = position;
            d_out = new CudaDeviceVariable<float3>(Rendering.TotalPixels);
            scene.cudaDevice.GridDimensions = new dim3((int)(ViewState.Width / scene.cudaDevice.BlockDimensions.x + 1),
                (int)(ViewState.Height / scene.cudaDevice.BlockDimensions.x + 1));
            scene.cudaDevice.Run(d_plgs.DevicePointer, allPgs.Length, new int2(ViewState.Width, ViewState.Height),
                position, light, (float)ViewState.FOW,
                new float2(ViewState.Direction.Horisontal.AngleGradsF, ViewState.Direction.Vertical.AngleGradsF),
                d_out.DevicePointer);
            float3[] colors = d_out;
            int index = 0;
            for (int i = 0; i < ViewState.Height; i++)
            {
                for (int j = 0; j < ViewState.Width; j++)
                {
                    Rendering.SetPixel(new Point(j, i), new Color((byte)(colors[index].x), (byte)(colors[index].y), (byte)(colors[index].z)));
                    index++;
                }
            }
            d_pos.Dispose();
            d_out.Dispose();
            d_plgs.Dispose();
        }
        //public float3 VirtualCuda(PolygonCUDA[] pgs, int polCount, int2 resolution, float3 xyz, float3 light, float fow, float2 view, int row, int col)
        //{
        //    float3 y;
        //    y.x = 255;
        //    y.y = 255;
        //    y.z = 255;
        //    if (col >= resolution.x || row >= resolution.y)
        //    {
        //        return new float3(-1, -1, -1);
        //    }

        //    float3 dir;
        //    dir.y = col;
        //    dir.z = row;
        //    int2 halfRes;
        //    halfRes.x = resolution.x / 2;
        //    halfRes.y = resolution.y / 2;
        //    dir.y -= halfRes.x;
        //    dir.z -= halfRes.y;
        //    dir.z *= -1;
        //    dir.y /= halfRes.x;
        //    dir.z /= halfRes.y;
        //    float halfLenHor = MathF.Sin(fow * Utils.ToRadF / 2);
        //    dir.y *= halfLenHor;
        //    dir.x = MathF.Sqrt(1 - MathF.Pow(dir.y, 2));
        //    float halfLenVer = halfLenHor / resolution.x * resolution.y;
        //    dir.z *= halfLenVer;
        //    float verRatio = MathF.Sqrt(1 - MathF.Pow(dir.z, 2));
        //    if (view.y != 0)
        //    {
        //        float2 temp;
        //        float trueY = view.y * Utils.ToRadF;
        //        float2 tg;
        //        tg.x = MathF.Cos(trueY);
        //        tg.y = MathF.Sin(trueY);
        //        temp.y = verRatio * tg.x - dir.z * tg.y;
        //        temp.x = verRatio * tg.y + dir.z * tg.x;
        //        dir.z = temp.x;
        //        verRatio = temp.y;
        //    }
        //    dir.x *= verRatio;
        //    dir.y *= verRatio;
        //    if (view.x != 0)
        //    {
        //        float2 temp;
        //        float trueX = view.x * Utils.ToRadF;
        //        float2 tg;
        //        tg.x = MathF.Cos(trueX);
        //        tg.y = MathF.Sin(trueX);
        //        temp.x = dir.x * tg.x - dir.y * tg.y;
        //        temp.y = dir.x * tg.y + dir.y * tg.x;
        //        dir.x = temp.x;
        //        dir.y = temp.y;
        //    }
        //    float3 baseColor;
        //    baseColor.x = 16;
        //    baseColor.y = 16;
        //    baseColor.z = 16;
        //    float minDist = -1;
        //    float3 ret;
        //    ret = baseColor;
        //    float3 newPoint = new float3();
        //    int skip = -1;
        //    bool coled = false;
        //    float colRatio = 1;
        //    float2[] pts = new float2[4];
        //    PolygonCUDA pol;
        //    float3 colPoint = new float3();
        //    for (int j = 0; j < polCount; j++)
        //    {
        //        pol = pgs[j];
        //        float3 abc = pol.nor;
        //        float upper = pol.d + abc.x * xyz.x + abc.y * xyz.y + abc.z * xyz.z;
        //        float lower = abc.x * dir.x + abc.y * dir.y + abc.z * dir.z;
        //        if (lower == 0 && upper == 0)
        //        {
        //            continue;
        //        }
        //        if (upper > 0 && lower == 0)
        //        {
        //            continue;
        //        }
        //        float t = -(upper / lower);
        //        if (t < 0)
        //        {
        //            continue;
        //        }
        //        newPoint.x = dir.x * t + xyz.x;
        //        newPoint.y = dir.y * t + xyz.y;
        //        newPoint.z = dir.z * t + xyz.z;
        //        if (abc.x != 0)
        //        {
        //            pts[0].x = newPoint.y;
        //            pts[0].y = newPoint.z;
        //            pts[1].x = pol.p1.y;
        //            pts[1].y = pol.p1.z;
        //            pts[2].x = pol.p2.y;
        //            pts[2].y = pol.p2.z;
        //            pts[3].x = pol.p3.y;
        //            pts[3].y = pol.p3.z;
        //        }
        //        else if (abc.y != 0)
        //        {
        //            pts[0].x = newPoint.x;
        //            pts[0].y = newPoint.z;
        //            pts[1].x = pol.p1.x;
        //            pts[1].y = pol.p1.z;
        //            pts[2].x = pol.p2.x;
        //            pts[2].y = pol.p2.z;
        //            pts[3].x = pol.p3.x;
        //            pts[3].y = pol.p3.z;
        //        }
        //        else
        //        {
        //            pts[0].x = newPoint.x;
        //            pts[0].y = newPoint.y;
        //            pts[1].x = pol.p1.x;
        //            pts[1].y = pol.p1.y;
        //            pts[2].x = pol.p2.x;
        //            pts[2].y = pol.p2.y;
        //            pts[3].x = pol.p3.x;
        //            pts[3].y = pol.p3.y;
        //        }
        //        float a;
        //        a = (pts[1].x - pts[0].x) * (pts[2].y - pts[1].y) - (pts[2].x - pts[1].x) * (pts[1].y - pts[0].y);
        //        float b;
        //        b = (pts[2].x - pts[0].x) * (pts[3].y - pts[2].y) - (pts[3].x - pts[2].x) * (pts[2].y - pts[0].y);
        //        float c;
        //        c = (pts[3].x - pts[0].x) * (pts[1].y - pts[3].y) - (pts[1].x - pts[3].x) * (pts[3].y - pts[0].y);
        //        bool isIn = (a >= 0 && b >= 0 && c >= 0) || (a <= 0 && b <= 0 && c <= 0);
        //        if (isIn)
        //        {

        //            float newLen = MathF.Sqrt(MathF.Pow(newPoint.x - xyz.x, 2) + MathF.Pow(newPoint.y - xyz.y, 2) + MathF.Pow(newPoint.z - xyz.z, 2));
        //            bool around;
        //            if (minDist == -1)
        //            {
        //                minDist = newLen;
        //                around = true;
        //            }
        //            else
        //            {
        //                around = newLen < minDist;
        //            }
        //            if (around)
        //            {
        //                minDist = newLen;
        //                colRatio = (light.x - newPoint.x) * abc.x
        //                    + (light.y - newPoint.y) * abc.y
        //                    + (light.z - newPoint.z) * abc.z;
        //                if (colRatio < 0)
        //                    colRatio = 0;
        //                ret = pol.col;
        //                skip = j;
        //                coled = true;
        //                colPoint = newPoint;
        //            }
        //        }
        //    }
        //    bool shadow = false;
        //    if (coled)
        //    {
        //        float3 pos = colPoint;
        //        dir.x = light.x - pos.x;
        //        dir.y = light.y - pos.y;
        //        dir.z = light.z - pos.z;
        //        float dirLen = Utils.norm3df(dir.x, dir.y, dir.z);
        //        dir.x /= dirLen;
        //        dir.y /= dirLen;
        //        dir.z /= dirLen;
        //        for (int id = 0; id < polCount; id++)
        //        {
        //            if (id == skip)
        //                continue;
        //            pol = pgs[id];
        //            float3 nor = pol.nor;
        //            float u = pol.d + nor.x * pos.x + nor.y * pos.y + nor.z * pos.z;
        //            float l = nor.x * dir.x + nor.y * dir.y + nor.z * dir.z;
        //            if (l == 0 && u >= 0)
        //                continue;
        //            float t = -(u / l);
        //            if (t < 0)
        //                continue;
        //            float3 sdPt;
        //            sdPt.x = dir.x * t + pos.x;
        //            sdPt.y = dir.y * t + pos.y;
        //            sdPt.z = dir.z * t + pos.z;
        //            if (nor.x != 0)
        //            {
        //                pts[0].x = sdPt.y;
        //                pts[0].y = sdPt.z;
        //                pts[1].x = pol.p1.y;
        //                pts[1].y = pol.p1.z;
        //                pts[2].x = pol.p2.y;
        //                pts[2].y = pol.p2.z;
        //                pts[3].x = pol.p3.y;
        //                pts[3].y = pol.p3.z;
        //            }
        //            else if (nor.y != 0)
        //            {
        //                pts[0].x = sdPt.x;
        //                pts[0].y = sdPt.z;
        //                pts[1].x = pol.p1.x;
        //                pts[1].y = pol.p1.z;
        //                pts[2].x = pol.p2.x;
        //                pts[2].y = pol.p2.z;
        //                pts[3].x = pol.p3.x;
        //                pts[3].y = pol.p3.z;
        //            }
        //            else
        //            {
        //                pts[0].x = sdPt.x;
        //                pts[0].y = sdPt.y;
        //                pts[1].x = pol.p1.x;
        //                pts[1].y = pol.p1.y;
        //                pts[2].x = pol.p2.x;
        //                pts[2].y = pol.p2.y;
        //                pts[3].x = pol.p3.x;
        //                pts[3].y = pol.p3.y;
        //            }
        //            float a;
        //            a = (pts[1].x - pts[0].x) * (pts[2].y - pts[1].y) - (pts[2].x - pts[1].x) * (pts[1].y - pts[0].y);
        //            float b;
        //            b = (pts[2].x - pts[0].x) * (pts[3].y - pts[2].y) - (pts[3].x - pts[2].x) * (pts[2].y - pts[0].y);
        //            float c;
        //            c = (pts[3].x - pts[0].x) * (pts[1].y - pts[3].y) - (pts[1].x - pts[3].x) * (pts[3].y - pts[0].y);
        //            bool isIn = (a >= 0 && b >= 0 && c >= 0) || (a <= 0 && b <= 0 && c <= 0);
        //            if (isIn)
        //            {
        //                shadow = true;
        //                break;
        //            }

        //        }
        //    }
        //    if (shadow)
        //    {
        //        colRatio *= 0.25f;
        //    }
        //    colRatio = MathF.Pow(MathF.Abs(colRatio), 0.3f);
        //    ret.x *= colRatio;
        //    ret.y *= colRatio;
        //    ret.z *= colRatio;
        //    return ret;
        //}
    }
}
