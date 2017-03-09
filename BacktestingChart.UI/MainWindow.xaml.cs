using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BacktestingChart.DataAccess;
using BacktestingChart.UI.Control.ChartsControl;

namespace BacktestingChart.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ChartStyleGridlines cs;
        private DataCollection dc = new DataCollection();
        private DataSeries ds = new DataSeries();
        public MainWindow()
        {
            InitializeComponent();
            //AddChart();
            //AddSimpleChart();
        }
        //private void AddChart()
        //{
        //    cs = new ChartStyleGridlines();
        //    cs.ChartCanvas = chartCanvas;
        //    cs.TextCanvas = textCanvas;
        //    cs.Title = "Chart";
        //    cs.Xmin = 0;
        //    cs.Xmax = 7;
        //    cs.Ymin =0;
        //    cs.Ymax = 5;
        //    cs.YTick = 0.5;
        //    cs.GridlinePattern =
        //    ChartStyleGridlines.GridlinePatternEnum.Dot;
        //    cs.GridlineColor = Brushes.Black;
        //    cs.AddChartStyle(tbTitle, tbXLabel, tbYLabel);
        //    // Draw Sine curve: 
        //    ds.LineColor = Brushes.Blue;
        //    ds.LineThickness = 3;
        //    for (int i = 0; i < 70; i++)
        //    {
        //        double x = i;
        //        double y = x;
        //        ds.LineSeries.Points.Add(new Point(x, y));
        //    }
        //    dc.DataList.Add(ds);
        //    // Draw cosine curve: 
        //    ds = new DataSeries();
        //    ds.LineColor = Brushes.Red;
        //    ds.LinePattern =
        //    DataSeries.LinePatternEnum.DashDot;
        //    ds.LineThickness = 3;
        //    for (int i = 0; i < 50; i++)
        //    {
        //        double x = i;
        //        double y = x;
        //        ds.LineSeries.Points.Add(new Point(x, y));
        //    }
        //    dc.DataList.Add(ds);
        //    dc.AddLines(chartCanvas, cs);
        //}

        //private void AddSimpleChart()
        //{
        //    List<CurrencyInfos> book=LoadCurrencyRates("book.csv");
        //    cs = new ChartStyleGridlines();
        //    cs.ChartCanvas = chartCanvas;
        //    cs.TextCanvas = textCanvas;
        //    cs.Title = "Chart";
        //    cs.Xmin = 0;
        //    cs.Xmax = 89;
        //    cs.Ymin = 0;                      
        //    cs.Ymax = 89;
        //    cs.YTick = 0.5;
        //    cs.GridlinePattern =
        //    ChartStyleGridlines.GridlinePatternEnum.Dot;
        //    cs.GridlineColor = Brushes.Black;
        //    cs.AddChartStyle(tbTitle, tbXLabel, tbYLabel);
        //    // Draw Sine curve: 
        //    ds.LineColor = Brushes.Blue;
        //    ds.LineThickness = 3;
        //    for (int i = 0; i < 89; i++)
        //    {
        //        long x = book[i].CurrentTime;
        //        double y = book[i].Price;
        //        ds.LineSeries.Points.Add(new Point(x, y));
        //    }
        //    dc.DataList.Add(ds);
        //    // Draw cosine curve: 
           
        //    dc.DataList.Add(ds);
        //    dc.AddLines(chartCanvas, cs);
        //}
        List<CurrencyInfos> book;
        
        private static List<CurrencyInfos> LoadCurrencyRates(string fileName)
        {
            string[] strings = File.ReadAllLines(fileName);

            var res = new List<CurrencyInfos>(strings.Length - 1);
            //for (int i = 1; i < strings.Length; i++)
            for (int i = 1; i < 90; i++)
            {
                string line = strings[i];
                //string[] subLines = line.Split('\t');
                string[] subLines = line.Split(',');
                long date = long.Parse(subLines[0]);
                double price =int.Parse(subLines[4]);

                res.Add(new CurrencyInfos { CurrentTime = date, Price = price });
            }

            return res;
        }
        internal class CurrencyInfos
        {
            public long CurrentTime { get; set; }
            public double Price { get; set; }
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            book = LoadCurrencyRates("book.csv");

            

            Symbols symbols=new Symbols();            
            symbols.Show();
        }
    }
}
