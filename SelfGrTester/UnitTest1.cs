using Microsoft.VisualStudio.TestTools.UnitTesting;
using SelfGraphicsNext.RayGraphics.Graphics3D.Geometry;
using System;

namespace SelfGrTester
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void AngleTest50x30()
        {
            Direction3 startDir = new Direction3(150, 30);
            var vec = startDir.GetVector();
            Direction3 nextDir = new Direction3();
            nextDir.SetDirection(vec);
            var nextVec = nextDir.GetVector();
            Assert.AreEqual(vec.ToString(), nextVec.ToString());
        }
        [TestMethod]
        public void AngleTest50X100()
        {
            Direction3 startDir = new Direction3(150, 100);
            var vec = startDir.GetVector();
            Direction3 nextDir = new Direction3();
            nextDir.SetDirection(vec);
            var nextVec = nextDir.GetVector();
            Assert.AreEqual(vec.ToString(), nextVec.ToString());
        }
        [TestMethod]
        public void AngleTest50X200()
        {
            Direction3 startDir = new Direction3(150, 200);
            var vec = startDir.GetVector();
            Direction3 nextDir = new Direction3();
            nextDir.SetDirection(vec);
            var nextVec = nextDir.GetVector();
            Assert.AreEqual(vec.ToString(), nextVec.ToString());
        }
        [TestMethod]
        public void AngleTest50X300()
        {
            Direction3 startDir = new Direction3(150, 300);
            var vec = startDir.GetVector();
            Direction3 nextDir = new Direction3();
            nextDir.SetDirection(vec);
            var nextVec = nextDir.GetVector();
            Assert.AreEqual(vec.ToString(), nextVec.ToString());
        }
    }
}