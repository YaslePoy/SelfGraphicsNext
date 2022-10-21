using SelfGraphicsNext.BaseGraphics;
using SelfGraphicsNext.RayGraphics.Graphics3D.Geometry;
using SelfGraphicsNext.RayGraphics.Graphics3D.Rendering;
using SFML.Graphics;
using SFML.Window;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Text.Json.Serialization;
using Font = SFML.Graphics.Font;

namespace SelfGraphicsNext
{
    class Run
    {
        public static Scene scene;
        public static RenderWindow _rw;
        public static Camera3 camera;
        public const int mRatio = 16;
        public static Task process;
        public static bool deb = false;
        static bool benchMode = false;
        static bool drawInfo = false;
        static RunConfig RunConfig;
        static string rgPath;
        static Direction localLightDirection;
        public static void Main(string[] args)
        {
            localLightDirection = new Direction(0);
            Console.WriteLine(args[0]);
            rgPath = args[0];
            RunConfig = JsonSerializer.Deserialize<RunConfig>(File.ReadAllText(rgPath));
            var cfg = RunConfig.Get();
            scene = cfg.scene;
            camera = cfg.camera;
            scene.LoadModel();
            _rw = new RenderWindow(new VideoMode((uint)RunConfig.XResolution, (uint)RunConfig.YResolution), "SGN", Styles.None);
            _rw.Closed += (o, e) => _rw.Close();
            _rw.SetActive(true);
            _rw.Display();
            _rw.SetFramerateLimit(60);
            _rw.KeyReleased += _rw_KeyReleased;
            //camera.RenderSceneMulti(scene, mRatio, false);
            Font font = new Font("GangSmallYuxian-Rpep6.ttf");
            drawInfo = false;
            TimeSpan record = TimeSpan.MaxValue;
            int recordCounter = 20;
            //camera.RenderSceneCUDA(scene);
            camera.RenderSceneCUDA(scene);
            while (_rw.IsOpen)
            {
                _rw.Clear();
                _rw.DispatchEvents();
                {
                    _rw_MouseMoved();
                    {
                        //scene.Light = new Point3(localLightDirection.Cos, localLightDirection.Sin, 1) * 2;
                        //localLightDirection += 2;
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
                                recordCounter--;
                                var timeStr = camera.Rendering.RenderTime.ToString("s','ffff");
                                if (record > camera.Rendering.RenderTime)
                                {
                                    Console.WriteLine($"New Record: {timeStr}");
                                    record = camera.Rendering.RenderTime;
                                    recordCounter = 20;
                                }
                                else
                                {
                                    if (recordCounter == 0)
                                    {
                                        Console.WriteLine($"{timeStr} Record steel {record.ToString("s','ffff")}");
                                        recordCounter = 20;
                                    }
                                    //else
                                    //    Console.WriteLine(timeStr);
                                }
                                RunConfig.SceneLight = new EasyPoint() { X = localLightDirection.Cos * 2, Y = localLightDirection.Sin, Z = 2 };
                                scene.Light = new Point3(localLightDirection.Cos, localLightDirection.Sin, 1) * 2;
                                localLightDirection += 5;
                                camera.RenderSceneMulti(scene, mRatio);
                            }
                        }
                        if (Keyboard.IsKeyPressed(Keyboard.Key.Escape))
                            _rw.Close();
                    }
                }
                _rw.Display();
            }
            Console.ReadKey();
            static void _rw_MouseMoved()
            {
                void updateImg()
                {
                    if (camera.Rendering.State != RenderState.Active)
                        camera.RenderSceneMulti(scene, mRatio);
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.Up))
                {
                    while (Keyboard.IsKeyPressed(Keyboard.Key.Up))
                        continue;
                    camera.ViewState.Direction.Vertical += 10;
                    updateImg();
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.Down))
                {
                    while (Keyboard.IsKeyPressed(Keyboard.Key.Down))
                        continue;
                    camera.ViewState.Direction.Vertical -= 10;
                    updateImg();
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.Left))
                {
                    while (Keyboard.IsKeyPressed(Keyboard.Key.Left))
                        continue;
                    camera.ViewState.Direction.Horisontal -= 10;
                    updateImg();
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.Right))
                {
                    while (Keyboard.IsKeyPressed(Keyboard.Key.Right))
                        continue;
                    camera.ViewState.Direction.Horisontal += 10;
                    updateImg();
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.W))
                {
                    while (Keyboard.IsKeyPressed(Keyboard.Key.W))
                        continue;
                    //camera.ViewState.Position.X += 1;
                    camera.ViewState.Position += camera.ViewState.Direction.GetVector();
                    updateImg();
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.S))
                {
                    while (Keyboard.IsKeyPressed(Keyboard.Key.S))
                        continue;
                    camera.ViewState.Position -= camera.ViewState.Direction.GetVector();
                    updateImg();
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.A))
                {
                    while (Keyboard.IsKeyPressed(Keyboard.Key.A))
                        continue;
                    camera.ViewState.Position += (camera.ViewState.Direction + new Direction3(-90, 0)).GetVector();
                    updateImg();
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.D))
                {
                    while (Keyboard.IsKeyPressed(Keyboard.Key.D))
                        continue;
                    camera.ViewState.Position += (camera.ViewState.Direction + new Direction3(90, 0)).GetVector();

                    updateImg();
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.Q))
                {
                    while (Keyboard.IsKeyPressed(Keyboard.Key.Q))
                        continue;
                    camera.ViewState.Position.Z += 1;
                    updateImg();
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.E))
                {
                    while (Keyboard.IsKeyPressed(Keyboard.Key.E))
                        continue;
                    camera.ViewState.Position.Z -= 1;
                    updateImg();
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.R))
                {
                    while (Keyboard.IsKeyPressed(Keyboard.Key.R))
                        continue;
                    updateImg();
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.F5))
                {
                    while (Keyboard.IsKeyPressed(Keyboard.Key.F5))
                        continue;
                    RunConfig = JsonSerializer.Deserialize<RunConfig>(File.ReadAllText(rgPath));
                    var cfg = RunConfig.Get();
                    camera.Rendering.Stop();
                    scene = cfg.scene;
                    camera = cfg.camera;
                    _rw.Close();
                    _rw.Dispose();
                    _rw = new RenderWindow(new VideoMode((uint)RunConfig.XResolution, (uint)RunConfig.YResolution), "SGN test", Styles.None);
                    _rw.Closed += (o, e) => _rw.Close();
                    _rw.SetActive(true);
                    _rw.Display();
                    _rw.SetFramerateLimit(60);
                    _rw.KeyReleased += _rw_KeyReleased;
                    //camera.RenderSceneCUDA(scene);
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