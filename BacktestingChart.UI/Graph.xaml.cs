using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DynamicDataDisplay.Markers.DataSources;
using Loya.Dameer;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts.Navigation;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;
using Npgsql;
using CompositeDataSource = Microsoft.Research.DynamicDataDisplay.DataSources.CompositeDataSource;

namespace BacktestingChart.UI
{
    /// <summary>
    /// Interaction logic for Graph.xaml
    /// </summary>
    public partial class Graph : Window
    {
        public Graph()
        {
            InitializeComponent();
            
        }
        
        //List<BidCurrencyInfo> bidbook;
        //List<AskCurrencyInfo> askbook;
        //List<AskQuCurrencyInfo> askQubook;
        //List<BidQuCurrencyInfo> _bidQubook;
        //List<CurrencyInfo> _allCurrencyInfos;
        private void Graf_Loaded(object sender, RoutedEventArgs e)
        {
            //bidbook = LoadCurrencyRates1("book.csv");
            //askbook = LoadCurrencyRates2("book.csv");
            //askQubook = LoadCurrencyRatesAsQu("book.csv");
            //_bidQubook = LoadCurrencyRatesBidQu("book.csv");                   
            //Color[] colors = ColorHelper.CreateRandomColors(4);
            //plotter.AddLineGraph(CreateCurrencyDataSourceBid(LoadAllCurrency("book.csv")), colors[0], 1, "BidPrice /Timestampt");
            //plotter.AddLineGraph(CreateCurrencyDataSourceAsk(LoadAllCurrency("book.csv")), colors[1], 1, "AskPrice / Timestampt");
            //plotterAsQu.AddLineGraph(CreateCurrencyDataSourceAskQu(LoadAllCurrency("book.csv")), colors[2], 1, "AskQuantity /Timestampt");
            //plotterBidQu.AddLineGraph(CreateCurrencyDataSourceBidQu(LoadAllCurrency("book.csv")), colors[3], 1, "BidQuantity /Timestampt");
            //plotter.AddLineGraph(CreateCurrencyDataSourceForBid(bidbook), colors[0], 1, "BidPrice /Timestampt");
            //plotter.AddLineGraph(CreateCurrencyDataSourceForAsk(askbook), colors[1], 1, "AskPrice / Timestampt");
            //plotterAsQu.AddLineGraph(CreateCurrencyDataSourceForAskQu(askQubook), colors[2], 1, "AskQuantity /Timestampt");
            //plotterBidQu.AddLineGraph(CreateCurrencyDataSourceForBidQu(_bidQubook), colors[3], 1, "BidQuantity /Timestampt");
        }
        //private EnumerableDataSource<BidCurrencyInfo> CreateCurrencyDataSourceForBid(List<BidCurrencyInfo> rates)
        //{
        //    EnumerableDataSource<BidCurrencyInfo> ds = new EnumerableDataSource<BidCurrencyInfo>(rates);          
        //    ds.SetXMapping(ci => (ci.CurrentTime));
        //    ds.SetYMapping(ci => ci.Price);           
        //    return ds;
        //}
        //private EnumerableDataSource<AskCurrencyInfo> CreateCurrencyDataSourceForAsk(List<AskCurrencyInfo> rates)
        //{
        //    EnumerableDataSource<AskCurrencyInfo> ds = new EnumerableDataSource<AskCurrencyInfo>(rates);            
        //    ds.SetXMapping(ci => (ci.CurrentTime));
        //    ds.SetYMapping(ci => ci.Price);
        //    return ds;
        //}
        //private EnumerableDataSource<AskQuCurrencyInfo> CreateCurrencyDataSourceForAskQu(List<AskQuCurrencyInfo> rates)
        //{
        //    EnumerableDataSource<AskQuCurrencyInfo> ds = new EnumerableDataSource<AskQuCurrencyInfo>(rates);
        //    //ds.SetXMapping(ci => dateAxis.ConvertToDouble(ci.CurrentTime));
        //    ds.SetXMapping(ci => (ci.CurrentTime));
        //    ds.SetYMapping(ci => ci.AskQuantity);
        //    return ds;
        //}
        //private EnumerableDataSource<BidQuCurrencyInfo> CreateCurrencyDataSourceForBidQu(List<BidQuCurrencyInfo> rates)
        //{
        //    EnumerableDataSource<BidQuCurrencyInfo> ds = new EnumerableDataSource<BidQuCurrencyInfo>(rates);
        //    //ds.SetXMapping(ci => dateAxis.ConvertToDouble(ci.CurrentTime));
        //    ds.SetXMapping(ci => (ci.CurrentTime));
        //    ds.SetYMapping(ci => ci.BidQuantity);
        //    return ds;
        //}
        //private static List<BidCurrencyInfo> LoadCurrencyRates1(string fileName)
        //{
        //    string[] strings = File.ReadAllLines(fileName);

