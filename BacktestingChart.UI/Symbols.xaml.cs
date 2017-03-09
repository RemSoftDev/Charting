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
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BacktestingChart.DataAccess;
using Loya.Dameer;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts.Navigation;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;
using Npgsql;

namespace BacktestingChart.UI
{
    /// <summary>
    /// Interaction logic for Symbols.xaml
    /// </summary>
    public partial class Symbols : Window
    {
        public Symbols()
        {
            InitializeComponent();
            OpenConnection("postgres", "vitaliy", "localhost", "5432");
            //_querryThread.Start();
        }

        public class Record
        {
            public DateTime Time { get; set; }
            public string Symbol { get; set; }
            public string Currency { get; set; }
            public int Level { get; set; }
            public double[] BidPrices { get; set; } = new double[5];
            public int[] BidQuantities { get; set; } = new int[5];
            public int[] AskQuantities { get; set; } = new int[5];
            public double[] AskPrices { get; set; } = new double[5];

            public void InitContent()
            {
                for (int i = 0; i < 5; i++)
                {
                    BidPrices[i] = AskPrices[i] = 0.0;
                    BidQuantities[i] = AskQuantities[i] = 0;
                }
            }
        }

        //public struct LoadSymbols
        //{
        //    public DateTime Time;
        //    public string Symbol;
        //    public string Currency;
        //    public int Level;
        //    public decimal[] BidPrices;
        //    public int[] BidQuantities;
        //    public int[] AskQuantities;
        //    public decimal[] AskPrices;
        //}

        public class OrderBookReq
        {
            public int Id { get; set; }
            public DateTime Time { get; set; }
            public string Symbol { get; set; }
            public string Currency { get; set; }
            public int Level { get; set; }
            public double[] BidPrices { get; set; } = new double[5];
            public int[] BidQuantities { get; set; } = new int[5];
            public int[] AskQuantities { get; set; } = new int[5];
            public double[] AskPrices { get; set; } = new double[5];
        }
        public class OrderBookResp
        {
            public int Id { get; set; }
            public DateTime Time { get; set; }
            public string Symbol { get; set; }
            public string Currency { get; set; }
            public int Level { get; set; }
            public double[] BidPrices { get; set; } = new double[5];
            public int[] BidQuantities { get; set; } = new int[5];
            public int[] AskQuantities { get; set; } = new int[5];
            public double[] AskPrices { get; set; } = new double[5];
        }
        public class LoadSymbols
        {
            public int Id { get; set; }
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

        public class GraphLoad
        {
            public List<LoadSymbols> ChartsList = new List<LoadSymbols>();
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
        public List<OrderBookReq> OrderBookReqs = new List<OrderBookReq>();
        public List<OrderBookResp> OrderBookResps = new List<OrderBookResp>();
        private GraphLoad graph = new GraphLoad();
        public GraphArrow graphArrow = new GraphArrow();
        public Thread _querryThread;
        public bool okRun = true;
        public Queue queue = new Queue();
        private NpgsqlConnection _conn;
        public StringBuilder SqlAddArrowRecord = new StringBuilder();
        public int InsertLength = 10000;

        [DllImport("kernel32")]
        static extern int GetCurrentThreadId();

        private void Run_QuerryThread()
        {
            _querryThread.Priority = ThreadPriority.Highest;
            foreach (ProcessThread pt in Process.GetCurrentProcess().Threads)
            {
                int utid = GetCurrentThreadId();

                if (utid == pt.Id)
                {
                    pt.ProcessorAffinity = (IntPtr) (Math.Pow(2, 1));
                }
            }
            while (okRun)
            {
                if (queue.Count <= 0) continue;
                var auxQuerry = (string) queue.Dequeue();
                ExecNonQuery(auxQuerry);
            }
        }

