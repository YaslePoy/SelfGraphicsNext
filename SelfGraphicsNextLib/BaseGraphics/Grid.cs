using SFML.Graphics;
using System.Globalization;
using System.Text.Json;
using System.Xml;

namespace SelfGraphicsNext.BaseGraphics
{
    public class Grid
    {

        public List<List<Point>> Layers;

        int width, height;

        public Grid(int w = 0, int h = 0)
        {

            Layers = new List<List<Point>>() { new List<Point>(), new List<Point>() };
        }

        public Grid LoadGridJson(string strGrid) => JsonSerializer.Deserialize<Grid>(strGrid);

        public void LoadGridSVG(string strGrid)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(strGrid);
            XmlElement xml = xmlDoc.DocumentElement;
            List<XmlNode> list = new List<XmlNode>();
            foreach (XmlNode node in xml.ChildNodes)
            {
                if (node.Name == "g")
                    list.Add(node);
            }
            foreach (var layer in list)
            {
                if (!(Layers.Count - 1 >= list.IndexOf(layer)))
                    AddLayer();
                var prims = layer.ChildNodes;
                foreach (XmlNode node in prims)
                {
                    Color finalColor = Color.Yellow;
                    var style = node.Attributes["style"].Value;
                    var styleParams = style.Split(';').ToList().ToDictionary(t => t.Split(":")[0], t => t.Split(":")[1]);
                    if (styleParams["fill"] != "none")
                    {
                        var colorHex = styleParams["fill"].Substring(1);
                        var rK = byte.Parse(colorHex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                        var gK = byte.Parse(colorHex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                        var bK = byte.Parse(colorHex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                        finalColor = new Color(rK, gK, bK);
                    }

                    if (node.Name == "rect")
                    {
                        var x = double.Parse(node.Attributes["x"].Value.Replace('.', ','), NumberStyles.Float).Round();
                        var y = double.Parse(node.Attributes["y"].Value.Replace('.', ','), NumberStyles.Float).Round();
                        var h = double.Parse(node.Attributes["height"].Value.Replace('.', ','), NumberStyles.Float).Round();
                        var w = double.Parse(node.Attributes["width"].Value.Replace('.', ','), NumberStyles.Float).Round();
                        Reclangle reclangle = new Reclangle(x, y, w, h) { Color = finalColor };
                        AddDrawable(reclangle, list.IndexOf(layer) + 1);

                    }
                    if (node.Name == "circle")
                    {
                        var rad = double.Parse(node.Attributes["r"].Value).Round();
                        var x = double.Parse(node.Attributes["cx"].Value).Round();
                        var y = double.Parse(node.Attributes["cy"].Value).Round();

                        Circle circle = new Circle(x, y, rad) { Color = finalColor };
                        AddDrawable(circle, list.IndexOf(layer) + 1);

                    }
                    //if (node.Name == "path")
                    //{

                    //    var path = node.Attributes["d"].Value;
                    //    var mode = path[0];
                    //    path = path.Substring(2);
                    //    Point startPoint = new Point(-789, -789);
                    //    Point lastPoint = new Point(-789, -789);

                    //    foreach (var xy in path.Split(" "))
                    //    {
                    //        var strArr = xy.Split(",").ToList();
                    //        strArr.ForEach(i => i = i.Replace('.', ','));
                    //        var localPoint = new Point(double.Parse(strArr.First().Replace('.', ',')).Round(), double.Parse(strArr.Last().Replace('.', ',')).Round()) { Color = finalColor };
                    //        if (startPoint == new Point(-789, -789))
                    //        {
                    //            startPoint = localPoint;
                    //            continue;
                    //        }
                    //        var localLine = new Line(startPoint, localPoint) { Color = finalColor };
                    //        AddDrawable(localLine, list.IndexOf(layer) + 1);
                    //        startPoint = localPoint;

                    //    }
                    //}
                }
            }
        }

        public void Draw(RenderWindow output)
        {
            foreach (var layer in Layers)
            {
                foreach (var prim in layer)
                {
                    if(!(prim is null))
                    prim.Draw(output);
                }
            }
        }

        public void AddDrawable(Point drawable, int layer = 1) => Layers[layer].Add(drawable);

        public void AddLayer() => Layers.Add(new List<Point>());

        public void ClearGrid() => Layers = new List<List<Point>>() { new List<Point>(), new List<Point>() };

        public void ClearScene() => Layers.ForEach(x => Layers[Layers.IndexOf(x)] = Layers.IndexOf(x) == 0 ? x : new List<Point>());

        public void ClearLayer(int layer)
        {
            try
            {

            Layers[layer].Clear();
            }
            catch
            {

            }
        }
    }
}