        //    var res = new List<BidCurrencyInfo>(strings.Length - 1);
        //    //for (int i = 1; i < strings.Length; i++)
        //    for (int i = 1; i < strings.Length; i++)
        //    {
        //        string line = strings[i];
        //        //string[] subLines = line.Split('\t');
        //        string[] subLines = line.Split(',');
        //        if (subLines[1] != null & subLines[0] != null & subLines[1]!="" & subLines[1] != ",")
        //        {
        //            long date = long.Parse(subLines[0]);
        //            double price = Double.Parse(subLines[1],CultureInfo.InvariantCulture);
        //            res.Add(new BidCurrencyInfo { CurrentTime = date, Price = price });
        //        }              
        //    }
        //    return res;
        //}

        //private static List<AskCurrencyInfo> LoadCurrencyRates2(string fileName)
        //{
        //    string[] strings = File.ReadAllLines(fileName);

        //    var res = new List<AskCurrencyInfo>(strings.Length - 1);
        //    //for (int i = 1; i < strings.Length; i++)
        //    for (int i = 1; i < strings.Length; i++)
        //    {
        //        string line = strings[i];
        //        //string[] subLines = line.Split('\t');
        //        string[] subLines = line.Split(',');
        //        if (subLines[3] != null & subLines[0] != null & subLines[3] != "" & subLines[3] != ",")
        //        {
        //            long date = long.Parse(subLines[0]);
        //            double askprice = Double.Parse(subLines[3], CultureInfo.InvariantCulture);
        //            res.Add(new AskCurrencyInfo { CurrentTime = date, Price = askprice });
        //        }
        //    }
        //    return res;
        //}
        //private static List<AskQuCurrencyInfo> LoadCurrencyRatesAsQu(string fileName)
        //{
        //    string[] strings = File.ReadAllLines(fileName);

