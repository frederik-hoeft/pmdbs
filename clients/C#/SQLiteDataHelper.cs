using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;

namespace PasswordManager
{
    class SQLiteDataHelper
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
        public static async Task<DataTable> GetAllData(string txtQueryGetAllData)
        {
            SetConnection();
            sql_cmd = sql_con.CreateCommand();
            sql_cmd.CommandText = txtQueryGetAllData;
            await sql_cmd.ExecuteNonQueryAsync();
            SQLiteDataReader reader = sql_cmd.ExecuteReader();
            var StringTableResult = new DataTable();
            StringTableResult.Columns.Add("ID", typeof(string));
            StringTableResult.Columns.Add("Hostname", typeof(string));
            StringTableResult.Columns.Add("Url", typeof(string));
            StringTableResult.Columns.Add("Username", typeof(string));
            StringTableResult.Columns.Add("Email", typeof(string));
            StringTableResult.Columns.Add("Password", typeof(string));
            while (reader.Read())
            {
                try
                {
                    StringTableResult.Rows.Add(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString());
                }
                catch (Exception readerOutOfRange)
                {
                    Console.WriteLine("Error 'Reader out of range' occured: '{0}'", readerOutOfRange);
                }
            }
            sql_con.Close();
            sql_con.Dispose();
            return StringTableResult;
        }
        public static async Task ModifyData(string txtQueryModifyData)
        {
            SetConnection();
            sql_cmd = sql_con.CreateCommand();
            sql_cmd.CommandText = txtQueryModifyData;
            await sql_cmd.ExecuteNonQueryAsync();
            sql_con.Close();
            sql_con.Dispose();
        }
        public static async Task<List<String>> ReturnSingleColumn(string txtQuerySingleColumn)
        {
            SetConnection();
            sql_cmd = sql_con.CreateCommand();
            sql_cmd.CommandText = txtQuerySingleColumn;
            await sql_cmd.ExecuteNonQueryAsync();
            SQLiteDataReader reader = sql_cmd.ExecuteReader();
            var StringListResult = new List<String>();
            while (reader.Read())
            {
                try
                {
                    StringListResult.Add(reader[0].ToString());
                }
                catch (Exception readerOutOfRange)
                {
                    Console.WriteLine("Error 'Reader out of range' occured: '{0}'", readerOutOfRange);
                }
            }
            sql_con.Close();
            sql_con.Dispose();
            return StringListResult;
        }
    }
}
