using TQS.Metadata;
using TQS.Drawing2D;
using TQS.Geometry2D;
using System.Collections.Generic;

namespace TestProj
{
    [Header("Calculadora de seção")]
    public class Calculator
    {
        [Group("Grupo")]
        public double bw { get; set; }

        [Group("Grupo")]
        public double h { get; set; }

        public List<double> bitolas { get; set; }

        [Header("Área")][ReadOnly][Group("Grupo")]
        public double Area { get { return bw * h; } }

        [ReadOnly][Group("Grupo")]
        public double As { get; set; }

        public IDrawable drawing
        {
            get
            {
                DrawableBasic result = new DrawableBasic();
                PolyLine poly = new PolyLine() { Points = new List<Point>() { new Point(0, 0), new Point(0, h), new Point(bw, h), new Point(bw, 0), new Point(0, 0) } };
                result.Add(new DrawingPolyLine(poly) { ColorMode = ColorMode.Native, Layer = 1, LineColor = 1 });
                return result;
            }
        }

        [Header("Calcular")]
        [Help("Calcula a área")]
        public void Calculate()
        {
            As = 0.2 * Area;
        }
    }
}