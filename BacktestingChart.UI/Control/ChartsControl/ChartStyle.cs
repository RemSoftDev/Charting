using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BacktestingChart.UI.Control.ChartsControl
{
    class ChartStyle
    {
       
        public Canvas ChartCanvas { get; set; }

        public double Xmin { get; set; } = 0;

        public double Xmax { get; set; } = 1;

        public double Ymin { get; set; } = 0;

        public double Ymax { get; set; } = 1;

        public ChartStyle()
        {
        }
        public void ResizeCanvas(double width, double height)
        {
            ChartCanvas.Width = width;
            ChartCanvas.Height = height;
        }
        public Point NormalizePoint(Point pt)
        {
            if (ChartCanvas.Width.ToString() == "NaN")
                ChartCanvas.Width = 270;
            if (ChartCanvas.Height.ToString() == "NaN")
                ChartCanvas.Height = 250;
            Point result = new Point();
            result.X = (pt.X - Xmin) *
            ChartCanvas.Width / (Xmax - Xmin);
            result.Y = ChartCanvas.Height-(pt.Y - Ymin)
                *ChartCanvas.Height / (Ymax - Ymin);
            return result;
        }
    }
}
