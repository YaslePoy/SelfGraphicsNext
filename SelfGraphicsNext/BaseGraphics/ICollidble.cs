using SelfGraphicsNext.RayGraphics.Graphics2D;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfGraphicsNext.BaseGraphics
{
    public interface ICollidble
    {
        public Point? Collide(Ray ray);
        public void Draw(RenderWindow win);

    }
}
