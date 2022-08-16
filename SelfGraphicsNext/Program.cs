using SelfGraphicsNext.RayGraphics.Graphics3D.Geometry;
using SelfGraphicsNext.RayGraphics.Graphics3D.Rendering;
using SFML.Graphics;
using SFML.Window;

namespace SelfGraphicsNext
{
    class Run
    {
        public static Scene scene;
        public static RenderWindow _rw;
        public static Camera3 camera;
        public static int k = 200;
        public static int x = 1 * k;
        public static int y = 1 * k;
        public const int mRatio = 8;
        public static Task process;
        public static bool deb = false;
        static bool benchMode = true;
        static bool drawInfo = false;
        public static void Main(string[] args)
        {
            scene = Scene.LoadSceneObj(@"C:\Users\Mical\Projects\BlenderProjects\outs\Tests\ReShadowTest.obj");
            scene.Light = new Point3(0, 2, 2);
            camera = new Camera3(new Viewer(70, new Point3(-10, 0, 0), new Direction3(0, 0), x, y));
            _rw = new RenderWindow(new VideoMode((uint)x, (uint)y), "SGN test", Styles.Close);
            _rw.Closed += (o, e) => _rw.Close();
            _rw.SetActive(true);
            _rw.Display();
            _rw.SetFramerateLimit(60);
            _rw.KeyReleased += _rw_KeyReleased;
            camera.RenderSceneMulti(scene, mRatio, false); 
            Font font = new Font("GangSmallYuxian-Rpep6.ttf");
            drawInfo = false;
            TimeSpan record = TimeSpan.FromSeconds(5);
            while (_rw.IsOpen)
            {
                _rw.Clear();
                _rw.DispatchEvents();
                {
                    _rw_MouseMoved();
                    {
                        _rw.Draw(new Sprite(new Texture(camera.Rendering.OutputImage)));
                        if (drawInfo)
                        {
                            string info = $"Resolution: {camera.ViewState.Width}x{camera.ViewState.Height}\n" +
                                $"Rendering: {camera.Rendering.TotalPixels} / {camera.Rendering.RenderedPixels}\n" +
                                $"% : {Math.Round((decimal)camera.Rendering.RenderedPixels / (decimal)camera.Rendering.TotalPixels * (decimal)100, 3)}\n" +
                                $"Render time: {camera.Rendering.RenderTime}\n" +
                                $"State: {Enum.GetName(camera.Rendering.State)}";
                            Text infoText = new Text(info, font, 18);
                            _rw.Draw(infoText);
                        }
                        if (benchMode)
                        {
                            if (camera.Rendering.State == RenderState.Ready)
                            {
                                if (record > camera.Rendering.RenderTime)
                                {
                                    Console.WriteLine($"New Record: {camera.Rendering.RenderTime.ToString("s','ffff")}");
                                    record = camera.Rendering.RenderTime;
                                }
                                else
                                {
                                    Console.WriteLine(camera.Rendering.RenderTime.ToString("s','ffff"));
                                }
                                //File.AppendAllText("timeLog.txt", camera.Rendering.RenderTime.ToString("s','ffff") + "\n");
                                camera.RenderSceneMulti(scene, mRatio);
                            }
                        }
                        if (Keyboard.IsKeyPressed(Keyboard.Key.Escape))
                            _rw.Close();
                    }
                }
                _rw.Display();
            }

            static void _rw_MouseMoved()
            {

                void updateImg()
                {
                    camera.RenderSceneMulti(scene, mRatio);
                }
                //if (Keyboard.IsKeyPressed(Keyboard.Key.Up))
                //{
                //    while (Keyboard.IsKeyPressed(Keyboard.Key.Up))
                //        continue;
                //    camera.ViewState.Direction.Vertical += 10;
                //    updateImg();
                //}
                //if (Keyboard.IsKeyPressed(Keyboard.Key.Down))
                //{
                //    while (Keyboard.IsKeyPressed(Keyboard.Key.Down))
                //        continue;
                //    camera.ViewState.Direction.Vertical -= 10;
                //    updateImg();
                //}
                //if (Keyboard.IsKeyPressed(Keyboard.Key.Left))
                //{
                //    while (Keyboard.IsKeyPressed(Keyboard.Key.Left))
                //        continue;
                //    camera.ViewState.Direction.Horisontal -= 10;
                //    updateImg();
                //}
                //if (Keyboard.IsKeyPressed(Keyboard.Key.Right))
                //{
                //    while (Keyboard.IsKeyPressed(Keyboard.Key.Right))
                //        continue;
                //    camera.ViewState.Direction.Horisontal += 10;
                //    updateImg();
                //}
                //if (Keyboard.IsKeyPressed(Keyboard.Key.W))
                //{
                //    while (Keyboard.IsKeyPressed(Keyboard.Key.W))
                //        continue;
                //    camera.ViewState.Position.X += 1;
                //    updateImg();
                //}
                //if (Keyboard.IsKeyPressed(Keyboard.Key.S))
                //{
                //    while (Keyboard.IsKeyPressed(Keyboard.Key.S))
                //        continue;
                //    camera.ViewState.Position.X -= 1;
                //    updateImg();
                //}
                //if (Keyboard.IsKeyPressed(Keyboard.Key.A))
                //{
                //    while (Keyboard.IsKeyPressed(Keyboard.Key.A))
                //        continue;
                //    camera.ViewState.Position.Y -= 1;
                //    updateImg();
                //}
                //if (Keyboard.IsKeyPressed(Keyboard.Key.D))
                //{
                //    while (Keyboard.IsKeyPressed(Keyboard.Key.D))
                //        continue;
                //    camera.ViewState.Position.Y += 1;
                //    updateImg();
                //}
                //if (Keyboard.IsKeyPressed(Keyboard.Key.Q))
                //{
                //    while (Keyboard.IsKeyPressed(Keyboard.Key.Q))
                //        continue;
                //    camera.ViewState.Position.Z += 1;
                //    updateImg();
                //}
                //if (Keyboard.IsKeyPressed(Keyboard.Key.E))
                //{
                //    while (Keyboard.IsKeyPressed(Keyboard.Key.E))
                //        continue;
                //    camera.ViewState.Position.Z -= 1;
                //    updateImg();
                //}
                if (Keyboard.IsKeyPressed(Keyboard.Key.R))
                {
                    while (Keyboard.IsKeyPressed(Keyboard.Key.R))
                        continue;
                    updateImg();
                }
            }
        }

        private static void _rw_KeyReleased(object? sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.F1)
                drawInfo = !drawInfo;
        }
    }
}
