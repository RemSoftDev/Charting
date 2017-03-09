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
using Npgsql;
namespace BacktestingChart.DataAccess
{
  public  class ChDbAccess
    {
        public class Record
        {
            public DateTime Time;
            public string Symbol { get; set;}
            public string Currency { get; set;}
            public int Level { get; set; }
            public double[] BidPrices { get; set;} = new double[5];
            public int[] BidQuantities { get; set;} = new int[5];
            public int[] AskQuantities { get; set;} = new int[5];
            public double[] AskPrices { get; set;} = new double[5];

            public void InitContent()
            {
                for (int i = 0; i < 5; i++)
                {
                     BidPrices[i] =AskPrices[i]= 0.0;
                     BidQuantities[i] = AskQuantities[i] = 0;
                }
            }
        }
        public struct LoadSymbols
        {
            public DateTime Time;
            public string Symbol;
            public string Currency;
            public int Level;
            public double[] BidPrices;
            public int[] BidQuantities;
            public int[] AskQuantities;
            public double[] AskPrices;            
        }
        public class Graph
        {
           public List<LoadSymbols>ChartsList=new List<LoadSymbols>();
           int coutnt = 0;
        }
        public struct QueryReq
        {
           public DateTime timeFrom;
           public DateTime timeTo;
           public string symbol;
           public int nFrom;
           public int nCount;
           public bool IsRealTime;
        }

