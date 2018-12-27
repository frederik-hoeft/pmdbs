using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pmdbs
{
    class DataBaseHelper
    {
        private static SQLiteConnection sql_con;
        private static SQLiteCommand sql_cmd;

        private static void SetConnection()
        {
            string dataSource = "Resources\\account.db";

            sql_con = new SQLiteConnection
            {
                ConnectionString = "Data Source=" + dataSource
            };
            sql_con.Open();
        }
    }
}
