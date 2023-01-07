using SelfGraphicsNext.RayGraphics.Graphics3D.Geometry;
using SFML.Graphics;
using System.Globalization;

namespace SelfGraphicsNext.RayGraphics.Graphics3D.Rendering
{

    class MtlFile
    {
        Dictionary<string, Color> mtlColors;
        public MtlFile(string filename)
        {
            string colorName = "";
            mtlColors= new Dictionary<string, Color>();
            foreach (var str in File.ReadAllLines(filename))
            {
                var coms = str.Split(' ');
                switch(coms[0])
                {
                    case "newmtl":
                        colorName = coms[1];
                        break;
                    case "Kd":
                        var vecCol = Point3.ParceVector(coms[1], coms[2], coms[3]) * Byte.MaxValue;
                        mtlColors.Add(colorName, new Color((byte)vecCol.X, (byte)vecCol.Y, (byte)vecCol.Z));
                        break;
                }
            }
        }
        public Color this[string name] => mtlColors[name];
    }
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
