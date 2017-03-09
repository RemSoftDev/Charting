using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BacktestingChart.UI.Control.ChartsControl
{
    class DataSeries
    {
        public enum LinePatternEnum
        {
            Solid = 1,
            Dash = 2,
            Dot = 3,
            DashDot = 4,
            None = 5
        }

        public DataSeries()
        {
            LineColor = Brushes.Black;
        }
        public Brush LineColor { get; set; }

        public Polyline LineSeries { get; set; } = new Polyline();

        public double LineThickness { get; set; } = 1;

        public LinePatternEnum LinePattern { get; set; }

        public string SeriesName { get; set; } = "Default Name";

        public void AddLinePattern()
        {
            LineSeries.Stroke = LineColor;
            LineSeries.StrokeThickness = LineThickness;
            switch (LinePattern)
            {
                case LinePatternEnum.Dash:
                    LineSeries.StrokeDashArray =
                    new DoubleCollection(                   
                    new double[2] { 4, 3 });
                    break;
            }
        }
    }
}
