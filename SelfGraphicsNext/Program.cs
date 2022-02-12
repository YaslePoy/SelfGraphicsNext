using SelfGraphicsNext.RayGraphics.Graphics3D.Geometry;
using SelfGraphicsNext.RayGraphics.Graphics3D.Rendering;
using SFML.Graphics;
using SFML.Window;
using System.Diagnostics;

namespace SelfGraphicsNext
{
    class Run
    {
        public static Scene scene;
        public static RenderWindow _rw;
        public static Image rend;
        public static Camera3 camera;
        public static uint x = 200;
        public static uint y = 200;
        public const int mRatio = 6;

        public static void Main(string[] args)
        {
            scene = Scene.LoadSceneObj(@"C:\Users\mmm60\Desktop\AppDatas\2cubesRB.obj");
            camera = new Camera3(120, new Point3(-5, 0, 0), new Direction3(0, 0));
            rend = camera.RenderSceneMulti(scene, x, y, mRatio);
            _rw = new RenderWindow(new VideoMode(x * 1, y * 1), "SGN test");
            //camera.RendImgAsync(scene, x, y);
            _rw.Closed += (o, e) => _rw.Close();
            _rw.SetActive(true);
            new List<int>().Any();
            //_rw.Draw(new Sprite(new Texture(rend)));
            _rw.Display();

            //_rw.SetFramerateLimit(30);
            while (_rw.IsOpen)
            {
                _rw.Clear();
                _rw.DispatchEvents();
                {
                    _rw_MouseMoved();
                    {
                        _rw.SetTitle($"{camera.Position} : {camera.Direction}");
                        if (rend != null)
                            _rw.Draw(new Sprite(new Texture(rend)));
                    }
                }
                _rw.Display();
            }

            //public static void DrawFrame(List<Point> img, double maxDist, RenderWindow window)
            //{
            //    double height(double dist)
            //    {
            //        if (dist > maxDist)
            //            return 0;
            //        return (maxDist - dist) / maxDist;
            //    }
            //    double lineX = x / img.Count;
            //    foreach (Point point in img)
            //    {
            //        if (point is null)
            //            continue;
            //        double currentLineY = y * height(point.Distance);
            //        double yPos = (y / 2) - (currentLineY / 2);
            //        Reclangle veiw = new Reclangle(lineX * img.IndexOf(point), yPos, lineX, currentLineY) { Color = point.Color };
            //        veiw.Draw(window);
            //    }
            //}

            static void _rw_MouseMoved()
            {
                void updateImg()
                {
                    rend = camera.RenderSceneMulti(scene, x, y, mRatio);
                }
                //var currentCursorPosition = Mouse.GetPosition(_rw);
                //if (currentCursorPosition.X + 10 > _rw.Size.X)
                //    _rw.Size = new SFML.System.Vector2u(_rw.Size.X + 10, _rw.Size.Y);
                //if (currentCursorPosition.Y + 10 > _rw.Size.Y)
                //    _rw.Size = new SFML.System.Vector2u(_rw.Size.X, _rw.Size.Y + 10);
                if (Keyboard.IsKeyPressed(Keyboard.Key.Up))
                {
                    while (Keyboard.IsKeyPressed(Keyboard.Key.Up))
                        continue;
                    camera.Direction.Vertical += 10;
                    updateImg();
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.Down))
                {
                    while (Keyboard.IsKeyPressed(Keyboard.Key.Down))
                        continue;
                    camera.Direction.Vertical -= 10;
                    updateImg();
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.Left))
                {
                    while (Keyboard.IsKeyPressed(Keyboard.Key.Up))
                        continue;
                    camera.Direction.Horisontal -= 10;
                    updateImg();
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.Right))
                {
                    while (Keyboard.IsKeyPressed(Keyboard.Key.Right))
                        continue;
                    camera.Direction.Horisontal += 10;
                    updateImg();
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.W))
                {
                    while (Keyboard.IsKeyPressed(Keyboard.Key.W))
                        continue;
                    camera.Position.X += 1;
                    updateImg();
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.S))
                {
                    while (Keyboard.IsKeyPressed(Keyboard.Key.S))
                        continue;
                    camera.Position.X -= 1;
                    updateImg();
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.A))
                {
                    while (Keyboard.IsKeyPressed(Keyboard.Key.A))
                        continue;
                    camera.Position.Y -= 1;
                    updateImg();
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.D))
                {
                    while (Keyboard.IsKeyPressed(Keyboard.Key.D))
                        continue;
                    camera.Position.Y += 1;
                    updateImg();
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.Q))
                {
                    while (Keyboard.IsKeyPressed(Keyboard.Key.Q))
                        continue;
                    camera.Position.Z += 1;
                    updateImg();
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.E))
                {
                    while (Keyboard.IsKeyPressed(Keyboard.Key.E))
                        continue;
                    camera.Position.Z -= 1;
                    updateImg();
                }
            }
        }
    }
}