      private List<DataParse> parse = new List<DataParse>();      
        readonly Thread _querryThread;
        public bool okRun = true;
        public Queue queue = new Queue();
        private NpgsqlConnection _conn;
        public StringBuilder SqlAddArrowRecord = new StringBuilder();
        public int InsertLength = 10000;
        public ChDbAccess()
        {
            OpenConnection("test_trade", "vitaliy", "localhost", "5432");
        }
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
                    pt.ProcessorAffinity = (IntPtr)(Math.Pow(2, 1));
                }
            }
            while (okRun)
            {
                if (queue.Count <= 0) continue;
                var auxQuerry = (string)queue.Dequeue();
                ExecNonQuery(auxQuerry);
            }
        }
        private void OpenConnection(string user, string password, string ip, string port)
        {
            try
            {
                //conn = new NpgsqlConnection("User Id=" + user + ";Password=" + password + ";Data Source=" + ip + ":" + port + "/ORCL;");
                _conn = new NpgsqlConnection("Server=" + ip + ";Port=" + port + ";User Id=" + user + ";Password=" + password + ";Database=i_trade;");
                _conn.Open();                
            }
            catch (Exception)
                //this is your first time ! so,learn to use “try catch” as available as possible! it’s very important!
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

        public void AddRecord(string tableName,Record record)
        {
            SqlAddArrowRecord.Append("INSERT INTO");
            SqlAddArrowRecord.Append(tableName);
            SqlAddArrowRecord.Append("");
            SqlAddArrowRecord.Append("(Id, Time, TimeMS, Symbol, Currency, Lvl, BidP1, BidQ1, BidP2, BidQ2, BidP3, BidQ3, BidP4, BidQ4, BidP5, BidQ5, AskP1, AskQ1, AskP2, AskQ2, AskP3, AskQ3, AskP4, AskQ4, AskP5, AskQ5)");
            SqlAddArrowRecord.Append(" VALUES (");
            SqlAddArrowRecord.Append("\'");
            SqlAddArrowRecord.Append(record.Time.Year+"-");
            SqlAddArrowRecord.Append(record.Time.Month + "-");
            SqlAddArrowRecord.Append(record.Time.Day + " ");
            SqlAddArrowRecord.Append(record.Time.Hour + ":");
            SqlAddArrowRecord.Append(record.Time.Minute + ":");
            SqlAddArrowRecord.Append(record.Time.Second + "\',");
            SqlAddArrowRecord.Append(record.Time.Millisecond +",");
            SqlAddArrowRecord.Append("\'"+record.Symbol + "\',");
            SqlAddArrowRecord.Append("\'"+record.Currency + "\',");
            SqlAddArrowRecord.Append(record.Level + ",");
            for (int i = 0; i < 5; i++)
            {
                SqlAddArrowRecord.Append(record.BidPrices[i] + "," + record.BidQuantities[i]);
                if (i<4)
                {
                    SqlAddArrowRecord.Append(",");
                }
            }
            for (int i = 0; i < 5; i++)
            {
                SqlAddArrowRecord.Append(record.AskPrices[i] + "," + record.AskQuantities[i]);
                if (i < 4)
                {
                    SqlAddArrowRecord.Append(",");
                }
            }
            SqlAddArrowRecord.Append(")");
            if (SqlAddArrowRecord.Length > InsertLength)
            {
                queue.Enqueue(SqlAddArrowRecord.ToString());
                SqlAddArrowRecord.Clear();
            }
        }
        //demo version
        int GetFirstGraph(string tableName,QueryReq req)
        {
            int index = 0;
            SqlAddArrowRecord.Append("SELECT*FROM");
            SqlAddArrowRecord.Append(tableName);
            SqlAddArrowRecord.Append("WHERE Time >= \'"+req.timeFrom.Year);
            SqlAddArrowRecord.Append("-"+req.timeFrom.Month + "-");
            SqlAddArrowRecord.Append(req.timeFrom.Day + " ");
            SqlAddArrowRecord.Append(req.timeFrom.Hour + ":");
            SqlAddArrowRecord.Append(req.timeFrom.Minute + ":");
            SqlAddArrowRecord.Append(req.timeFrom.Second + "\' ");
            SqlAddArrowRecord.Append("AND Time < \'");
            SqlAddArrowRecord.Append(req.timeFrom.Year);
            SqlAddArrowRecord.Append("-" + req.timeFrom.Month + "-");
            SqlAddArrowRecord.Append(req.timeFrom.Day + " ");
            SqlAddArrowRecord.Append(req.timeFrom.Hour + ":");
            SqlAddArrowRecord.Append(req.timeFrom.Minute + ":");
            SqlAddArrowRecord.Append(req.timeFrom.Second + "\' ");
            SqlAddArrowRecord.Append("AND SYMBOL=\'"+req.symbol+"\'");
            SqlAddArrowRecord.Append("AND Lv1=1 ORDER BY id LIMIT 1");
            if (SqlAddArrowRecord.Length > InsertLength)
            {
                index = SqlAddArrowRecord.Length;
                queue.Enqueue(SqlAddArrowRecord.ToString());
                SqlAddArrowRecord.Clear();
            }
            return index;
        }
        int GetFirstGraphNext(string tableName, QueryReq req,int id)
        {
            int index = 0;
            SqlAddArrowRecord.Append("SELECT*FROM");
            SqlAddArrowRecord.Append(tableName);
            SqlAddArrowRecord.Append("WHERE Time >= \'" + req.timeFrom.Year);
            SqlAddArrowRecord.Append("-" + req.timeFrom.Month + "-");
            SqlAddArrowRecord.Append(req.timeFrom.Day + " ");
            SqlAddArrowRecord.Append(req.timeFrom.Hour + ":");
            SqlAddArrowRecord.Append(req.timeFrom.Minute + ":");
            SqlAddArrowRecord.Append(req.timeFrom.Second + "\' ");
            SqlAddArrowRecord.Append("AND Time < \'");
            SqlAddArrowRecord.Append(req.timeFrom.Year);
            SqlAddArrowRecord.Append("-" + req.timeFrom.Month + "-");
            SqlAddArrowRecord.Append(req.timeFrom.Day + " ");
            SqlAddArrowRecord.Append(req.timeFrom.Hour + ":");
            SqlAddArrowRecord.Append(req.timeFrom.Minute + ":");
            SqlAddArrowRecord.Append(req.timeFrom.Second + "\' ");
            SqlAddArrowRecord.Append(" AND id >= " + id);
            SqlAddArrowRecord.Append("AND SYMBOL=\'" + req.symbol + "\'");
            SqlAddArrowRecord.Append("AND Lv1=1 ORDER BY id LIMIT 1");

            //if (SqlAddArrowRecord.Length > InsertLength)
            //{
            //    index = SqlAddArrowRecord.Length;
            //    queue.Enqueue(SqlAddArrowRecord.ToString());
            //    SqlAddArrowRecord.Clear();
            //}
            return index;
        }

      public void LoadParam(QueryReq data)
      {
          
      }
        public void GetChart(int index,DateTime timeFrom, DateTime timeTo,string tableName,string symbolName,bool IsRealTime)
        {
            string sql;
            int count=0;
            int i = 0;
            parse[index].chart=new List<LoadSymbols>();
            sql = "SELECT id FROM " + parse[index].tableName1 + " WHERE Time >= '" + timeFrom.ToString("yyyy-MM-dd HH:mm:ss") + "' AND Time < '" + timeTo.ToString("yyyy-MM-dd HH:mm:ss") + "' AND SYMBOL = '" + parse[index].Symbol1 + "' AND Lvl=1 ORDER BY id limit 1";
            i = GetFirstId(index, sql);
            while (true)
            {                   
             sql = "SELECT Id, Time, TimeMS, Symbol, Currency, Lvl, BidP1, BidQ1, BidP2, BidQ2, BidP3, BidQ3, BidP4, BidQ4, BidP5, BidQ5, AskP1, AskQ1, AskP2, AskQ2, AskP3, AskQ3, AskP4, AskQ4, AskP5, AskQ5 FROM " + parse[index].tableName1 + " WHERE Time >= '" + timeFrom.ToString("yyyy-MM-dd HH:mm:ss") + "' AND Time < '" + timeTo.ToString("yyyy-MM-dd HH:mm:ss") + "' AND SYMBOL = '" + parse[index].Symbol1 + "' AND id >= " + i + " ORDER BY id";
             count = ExecQuery(index, sql);                               
            }
        }

      private int ExecQuery(int index, string sql)
      {
            using (NpgsqlCommand cmd = new NpgsqlCommand(sql))
            {
                cmd.Connection = parse[index].connection;
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
                            //InitSymbolSet(ref symbol, Streams[index].SymbolName, Streams[index].Digits);
                            symbol.Time = DateTime.Parse(reader["Time"].ToString());
                            symbol.Time = symbol.Time.AddMilliseconds(double.Parse(reader["TimeMS"].ToString()));
                            symbol.Level = int.Parse(reader["Lvl"].ToString());
                            for (int i = 0; i < 6 - 1; i++)
                            {
                                symbol.BidPrices[i] = double.Parse(reader["BidP" + (i + 1)].ToString());
                                symbol.AskPrices[i] = double.Parse(reader["AskP" + (i + 1)].ToString());
                                symbol.BidQuantities[i] = int.Parse(reader["BidQ" + (i + 1)].ToString());
                                symbol.AskQuantities[i] = int.Parse(reader["AskQ" + (i + 1)].ToString());
                            }
                            parse[index].chart.Add(symbol);

                            if (count > 100001)
                            {
                                if (parse[index].chart[count].Time < parse[index].chart[count - 1].Time)
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

      private int GetFirstId(int index, string sql)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand(sql))
            {
                cmd.Connection = parse[index].connection;
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
    }
}