        //    var res = new List<AskQuCurrencyInfo>(strings.Length - 1);
        //    //for (int i = 1; i < strings.Length; i++)
        //    for (int i = 1; i < strings.Length; i++)
        //    {
        //        string line = strings[i];
        //        //string[] subLines = line.Split('\t');
        //        string[] subLines = line.Split(',');
        //        if (subLines[4] != null & subLines[0] != null & subLines[4]!= "" & subLines[4] != ",")
        //        {
        //            long date = long.Parse(subLines[0]);
        //            int askQu = int.Parse(subLines[4]);
        //            res.Add(new AskQuCurrencyInfo { CurrentTime = date, AskQuantity = askQu});
        //        }
        //    }
        //    return res;
        //}
        //private static List<BidQuCurrencyInfo> LoadCurrencyRatesBidQu(string fileName)
        //{
        //    string[] strings = File.ReadAllLines(fileName);        
        //    var res = new List<BidQuCurrencyInfo>(strings.Length - 1);
        //    //for (int i = 1; i < strings.Length; i++)
        //    for (int i = 1; i < strings.Length; i++)
        //    {
        //        string line = strings[i];
        //        //string[] subLines = line.Split('\t');
        //        string[] subLines = line.Split(',');
        //        if (subLines[2] != null & subLines[0] != null & subLines[2] != "" & subLines[3] != ",")
        //        {
        //            long date = long.Parse(subLines[0]);
        //            int bidQu = int.Parse(subLines[2]);
        //            res.Add(new BidQuCurrencyInfo { CurrentTime = date, BidQuantity = bidQu});
        //        }
        //    }
        //    return res;
        //}
        private EnumerableDataSource<CurrencyInfo> CreateCurrencyDataSourceBid(List<CurrencyInfo> rates)
        {
            EnumerableDataSource<CurrencyInfo> ds = new EnumerableDataSource<CurrencyInfo>(rates);
            ds.SetXMapping(ci => (ci.CurrentTime));
            ds.SetYMapping(ci => ci.BidPrice);
            return ds;
        }
        private EnumerableDataSource<CurrencyInfo> CreateCurrencyDataSourceAsk(List<CurrencyInfo> rates)
        {
            EnumerableDataSource<CurrencyInfo> ds = new EnumerableDataSource<CurrencyInfo>(rates);
            ds.SetXMapping(ci => (ci.CurrentTime));
            ds.SetYMapping(ci => ci.AskPrice);
            return ds;
        }
        private EnumerableDataSource<CurrencyInfo> CreateCurrencyDataSourceBidQu(List<CurrencyInfo> rates)
        {
            EnumerableDataSource<CurrencyInfo> ds = new EnumerableDataSource<CurrencyInfo>(rates);
            ds.SetXMapping(ci => (ci.CurrentTime));
            ds.SetYMapping(ci => ci.BidQuantity);
            return ds;
        }
        private EnumerableDataSource<CurrencyInfo> CreateCurrencyDataSourceAskQu(List<CurrencyInfo> rates)
        {
            EnumerableDataSource<CurrencyInfo> ds = new EnumerableDataSource<CurrencyInfo>(rates);
            ds.SetXMapping(ci => (ci.CurrentTime));
            ds.SetYMapping(ci =>Convert.ToDouble(ci.AskQuantity));
            return ds;
        }
        private static List<CurrencyInfo> LoadAllCurrency(string fileName)
        {
            string[] strings = File.ReadAllLines(fileName);
            var res = new List<CurrencyInfo>(strings.Length - 1);
          
            for (int i = 1; i < strings.Length; i++)
            {
                string line = strings[i];               
                string[] subLines = line.Split(',');
                if (subLines[2] != null & subLines[0] != null & subLines[2] != "" & subLines[3] != "," & subLines[4] != "," & subLines[4] != null & subLines[4] != "")
                {
                    long date = long.Parse(subLines[0]);
                    double bidPrice = Double.Parse(subLines[1], CultureInfo.InvariantCulture);
                    int bidQu = int.Parse(subLines[2]);
                    double askPrice = Double.Parse(subLines[3], CultureInfo.InvariantCulture);
                    decimal askQu = Decimal.Parse(subLines[4]);
                    res.Add(new CurrencyInfo { CurrentTime = date,BidPrice = bidPrice, BidQuantity = bidQu,AskPrice = askPrice,AskQuantity = askQu});
                }
            }
            return res;
        }
        LineAndMarker<ElementMarkerPointsGraph> chart;
        public IPointDataSource ds;
        private void dataSource(List<CurrencyInfo> rates)
        {
            
            EnumerableDataSource<CurrencyInfo> xDataSource=new EnumerableDataSource<CurrencyInfo>(rates);
            EnumerableDataSource<CurrencyInfo> yDataSource = new EnumerableDataSource<CurrencyInfo>(rates);            
            yDataSource.SetYMapping(y=>y.BidPrice);
            yDataSource.AddMapping(ShapeElementPointMarker.ToolTipTextProperty,y=>String.Format("BidPrice is {0}", y.BidPrice));
            xDataSource.SetXMapping(x=>x.CurrentTime);
            ds = new CompositeDataSource(xDataSource, yDataSource);
            chart = plotter.AddLineGraph(ds,
                new Pen(Brushes.LimeGreen, 1),
                new CircleElementPointMarker
                {
                    Size = 5,
                    Brush = Brushes.Red,
                    Fill = Brushes.Orange
                },
                new PenDescription("BidPrice"));
           
            plotter.Children.Add(new CursorCoordinateGraph());            
            plotter.FitToView();
        }
        private EnumerableDataSource<LoadSymbols1> CreateTest(List<LoadSymbols1> rates)
        {
            EnumerableDataSource< LoadSymbols1 > ds = new EnumerableDataSource<LoadSymbols1> (rates);
            ds.SetXMapping(ci => (ci.AskPrices[0]));
            ds.SetYMapping(ci => ci.BidPrices[0]);
            return ds;
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            //GetChart(dameer3, dameer1, graphlie.Text, Symbol.Text, false);
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".csv"; // Default file extension
                                    

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                //LoadAllCurrency(filename);
                Color[] colors = ColorHelper.CreateRandomColors(4);
                //plotter.AddLineGraph(CreateCurrencyDataSourceBid(LoadAllCurrency(filename)), colors[0], 1, "BidPrice /Timestampt");
                plotter.AddLineGraph(CreateCurrencyDataSourceAsk(LoadAllCurrency(filename)), colors[1], 1, "AskPrice / Timestampt");
                plotterAsQu.AddLineGraph(CreateCurrencyDataSourceAskQu(LoadAllCurrency(filename)), colors[2], 1, "AskQuantity /Timestampt");
                plotterBidQu.AddLineGraph(CreateCurrencyDataSourceBidQu(LoadAllCurrency(filename)), colors[3], 1, "BidQuantity /Timestampt");
                dataSource(LoadAllCurrency(filename));
                plotter.AddLineGraph(CreateTest(graphChart.ChartsList), colors[3], 2, "BidPrice1 /Timestampt");

            }

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (chart.MarkerGraph.DataSource != null)
            {
                chart.MarkerGraph.DataSource = null;
            }
            else
            {
                chart.MarkerGraph.DataSource = ds;
            }
        }


