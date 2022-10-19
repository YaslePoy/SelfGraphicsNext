using SelfGraphicsNext.RayGraphics.Graphics3D.Geometry;
using SelfGraphicsNext.RayGraphics.Graphics3D.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace SelfGraphicsNext
{
    public class RunConfig
    {
        public string ConfigPath;
        public string Model { get; set; }
        public double FOW { get; set; }
        public EasyPoint SceneLight { get; set; }
        public EasyPoint Position { get; set; }
        public EasyDirection Direction { get; set; }
        public int XResolution { get; set; }
        public int YResolution { get; set; }
        public (Scene scene, Camera3 camera) Get()
        {
            var scene = Scene.LoadSceneObj(Model);
            scene.Light = SceneLight.Get();
            Viewer viewer = new Viewer(FOW, Position.Get(), Direction.Get(), XResolution, YResolution);
            Camera3 cam = new Camera3(viewer);
            return (scene, cam);
        }
    }

    public class EasyPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public Point3 Get() => new Point3(X, Y, Z);
    }
    public class EasyDirection
    {
        public double HorisontalView { get; set; }
        public double VertivalView { get; set; }

        public Direction3 Get() => new Direction3(HorisontalView, VertivalView);
    }
}
