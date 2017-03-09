using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;

namespace BacktestingChart.DataAccess
{
    class BtDbAccess
    {
        readonly Thread _querryThread;
        public bool okRun = true;
        public Queue queue = new Queue();
        private NpgsqlConnection _conn;
        public StringBuilder SqlAddArrowRecord = new StringBuilder();
        public int InsertLength = 10000;
        public BtDbAccess()
        {
           //testconnect
             OpenConnection("test_trade", "vitaliy", "localhost", "5432");
            _querryThread = new Thread(Run_QuerryThread) {IsBackground = true};
            _querryThread.Start();
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
                execNonQuery(auxQuerry);
            }
        }
        private bool OpenConnection(string user, string password, string ip, string port)
        {
            try
            {
                //conn = new NpgsqlConnection("User Id=" + user + ";Password=" + password + ";Data Source=" + ip + ":" + port + "/ORCL;");
                _conn = new NpgsqlConnection("Server=" + ip + ";Port=" + port + ";User Id=" + user + ";Password=" + password + ";Database=i_trade;");
                _conn.Open();

                return (true);
            }
            catch (Exception) //this is your first time ! so,learn to use “try catch” as available as possible! it’s very important!
            {
                return (false);
            }
        }

        public void CloseConnection()
        {
            _conn.Close();
            _conn.Dispose();
        }

        private bool execNonQuery(string sql)
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

                    return true;
                }
                catch (Exception ex) //same table found
                {
                    return false;
                }
            }
        }
        public void CreateArrowsTable(string name)
        {
            //TableNames.Add(name);

            try
            {
                string sqlTable = "DROP TABLE " + name;
                execNonQuery(sqlTable);
                sqlTable = "CREATE TABLE " + name + " (Time timestamp(6) NOT NULL, Price numeric NOT NULL, Direction varchar (4) NOT NULL, Name varchar (2) NOT NULL, TimeMS numeric NOT NULL, Symbol varchar (32) NOT NULL)";
                execNonQuery(sqlTable);
            }
            catch
            {
            }

        }
        public void InsertArrow(string time, decimal price, string direction, string name, int timeMS, string symbol, string tblName)
        {

            SqlAddArrowRecord.Append("INSERT INTO ");
            SqlAddArrowRecord.Append(tblName);

            SqlAddArrowRecord.Append("(Time, Price, Direction, Name, TimeMS, Symbol)");

            SqlAddArrowRecord.Append(" VALUES (");
            //sqlAddRecord.Append(tblName);
            SqlAddArrowRecord.Append("to_timestamp('");

            SqlAddArrowRecord.Append(time + "' ,'yyyyMMdd-hh24:MI:ss')");
            SqlAddArrowRecord.Append(",");

            SqlAddArrowRecord.Append(price.ToString());
            SqlAddArrowRecord.Append(",'");
            SqlAddArrowRecord.Append(direction);

            SqlAddArrowRecord.Append("','");
            SqlAddArrowRecord.Append(name);
            SqlAddArrowRecord.Append("',");


            SqlAddArrowRecord.Append(timeMS.ToString());
            SqlAddArrowRecord.Append(",'");
            SqlAddArrowRecord.Append(symbol);
            SqlAddArrowRecord.Append("'");
            SqlAddArrowRecord.Append(");");
            if (SqlAddArrowRecord.Length > InsertLength)
            {
                queue.Enqueue(SqlAddArrowRecord.ToString());
                SqlAddArrowRecord.Clear();
            }
            //execNonQuery(sqlAddArrowRecord.ToString());

        }

    }
}