        private void OpenConnection(string user, string password, string ip, string port)
        {
            try
            {
                //conn = new NpgsqlConnection("User Id=" + user + ";Password=" + password + ";Data Source=" + ip + ":" + port + "/ORCL;");
                _conn =
                    new NpgsqlConnection("Server=" + ip + ";Port=" + port + ";User Id=" + user + ";Password=" + password +
                                         ";Database=test_trade;");
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

        public void GetArrowses(Dameer timeFrom, Dameer timeTo, string tableName, string symbolName)
        {
            string sql;
            var firstIdsql = "SELECT id FROM " + tableName + " WHERE Time >= '" + timeFrom.Value + "' AND Time < '" +
                      timeTo.Value + "' AND SYMBOL = '" + symbolName + "' AND Lvl=1 ORDER BY id limit 1";
            var i = GetFirstId(firstIdsql);
            sql = "SELECT Id, Time, TimeMS, Symbol,Price,Direction,Name FROM " + tableName + " WHERE Time >= '" +
                  timeFrom.Value + "' AND Time < '" + timeTo.Value + "' AND SYMBOL = '" + symbolName + "' AND id >= " +
                  i + " ORDER BY id";
            ExecQueryArrow(sql);
        }
       
        public void GetChart(Dameer timeFrom, Dameer timeTo, string tableName, string symbolName, bool IsRealTime)
        {
            string sql;
            int count = 0;
            int i = 0;
            string sqlRange;
            string sqlFirst;
           
            sql = "SELECT id FROM " + tableName + " WHERE Time >= '" + timeFrom.Value + "' AND Time < '" + timeTo.Value +
                  "' AND SYMBOL = '" + symbolName + "' AND Lvl=1 ORDER BY id limit 1";
            i = GetFirstId(sql);
            
            
               
                sqlFirst =
                    "SELECT Id, Time, TimeMS, Symbol, Currency, Lvl, BidP1, BidQ1, BidP2, BidQ2, BidP3, BidQ3, BidP4, BidQ4, BidP5, BidQ5, AskP1, AskQ1, AskP2, AskQ2, AskP3, AskQ3, AskP4, AskQ4, AskP5, AskQ5 FROM " +
                    tableName + " WHERE Time >= '" + timeFrom.Value + "' AND Time < '" + timeTo.Value +
                    "' AND SYMBOL = '" + symbolName + "' AND id >= " + i + " ORDER BY id";            
            count = ExecQueryFprGraph(sqlFirst);
            int firstId = 0;
            firstId = i;  
            int afterId = firstId + count;
            string Id = afterId.ToString();
            sqlRange =
               "SELECT Id, Time, TimeMS, Symbol, Currency, Lvl, BidP1, BidQ1, BidP2, BidQ2, BidP3, BidQ3, BidP4, BidQ4, BidP5, BidQ5, AskP1, AskQ1, AskP2, AskQ2, AskP3, AskQ3, AskP4, AskQ4, AskP5, AskQ5 FROM " +
               tableName + " WHERE Time >= '" + timeFrom.Value + "' AND Time < '" + timeTo.Value +
               "' AND SYMBOL = '" + symbolName + "' AND id >= " + firstId + " AND id <= " + afterId + " ORDER BY id";
            ExecQueryGraph(sqlRange,count);        

        }
        public void GetOrderBook(Dameer timeFrom, Dameer timeTo, string tableName, string symbolName)
        {
            string sql;
            int count = 0;
            int i = 0;
            string sqlRange;
            string sqlFirst;

            sql = "SELECT id FROM " + tableName + " WHERE Time >= '" + timeFrom.Value + "' AND Time < '" + timeTo.Value +
                  "' AND SYMBOL = '" + symbolName + "' AND Lvl=1 ORDER BY id limit 1";
            i = GetFirstId(sql);

            sqlFirst =
                "SELECT Id, Time, TimeMS, Symbol, Currency, Lvl, BidP1, BidQ1, BidP2, BidQ2, BidP3, BidQ3, BidP4, BidQ4, BidP5, BidQ5, AskP1, AskQ1, AskP2, AskQ2, AskP3, AskQ3, AskP4, AskQ4, AskP5, AskQ5 FROM " +
                tableName + " WHERE Time >= '" + timeFrom.Value + "' AND Time < '" + timeTo.Value +
                "' AND SYMBOL = '" + symbolName + "' AND id >= " + i + " ORDER BY id";
            count = ExecQueryFprGraph(sqlFirst);
            int firstId = 0;
            firstId = i;
            int afterId = firstId + count;
            string Id = afterId.ToString();
            sqlRange =
               "SELECT Id, Time, TimeMS, Symbol, Currency, Lvl, BidP1, BidQ1, BidP2, BidQ2, BidP3, BidQ3, BidP4, BidQ4, BidP5, BidQ5, AskP1, AskQ1, AskP2, AskQ2, AskP3, AskQ3, AskP4, AskQ4, AskP5, AskQ5 FROM " +
               tableName + " WHERE Time >= '" + timeFrom.Value + "' AND Time < '" + timeTo.Value +
               "' AND SYMBOL = '" + symbolName + "' AND id >= " + firstId + " AND id <= " + afterId + " ORDER BY id";
            ExecQueryOrderBook(sqlRange, count);

        }
        //public List<LoadSymbols> GetChart1(Dameer timeFrom, Dameer timeTo, string tableName, string symbolName, bool IsRealTime)
        //{
        //    string sql;
        //    int count = 0;
        //    int i = 0;

        //    sql = "SELECT id FROM " + tableName + " WHERE Time >= '" + timeFrom.Value + "' AND Time < '" + timeTo.Value +
        //          "' AND SYMBOL = '" + symbolName + "' AND Lvl=1 ORDER BY id limit 1";
        //    i = GetFirstId(sql);          
        //     sql =
        //            "SELECT Id, Time, TimeMS, Symbol, Currency, Lvl, BidP1, BidQ1, BidP2, BidQ2, BidP3, BidQ3, BidP4, BidQ4, BidP5, BidQ5, AskP1, AskQ1, AskP2, AskQ2, AskP3, AskQ3, AskP4, AskQ4, AskP5, AskQ5 FROM " +
        //            tableName + " WHERE Time >= '" + timeFrom.Value + "' AND Time < '" + timeTo.Value +
        //            "' AND SYMBOL = '" + symbolName + "' AND id >= " + i + " ORDER BY id";

        //     return ExecQueryGraph(sql);

        //}       
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
                   
                    //using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        LoadSymbols symbol = new LoadSymbols();
                        while (reader.Read())
                        {
                            //symbol.Id = int.Parse(reader["Id"].ToString());
                            //symbol.Time = DateTime.Parse(reader["Time"].ToString());
                            symbol.Id = int.Parse(reader["Id"].ToString());
                            symbol.Time = DateTime.Parse(reader["Time"].ToString());
                            symbol.Time = symbol.Time.AddMilliseconds(double.Parse(reader["TimeMS"].ToString()));                                                  
                            symbol.Level = int.Parse(reader["Lvl"].ToString());
                            symbol.Symbol = reader["Symbol"].ToString();
                            symbol.Currency = reader["Currency"].ToString();
                            for (int i = 0; i < 5; i++)
                            {
                                symbol.BidPrices[i] = double.Parse(reader["BidP" + (i + 1)].ToString());
                                symbol.AskPrices[i] = double.Parse(reader["AskP" + (i + 1)].ToString());
                                symbol.BidQuantities[i] = int.Parse(reader["BidQ" + (i + 1)].ToString());
                                symbol.AskQuantities[i] = int.Parse(reader["AskQ" + (i + 1)].ToString());
                            }                                                                                                  
                          
                            //if (count > 10000)
                            //{
                            //    //if (records[count].Time < records[count - 1].Time)
                            //    {
                            //        //MessageBox.Show("urgh data is not sequenced properly: " + Streams[index].chart[count].Time.ToString(), Streams[index].SymbolName);
                            //        return (-1);
                            //    }
                            //}
                            count++;

                            //graph.ChartsList.Add(symbol);
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
        List<Record> records=new List<Record>();
        private int ExecQueryGraph(string sql,int countRecod)
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
                        //List<Record> records = new List<Record>();                      
                         while (reader.Read())
                        //for (int j = 0; j < count; j++)

                        {
                            Record record = new Record();
                            //record.Id = int.Parse(reader["Id"].ToString());
                            record.Time = DateTime.Parse(reader["Time"].ToString());
                            record.Time = record.Time.AddMilliseconds(double.Parse(reader["TimeMS"].ToString()));                                             
                            record.Level = int.Parse(reader["Lvl"].ToString());
                            record.Symbol = reader["Symbol"].ToString();
                            record.Currency = reader["Currency"].ToString();
                            for (int i = 0; i < 5; i++)
                            {
                                record.BidPrices[i] = double.Parse(reader["BidP" + (i + 1)].ToString());
                                record.AskPrices[i] = double.Parse(reader["AskP" + (i + 1)].ToString());
                                record.BidQuantities[i] = int.Parse(reader["BidQ" + (i + 1)].ToString());
                                record.AskQuantities[i] = int.Parse(reader["AskQ" + (i + 1)].ToString());
                            }

                            records.Add(record);
                            //if (count > 10000)
                            //{
                            //    //if (records[count].Time < records[count - 1].Time)
                            //    {
                            //        //MessageBox.Show("urgh data is not sequenced properly: " + Streams[index].chart[count].Time.ToString(), Streams[index].SymbolName);
                            //        return (-1);
                            //    }
                            //}
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
        private List<LoadArrows> ExecQueryArrow(string sql)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand(sql))
            {
                cmd.Connection = _conn;
                OpenConnection("postgres", "vitaliy", "localhost", "5432");
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 0;
                           
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    LoadArrows loadArrowses = new LoadArrows();
                    while (reader.Read())
                    {
                        loadArrowses.Time = DateTime.Parse(reader["Time"].ToString());
                        loadArrowses.Time = loadArrowses.Time.AddMilliseconds(double.Parse(reader["TimeMS"].ToString()));
                        loadArrowses.BidPrices = double.Parse(reader["Price"].ToString());
                        loadArrowses.Symbol = reader["Symbol"].ToString();
                        loadArrowses.Name = reader["Name"].ToString();
                        loadArrowses.Directive = reader["Direction"].ToString();
                        graphArrow.Arrowses.Add(loadArrowses);
                    }
                    return graphArrow.Arrowses;
                }
            }
        }
        private int ExecQueryOrderBook(string sql, int countRecod)
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
                        //List<Record> records = new List<Record>();                      
                        while (reader.Read())
                        //for (int j = 0; j < count; j++)

                        {
                            int bidPriceBase = 3, bidQuantityBase = 8, askPriceBase = 13, askQuantityBase = 18;
                            OrderBookResp record = new OrderBookResp();
                            //record.Id = int.Parse(reader["Id"].ToString());
                            record.Time = DateTime.Parse(reader["Time"].ToString());
                            record.Time = record.Time.AddMilliseconds(double.Parse(reader["TimeMS"].ToString()));
                            record.Level = int.Parse(reader["Lvl"].ToString());
                            record.Symbol = reader["Symbol"].ToString();
                            record.Currency = reader["Currency"].ToString();
                            for (int i = 0; i < 5; i++)
                            {
                                record.BidPrices[i] = double.Parse(reader["BidP" + (i + 1 + bidPriceBase)].ToString());
                                record.AskPrices[i] = double.Parse(reader["AskP" + (i + 1 + bidQuantityBase)].ToString());
                                record.BidQuantities[i] = int.Parse(reader["BidQ" + (i + 1 + askPriceBase)].ToString());
                                record.AskQuantities[i] = int.Parse(reader["AskQ" + (i + 1 + askQuantityBase)].ToString());
                            }

                            OrderBookResps.Add(record);
                            //if (count > 10000)
                            //{
                            //    //if (records[count].Time < records[count - 1].Time)
                            //    {
                            //        //MessageBox.Show("urgh data is not sequenced properly: " + Streams[index].chart[count].Time.ToString(), Streams[index].SymbolName);
                            //        return (-1);
                            //    }
                            //}
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
        //executeMethod
        //public List<LoadSymbols> Symbolses=new List<LoadSymbols>();
        //private List<LoadSymbols> ExecQueryGraph(string sql)
        //{
        //    using (NpgsqlCommand cmd = new NpgsqlCommand(sql))
        //    {
        //        cmd.Connection = _conn;
        //        OpenConnection("postgres", "vitaliy", "localhost", "5432");
        //        cmd.CommandType = CommandType.Text;
        //        cmd.CommandTimeout = 0;
        //        int count = 0;
        //        using (NpgsqlDataReader reader = cmd.ExecuteReader())
        //        {
        //            LoadSymbols symbol = new LoadSymbols();
        //            while (reader.Read())
        //            {
        //                symbol.Time = DateTime.Parse(reader["Time"].ToString());
        //                symbol.Time = symbol.Time.AddMilliseconds(double.Parse(reader["TimeMS"].ToString()));                                               
        //                symbol.Level = int.Parse(reader["Lvl"].ToString());
        //                symbol.Symbol = reader["Symbol"].ToString();
        //                symbol.Currency = reader["Currency"].ToString();
        //                for (int i = 0; i < 5; i++)
        //                {
        //                    symbol.BidPrices[i] = double.Parse(reader["BidP" + (i + 1)].ToString());
        //                    symbol.AskPrices[i] = double.Parse(reader["AskP" + (i + 1)].ToString());
        //                    symbol.BidQuantities[i] = int.Parse(reader["BidQ" + (i + 1)].ToString());
        //                    symbol.AskQuantities[i] = int.Parse(reader["AskQ" + (i + 1)].ToString());
        //                    Symbolses.Add(symbol);
        //                }

        //            }                   
        //        }              
        //    }
        //   return Symbolses;
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
            ds.SetYMapping(ci => Convert.ToDouble(ci.AskQuantity));
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
                    res.Add(new CurrencyInfo { CurrentTime = date, BidPrice = bidPrice, BidQuantity = bidQu, AskPrice = askPrice, AskQuantity = askQu });
                }
            }
            return res;
        }
        LineAndMarker<ElementMarkerPointsGraph> chart;
        public IPointDataSource ds;
        private void dataSource(List<CurrencyInfo> rates)
        {

            EnumerableDataSource<CurrencyInfo> xDataSource = new EnumerableDataSource<CurrencyInfo>(rates);
            EnumerableDataSource<CurrencyInfo> yDataSource = new EnumerableDataSource<CurrencyInfo>(rates);
            yDataSource.SetYMapping(y => y.BidPrice);
            yDataSource.AddMapping(ShapeElementPointMarker.ToolTipTextProperty, y => String.Format("BidPrice is {0}", y.BidPrice));
            xDataSource.SetXMapping(x => x.CurrentTime);
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
        private void dataSource1(List<Record> rates)
        {

            EnumerableDataSource<Record> xDataSource = new EnumerableDataSource<Record>(rates);
            EnumerableDataSource<Record> yDataSource = new EnumerableDataSource<Record>(rates);
            yDataSource.SetYMapping(y => y.BidPrices[0]);
            yDataSource.AddMapping(ShapeElementPointMarker.ToolTipTextProperty, y => String.Format("BidPrice is {0}", y.BidPrices[0]));
            xDataSource.SetXMapping(x => dateAxisBidP.ConvertToDouble(x.Time));
            xDataSource.AddMapping(ShapeElementPointMarker.ToolTipTextProperty,x=> String.Format("Time is {0}", x.Time.ToString(CultureInfo.InvariantCulture)));
            ds = new CompositeDataSource(xDataSource, yDataSource);
            chart = plotter.AddLineGraph(ds,
                new Pen(Brushes.LimeGreen, 3),
                new CircleElementPointMarker
                {
                    Size = 5,
                    Brush = Brushes.Red,
                    Fill = Brushes.Orange
                },

                new PenDescription("BidPrice"));

            plotter.Children.Add(new CursorCoordinateGraph());
            //plotter.FitToView();
        }

        public EnumerableDataSource<Record> CreateCurrencyDataSourceForBidP(List<Record> rates)
        {
            EnumerableDataSource<Record> ds = new EnumerableDataSource<Record>(rates);
            //ds.SetXMapping(ci =>dateAxis.ConvertToDouble(ci.Time.Date));
            //ds.SetXMapping(ci => dateAxis.ConvertToDouble(Convert.ToDateTime(ci.Time.Date.ToString("MM/dd/yyyy HH:mm"))));
            //ds.SetXMapping(ci =>ci.BidPrices[0]);
            //ds.SetXMapping(ci => dateAxisBidP.ConvertToDouble(new DateTime(ci.Time.Minute)));
            ds.SetXMapping(ci => dateAxisBidP.ConvertToDouble(ci.Time));
            ds.SetYMapping(ci =>(ci.BidPrices[0]));                      
            return ds;
        }
        public EnumerableDataSource<Record> CreateCurrencyDataSourceForAskQuantity(List<Record> rates)
        {
            EnumerableDataSource<Record> ds = new EnumerableDataSource<Record>(rates);
            
            ds.SetYMapping(ci => Convert.ToDouble(ci.AskQuantities[0]));
            //ds.SetXMapping(ci => ci.AskPrices[0]);
            ds.SetXMapping(ci => dateAxisAsq.ConvertToDouble(ci.Time));
            return ds;
        }
        public EnumerableDataSource<Record> CreateCurrencyDataSourceForBidQuantity(List<Record> rates)
        {
            EnumerableDataSource<Record> ds = new EnumerableDataSource<Record>(rates);

            ds.SetYMapping(ci => Convert.ToDouble(ci.BidQuantities[0]));
            //ds.SetXMapping(ci => ci.BidPrices[0]);
            ds.SetXMapping(ci => dateAxis.ConvertToDouble(ci.Time));
            return ds;
        }
        private void button1_Click(object sender, RoutedEventArgs e)
        {
                                  
            GetChart(dameer3,dameer1,graphLine.Text,Symbol.Text,false);
            Color[] colors = ColorHelper.CreateRandomColors(4);          
           //plotter.AddLineGraph(CreateCurrencyDataSourceForBidP(records),colors[1], 1, "BidPrice /Timestampt");
            plotterAsQu.AddLineGraph(CreateCurrencyDataSourceForAskQuantity(records), colors[2], 1, "AskQuantity /Timestampt");
            plotterBidQu.AddLineGraph(CreateCurrencyDataSourceForBidQuantity(records), colors[3], 1, "BidQuantity /Timestampt");
            dataSource1(records);
            chart.MarkerGraph.DataSource = null;
        }

        //private void button2_Click(object sender, RoutedEventArgs e)
        //{           
        //    UI.Graph graph=new UI.Graph();
        //    graph.Show();           
        //}

        private void button_Click(object sender, RoutedEventArgs e)
        {
            //GetChart(dameer3, dameer1, graphlie.Text, Symbol.Text, false);
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".csv"; // Default file extension

            // Show open file dialog box
            bool? result = dlg.ShowDialog();
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
                chart.MarkerGraph.DataSource = null;

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
        
        private void dragCursorLine(object sender, DragEventArgs e)
        {

        //    plotter.Children.Add(mouseTrack);
        //    mouseTrack.ShowHorizontalLine = false;
        //    mouseTrack.ShowVerticalLine = false;
        //    Point mousePos = mouseTrack.Position;
        //    var transform = plotter.Viewport.Transform;
        //    Point mousePosInData = mousePos.ScreenToData(transform);
        //    double xValue = mousePosInData.X;
        //    double yValue = mousePosInData.Y;


        }
      
        private void MousePosClick(object sender, MouseButtonEventArgs e)
        {
                      
        }
        //arrows
        private void button3_Click(object sender, RoutedEventArgs e)
        {
            GetArrowses(dameer3, dameer1, arrowsBox.Text, Symbol.Text);
            


        }
    }
    internal class CurrencyInfo
    {
        public double CurrentTime { get; set; }
        public decimal AskQuantity { get; set; }
        public double BidPrice { get; set; }
        public double AskPrice { get; set; }
        public int BidQuantity { get; set; }

    }
}

