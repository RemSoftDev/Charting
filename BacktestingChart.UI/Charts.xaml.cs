using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BacktestingChart.DataAccess;
using Loya.Dameer;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using DynamicDataDisplay.Markers.DataSources;
using Microsoft.Research.DynamicDataDisplay;
using Npgsql;

namespace BacktestingChart.UI
{
    /// <summary>
    /// Interaction logic for Charts.xaml
    /// </summary>
    public partial class Charts : Window
    {
        public Charts()
        {
            InitializeComponent();
            OpenConnection("postgres", "vitaliy", "localhost", "5432");
        }

        
        public class LoadSymbols
        {
            public DateTime Time;
            public string Symbol { get; set; }
            public string Currency { get; set; }
            public int Level { get; set; }
            public double[] BidPrices { get; set; } = new double[5];
            public int[] BidQuantities { get; set; } = new int[5];
            public int[] AskQuantities { get; set; } = new int[5];
            public double[] AskPrices { get; set; } = new double[5];
        }

        public class Graph
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
        public Graph graph = new Graph();                     
        private NpgsqlConnection _conn;        
        //public int InsertLength = 10000;       
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

        //public void GetChart(string timeFrom, string timeTo, string tableName, string symbolName, bool IsRealTime,int Count)
        public void GetChart(Dameer timeFrom, Dameer timeTo, string tableName, string symbolName, bool IsRealTime)
        {
            int count = 0;
            int i = 0;
            graph.ChartsList = new List<LoadSymbols>();
            var sql = "SELECT id FROM " + tableName + " WHERE Time >= '" + timeFrom.Value + "' AND Time < '" + timeTo.Value + "' AND SYMBOL = '" + symbolName + "' AND Lvl=1 ORDER BY id limit 1";
            i = GetFirstId(sql);
            // while (true)
            {
                sql = "SELECT Id, Time, TimeMS, Symbol, Currency, Lvl, BidP1, BidQ1, BidP2, BidQ2, BidP3, BidQ3, BidP4, BidQ4, BidP5, BidQ5, AskP1, AskQ1, AskP2, AskQ2, AskP3, AskQ3, AskP4, AskQ4, AskP5, AskQ5 FROM " + tableName + " WHERE Time >= '" + timeFrom.Value + "' AND Time < '" + timeTo.Value + "' AND SYMBOL = '" + symbolName + "' AND id >= " + i + " ORDER BY id";
                // sql = "SELECT Id, Time, TimeMS, Symbol, Currency, Lvl, BidP1, BidQ1, BidP2, BidQ2, BidP3, BidQ3, BidP4, BidQ4, BidP5, BidQ5, AskP1, AskQ1, AskP2, AskQ2, AskP3, AskQ3, AskP4, AskQ4, AskP5, AskQ5 FROM " + tableName + " WHERE Time >= '" + timeFrom.Value + "' AND Time < '" + timeTo.Value + "' AND SYMBOL = '" + symbolName + "'ORDER BY id";
                count = ExecQuery(sql);
            }
        }

        private int ExecQuery(string sql)
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
                        LoadSymbols symbol = new LoadSymbols();
                        while (reader.Read())
                        {
                           
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
                            graph.ChartsList.Add(symbol);
                            if (count > 10000)
                            {
                                if (graph.ChartsList[count].Time < graph.ChartsList[count - 1].Time)
                                {                                   
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
                    //MessageBox.Show(ex.Message, Streams[index].SymbolName);
                    return (0);
                }
            }
        }
        private void Charts_OnLoaded_Loaded(object sender, RoutedEventArgs e)
        {
            
            Color[] colors = ColorHelper.CreateRandomColors(4);
            plotter.AddLineGraph(CreateCurrencyDataSourceForBidP(graph.ChartsList), colors[0], 1, "BidPrice /Timestampt");
        }
        private EnumerableDataSource<LoadSymbols> CreateCurrencyDataSourceForBidP(List<LoadSymbols> rates)
        {
            EnumerableDataSource<LoadSymbols> ds = new EnumerableDataSource<LoadSymbols>(rates);
            ds.SetXMapping(ci => dateAxis.ConvertToDouble(ci.Time));
            //ds.SetXMapping(ci => (ci.Time));
            ds.SetYMapping(ci => ci.BidPrices[0]);
            ds.SetYMapping(ci => ci.BidPrices[1]);
            ds.SetYMapping(ci => ci.BidPrices[2]);
            ds.SetYMapping(ci => ci.BidPrices[3]);
            ds.SetYMapping(ci => ci.BidPrices[4]);
            return ds;
        }
    }
}
