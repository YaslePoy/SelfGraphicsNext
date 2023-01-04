using SelfGraphicsNext.BaseGraphics;
using SelfGraphicsNext.Properties;
using SelfGraphicsNext.RayGraphics.Graphics3D.Geometry;
using SelfGraphicsNext.RayGraphics.Graphics3D.Rendering;
using SFML.Graphics;
using SFML.Window;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Text.Json.Serialization;
using static SFML.Graphics.Font;
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
        static bool drawInfo = true;
        public static bool animation = false;
        static RunConfig RunConfig;
        static string rgPath;
        static Direction localLightDirection;
        static bool lockMouse = false;
        static double mouseRatio;
        public static void Main(string[] args)
        {
            localLightDirection = new Direction(0);
            Console.WriteLine(args[0]);
            rgPath = args[0];
            RunConfig = JsonSerializer.Deserialize<RunConfig>(File.ReadAllText(rgPath));
            var cfg = RunConfig.Get();
            scene = cfg.scene;
            camera = cfg.camera;
            if (RunConfig.UseCuda)
                scene.LoadModel();
            _rw = new RenderWindow(new VideoMode((uint)RunConfig.XResolution, (uint)RunConfig.YResolution), "SGN", Styles.None);
            _rw.Closed += (o, e) => _rw.Close();
            _rw.SetActive(true);
            _rw.Display();
            _rw.SetFramerateLimit(60);
            _rw.KeyReleased += _rw_KeyReleased;
            _rw.SetFramerateLimit(60);
            //_rw.MouseMoved += _rw_MouseMoved;
            Font font = new Font("GangSmallYuxian-Rpep6.ttf");
            TimeSpan record = TimeSpan.MaxValue;
            int recordCounter = 20;
            if (RunConfig.UseCuda)
                camera.RenderSceneCUDA(scene);
            else
                camera.RenderSceneMulti(scene, 16, false);
            while (_rw.IsOpen)
            {
                _rw.Clear();
                _rw.DispatchEvents();
                {
                    _rwKeyboard();
                    {
                        if (animation)
                        {
                            localLightDirection = 130;
                            //camera.ViewState.Position = new Point3(localLightDirection.Cos, localLightDirection.Sin, 0) * 4;
                            //camera.ViewState.Direction = new Direction3(-localLightDirection.AngleGrads, 0);
                            scene.Light = new Point3(localLightDirection.Cos, localLightDirection.Sin, 1) * 2;
                            camera.RenderSceneCUDA(scene);

                        }
                        if (drawInfo)
                        {
                            string info = $"Resolution: {camera.ViewState.Width}x{camera.ViewState.Height}\n" +
                                //$"Rendering: {camera.Rendering.TotalPixels} / {camera.Rendering.RenderedPixels}\n" +
                                //$"% : {Math.Round((decimal)camera.Rendering.RenderedPixels / (decimal)camera.Rendering.TotalPixels * (decimal)100, 3)}\n" +
                                //$"Render time: {camera.Rendering.RenderTime}\n" +
                                //$"State: {Enum.GetName(camera.Rendering.State)}";
                                $"Posision: {camera.ViewState.Position}\n" +
                                $"Veiw: {camera.ViewState.Direction}\n";
                            Text infoText = new Text(info, font, 18);
                            _rw.Draw(infoText);
                        }
                        if (benchMode)
                        {
                            if (camera.Rendering.State == RenderState.Ready)
                            {
                                var timeStr = camera.Rendering.RenderTime.ToString("s','ffff");
                                if (record > camera.Rendering.RenderTime)
                                {
                                    Console.WriteLine($"New Record: {timeStr}({MathF.Round(1000f / camera.Rendering.RenderTime.Milliseconds, 1)}FPS)");
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
                                if (RunConfig.UseCuda)
                                    camera.RenderSceneCUDA(scene);
                                else
                                    camera.RenderSceneMulti(scene, 16, true);
                            }

                        }
                        _rw.Draw(new Sprite(new Texture(camera.Rendering.OutputImage)));

                        if (Keyboard.IsKeyPressed(Keyboard.Key.Escape))
                            _rw.Close();
                    }
                }
                _rw.Display();


            }
            Console.ReadKey();
            static void _rwKeyboard()
            {
                if (benchMode)
                    return;
                bool update = false;
                void updateImg()
                {
                    //camera.ViewState.UpdateDirectionList();
                    camera.RenderSceneCUDA(scene, wait:true);
                    string info = $"Resolution: {camera.ViewState.Width}x{camera.ViewState.Height}\n" +
                                 //$"Rendering: {camera.Rendering.TotalPixels} / {camera.Rendering.RenderedPixels}\n" +
                                 //$"% : {Math.Round((decimal)camera.Rendering.RenderedPixels / (decimal)camera.Rendering.TotalPixels * (decimal)100, 3)}\n" +
                                 //$"Render time: {camera.Rendering.RenderTime}\n" +
                                 //$"State: {Enum.GetName(camera.Rendering.State)}";
                                 $"Posision: {camera.ViewState.Position}\n" +
                                 $"Veiw: {camera.ViewState.Direction}\n" +
                                 $"Time: {camera.Rendering.RenderTime}\n";
                    Console.WriteLine(info);
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.Up))
                {
                    //while (Keyboard.IsKeyPressed(Keyboard.Key.Up))
                    //    continue;
                    camera.ViewState.Direction.AddVertical(1);
                    update = true;
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.Down))
                {
                    //while (Keyboard.IsKeyPressed(Keyboard.Key.Down))
                    //    continue;
                    camera.ViewState.Direction.AddVertical(-1);
                    update = true;
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.Left))
                {
                    //while (Keyboard.IsKeyPressed(Keyboard.Key.Left))
                    //    continue;
                    camera.ViewState.Direction.AddHorisontal(-1);
                    update = true;
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.Right))
                {
                    //while (Keyboard.IsKeyPressed(Keyboard.Key.Right))
                    //    continue;
                    camera.ViewState.Direction.AddHorisontal(1);
                    update = true;
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.W))
                {
                    //while (Keyboard.IsKeyPressed(Keyboard.Key.W))
                    //    continue;
                    camera.ViewState.Position += camera.ViewState.Direction.GetVector() * 0.1;
                    update = true;
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.S))
                {
                    //while (Keyboard.IsKeyPressed(Keyboard.Key.S))
                    //    continue;
                    camera.ViewState.Position -= camera.ViewState.Direction.GetVector() * 0.1;
                    update = true;
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.A))
                {
                    //while (Keyboard.IsKeyPressed(Keyboard.Key.A))
                    //    continue;
                    camera.ViewState.Position += (camera.ViewState.Direction + new Direction3(-90, 0)).GetVector() * 0.1;
                    update = true;
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.D))
                {
                    //while (Keyboard.IsKeyPressed(Keyboard.Key.D))
                    //    continue;
                    camera.ViewState.Position += (camera.ViewState.Direction + new Direction3(90, 0)).GetVector() * 0.1;

                    update = true;
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.Q))
                {
                    //while (Keyboard.IsKeyPressed(Keyboard.Key.Q))
                    //    continue;
                    camera.ViewState.Position.Z += 0.1;
                    update = true;
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.E))
                {
                    //while (Keyboard.IsKeyPressed(Keyboard.Key.E))
                    //    continue;
                    camera.ViewState.Position.Z -= 0.1;
                    update = true;
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.R))
                {
                    //while (Keyboard.IsKeyPressed(Keyboard.Key.R))
                    //    continue;
                    update = true;
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
                    _rw = new RenderWindow(new VideoMode((uint)RunConfig.XResolution, (uint)RunConfig.YResolution), "Very good render picture", Styles.None);
                    _rw.Closed += (o, e) => _rw.Close();
                    _rw.SetActive(true);
                    _rw.Display();
                    _rw.SetFramerateLimit(60);
                    _rw.KeyReleased += _rw_KeyReleased;
                    if (RunConfig.UseCuda)
                        camera.RenderSceneCUDA(scene);
                    else
                        camera.RenderSceneMulti(scene, 16, false);
                }
                //if (Mouse.IsButtonPressed(Mouse.Button.Left))
                //{
                //    while (Mouse.IsButtonPressed(Mouse.Button.Left)) ;
                //    lockMouse = !lockMouse;
                //    _rw.SetMouseCursorVisible(!lockMouse);
                //    Mouse.SetPosition(new SFML.System.Vector2i(RunConfig.XResolution, RunConfig.YResolution) / 2, _rw);
                //}
                if (update)
                    updateImg();
            }
        }

        private static void _rw_MouseMoved(object? sender, MouseMoveEventArgs e)
        {
            if (!lockMouse)
                return;
            var basePos = new SFML.System.Vector2i(RunConfig.XResolution, RunConfig.YResolution) / 2;
            var delta = basePos - new SFML.System.Vector2i(e.X, e.Y);
            if (delta.X > 0 || delta.Y > 0)
            {
                camera.ViewState.Direction += new Direction3(delta.X, delta.Y);
                camera.ViewState.UpdateDirectionList();
                Mouse.SetPosition(basePos, _rw);
                camera.RenderSceneCUDA(scene);
            }

        }

        private static void _rw_KeyReleased(object? sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.F1)
                drawInfo = !drawInfo;
        }
    }
}