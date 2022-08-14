using SelfGraphicsNext.BaseGraphics;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfGraphicsNext.RayGraphics.Graphics3D.Rendering
{
    public enum RenderState
    {
        Prepairing,
        Active,
        Stopped,
        Ready
    }
    public class RenderData
    {
        public RenderState State;
        public readonly int TotalPixels;
        public int RenderedPixels { get => rendPixels; }
        int rendPixels;
        public Image OutputImage { get => image; }
        Image image;
        public TimeSpan RenderTime => timer == null? TimeSpan.Zero : timer.Elapsed;
        Stopwatch timer;
        
        public RenderData(int xRes, int yRes)
        {
            State = RenderState.Prepairing;
            image = new Image((uint)xRes, (uint)yRes);
            TotalPixels = xRes * yRes;
        }
        public void SetPixel(Point pixelXY, Color color)
        {
            if (State != RenderState.Active)
                throw new Exception("This method should be used just when State is Active");
            image.SetPixel((uint)pixelXY.X, (uint)pixelXY.Y, color);
            rendPixels += 1;
            if (rendPixels == TotalPixels)
            {
                State = RenderState.Ready;
            }
        }
        public void Start()
        {
            State = RenderState.Active;
            timer = Stopwatch.StartNew();
        }
        public void Stop() => State = RenderState.Stopped;
        public void Clear()
        {
            image = new Image(image.Size.X, image.Size.Y);
            timer.Reset();
            State = RenderState.Stopped;
            rendPixels = 0;
        }
    }
}
