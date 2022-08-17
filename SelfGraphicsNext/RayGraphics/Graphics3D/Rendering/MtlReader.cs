using SFML.Graphics;
using System.Globalization;

namespace SelfGraphicsNext.RayGraphics.Graphics3D.Rendering
{
    public class MtlReader
    {
        public static Color GetColor(string mtlPath, string materialName)
        {
            bool isCurrMaterial = false;
            if (!File.Exists(mtlPath))
                return Color.White;
            foreach (var str in File.ReadAllLines(mtlPath))
            {
                if (str == $"newmtl {materialName}")
                    isCurrMaterial = true;
                if (isCurrMaterial && str.Contains("Kd"))
                {
                    var col = str.Split(' ').Skip(1).Take(3).Select(i => double.Parse(i, CultureInfo.InvariantCulture)).ToList();
                    return new Color((byte)(col[0] * 128),
                        (byte)(col[1] * 128),
                        (byte)(col[2] * 128));
                }

            }
            return new Color();
        }
    }
}