        private void MouseClick(object sender, MouseButtonEventArgs e)
        {
            if (chart.MarkerGraph.DataSource != null)
            {
                chart.MarkerGraph.DataSource = null;
            }
            else
            {
                chart.MarkerGraph.DataSource = ds;
            }
        }
               
        public class LoadSymbols1
        {
            public DateTime Time { get; set; }
            public string Symbol { get; set; }
            public string Currency { get; set; }
            public int Level { get; set; }
            public double[] BidPrices { get; set; } = new double[5];
            public int[] BidQuantities { get; set; } = new int[5];
            public int[] AskQuantities { get; set; } = new int[5];
            public double[] AskPrices { get; set; } = new double[5];
        }

        public class LoadArrows
        {
            public DateTime Time { get; set; }
            public string Symbol { get; set; }
            public double BidPrices { get; set; }
            public string Directive { get; set; }
            public string Name { get; set; }
        }
        public class GraphArrow
        {
            public List<LoadArrows> Arrowses = new List<LoadArrows>();
        }
        public class GraphChart
        {
            public List<LoadSymbols1> ChartsList = new List<LoadSymbols1>();
            int coutnt = 0;
        }
        public struct QueryReq
        {
            public DateTime TimeTo { get; set; }
            public DateTime TimeFrom { get; set; }
            public string Symbol { get; set; }
            public int NameFrom { get; set; }
            public int NCount { get; set; }
            public bool IsRealTime;
        }
      
        
        
        private NpgsqlConnection _conn;
        public GraphChart graphChart = new GraphChart();

        private void OpenConnection(string user, string password, string ip, string port)
        {
            try
            {
                //conn = new NpgsqlConnection("User Id=" + user + ";Password=" + password + ";Data Source=" + ip + ":" + port + "/ORCL;");
                _conn = new NpgsqlConnection("Server=" + ip + ";Port=" + port + ";User Id=" + user + ";Password=" + password + ";Database=test_trade;");
                _conn.Open();
            }
            catch (Exception)

            {
                // ignored
            }
        }
        public void CloseConnection()
        {
            _conn.Close();
            _conn.Dispose();
        }
        private void ExecNonQuery(string sql)
        {
            using (NpgsqlCommand command = new NpgsqlCommand(sql))
            {
                // command.CommandText = sql;
                command.Connection = _conn;
                command.CommandTimeout = 0;
                try
                {
                    //a non-query command doesn’t need any reader, all you have to do is execute them !
                    command.ExecuteNonQuery();
                    command.Dispose();
                }
                catch (Exception) //same table found
                {
                    // ignored
                }
            }
        }       
        public void GetChart1(Dameer timeFrom, Dameer timeTo, string tableName, string symbolName, bool IsRealTime)
        {
            string sql;
            int count = 0;
            int i = 0;
            sql = "SELECT id FROM " + tableName + " WHERE Time >= '" + timeFrom.Value + "' AND Time < '" + timeTo.Value + "' AND SYMBOL = '" + symbolName + "' AND Lvl=1 ORDER BY id limit 1";
            i = GetFirstId(sql);
            // while (true)
            {
                sql = "SELECT Id, Time, TimeMS, Symbol, Currency, Lvl, BidP1, BidQ1, BidP2, BidQ2, BidP3, BidQ3, BidP4, BidQ4, BidP5, BidQ5, AskP1, AskQ1, AskP2, AskQ2, AskP3, AskQ3, AskP4, AskQ4, AskP5, AskQ5 FROM " + tableName + " WHERE Time >= '" + timeFrom.Value + "' AND Time < '" + timeTo.Value + "' AND SYMBOL = '" + symbolName + "' AND id >= " + i + " ORDER BY id";
                // sql = "SELECT Id, Time, TimeMS, Symbol, Currency, Lvl, BidP1, BidQ1, BidP2, BidQ2, BidP3, BidQ3, BidP4, BidQ4, BidP5, BidQ5, AskP1, AskQ1, AskP2, AskQ2, AskP3, AskQ3, AskP4, AskQ4, AskP5, AskQ5 FROM " + tableName + " WHERE Time >= '" + timeFrom.Value + "' AND Time < '" + timeTo.Value + "' AND SYMBOL = '" + symbolName + "'ORDER BY id";
                count = ExecQueryFprGraph(sql);
            }
        }
     
