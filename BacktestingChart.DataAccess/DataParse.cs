using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;
namespace BacktestingChart.DataAccess
{
   public class DataParse
    {
        public DateTime dateTimeFor, dateTimeEnd;
        public NpgsqlConnection connection;
        public Thread thread;
        public List<ChDbAccess.LoadSymbols> chart = new List<ChDbAccess.LoadSymbols>();
        public string Symbol1;
        public string Symbol2;
        public string tableName1;
        public string tableName2;
        public bool IsRealTime;
       

    }
}