        private int ExecQueryFprGraph(string sql)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand(sql))
            {
                cmd.Connection = _conn;
                OpenConnection("postgres", "vitaliy", "localhost", "5432");
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 0;
                int count = 0;
                try
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        LoadSymbols1 symbol1 = new LoadSymbols1();
                        while (reader.Read())
                        {

                            symbol1.Time = DateTime.Parse(reader["Time"].ToString());

                            symbol1.Time = symbol1.Time.AddMilliseconds(double.Parse(reader["TimeMS"].ToString()));
                            // double d = double.Parse(symbol.Time.ToString());                           
                            symbol1.Level = int.Parse(reader["Lvl"].ToString());
                            symbol1.Symbol = reader["Symbol"].ToString();
                            symbol1.Currency = reader["Currency"].ToString();
                            for (int i = 0; i < 5; i++)
                            {
                                symbol1.BidPrices[i] = double.Parse(reader["BidP" + (i + 1)].ToString());
                                symbol1.AskPrices[i] = double.Parse(reader["AskP" + (i + 1)].ToString());
                                symbol1.BidQuantities[i] = int.Parse(reader["BidQ" + (i + 1)].ToString());
                                symbol1.AskQuantities[i] = int.Parse(reader["AskQ" + (i + 1)].ToString());
                            }
                            graphChart.ChartsList.Add(symbol1);
                            Color[] colors = ColorHelper.CreateRandomColors(4);
                            plotter.AddLineGraph(CreateTest(graphChart.ChartsList), colors[3], 2, "BidPrice1 /Timestampt");
                            if (count > 10000)
                            {
                                if (graphChart.ChartsList[count].Time < graphChart.ChartsList[count - 1].Time)
                                {
                                    //MessageBox.Show("urgh data is not sequenced properly: " + Streams[index].chart[count].Time.ToString(), Streams[index].SymbolName);
                                    return (-1);
                                }
                            }
                            count++;

                            //Application.DoEvents();
                        }
                        return (count);
                    }
                }
                catch //(Exception ex)
                {
                    //System.Windows.Forms.MessageBox.Show(ex.Message);
                    return (0);
                }
            }
        }
        
        private int GetFirstId(string sql)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand(sql))
            {
                cmd.Connection = _conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 0;
                try
                {
                    object a = cmd.ExecuteScalar();
                    return (int.Parse(a.ToString()));
                }
                catch (Exception)
                {

                    return (0);
                }
            }
        }
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            
            GetChart1(dameer3, dameer1, graphlie.Text, Symbol.Text, false);
           
        }
    }
}

    //internal class BidCurrencyInfo
    //{
    //    public double CurrentTime { get; set; }
    //    public double Price { get; set; }
    //}
    //internal class AskCurrencyInfo
    //{
    //    public double CurrentTime { get; set; }
    //    public double Price { get; set; }
    //}
    //internal class BidQuCurrencyInfo
    //{
    //    public double CurrentTime { get; set; }
    //    public int BidQuantity { get; set; }
    //}
    //internal class AskQuCurrencyInfo
    //{
    //    public double CurrentTime { get; set; }
    //    public int AskQuantity { get; set; }
    //}

internal class CurrencyInfo
{
    public double CurrentTime { get; set; }
    public decimal AskQuantity { get; set; }
    public double BidPrice { get; set; }
    public double AskPrice { get; set; }
    public int BidQuantity { get; set; }

}



