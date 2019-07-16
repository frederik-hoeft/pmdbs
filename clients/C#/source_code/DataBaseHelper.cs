using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pmdbs
{
    /// <summary>
    /// Provides thread-safe database access
    /// </summary>
    class DataBaseHelper
    {
        private static SQLiteConnection sql_con;
        private static SQLiteCommand sql_cmd;
        /// <summary>
        /// Opens the connection to the database
        /// </summary>
        /// <returns></returns>
        private static async Task SetConnection()
        {
            while (GlobalVarPool.databaseIsInUse)
            {
                await Task.Delay(100);
            }
            GlobalVarPool.databaseIsInUse = true;
            string dataSource = @"Resources\localdata_windows.db";

            sql_con = new SQLiteConnection
            {
                ConnectionString = "Data Source=" + dataSource
            };
            sql_con.Open();
        }

        /// <summary>
        /// Returns result of SQLite database query as a list.
        /// </summary>
        /// <param name="query">SQLite query to be executed.</param>
        /// <param name="columns">Number of by query returned columns.</param>
        /// <returns></returns>
        public static async Task<List<string>> GetDataAsList(string query, int columns)
        {
            await SetConnection();
            sql_cmd = sql_con.CreateCommand();
            sql_cmd.CommandText = query;
            await sql_cmd.ExecuteNonQueryAsync();
            SQLiteDataReader reader = sql_cmd.ExecuteReader();
            List<string> DataList = new List<string>();
            while (reader.Read())
            {
                try
                {
                    for (int i = 0; i < columns; i++)
                    {
                        DataList.Add(reader[i].ToString());
                    }
                }
                catch (IndexOutOfRangeException outOfRange)
                {
                    Console.WriteLine("Error 'Reader out of range' occured: '{0}'", outOfRange);
                }
            }
            sql_con.Close();
            sql_con.Dispose();
            GlobalVarPool.databaseIsInUse = false;
            return DataList;
        }
        /// <summary>
        /// Returns result of SQLite database query as 2d string list.
        /// </summary>
        /// <param name="query">SQLite query to be executed.</param>
        /// <param name="columns">Number of by query returned columns.</param>
        /// <returns></returns>
        public static async Task<List<List<string>>> GetDataAs2DList(string query, int columns)
        {
            await SetConnection();
            sql_cmd = sql_con.CreateCommand();
            sql_cmd.CommandText = query;
            await sql_cmd.ExecuteNonQueryAsync();
            SQLiteDataReader reader = sql_cmd.ExecuteReader();
            List<List<string>> OuterList = new List<List<string>>();
            while (reader.Read())
            {
                try
                {
                    List<string> InnerList = new List<string>();
                    for (int i = 0; i < columns; i++)
                    {
                        InnerList.Add(reader[i].ToString());
                    }
                    OuterList.Add(InnerList);
                }
                catch (IndexOutOfRangeException outOfRange)
                {
                    Console.WriteLine("Error 'Reader out of range' occured: '{0}'", outOfRange);
                }
            }
            sql_con.Close();
            sql_con.Dispose();
            GlobalVarPool.databaseIsInUse = false;
            return OuterList;
        }
        /// <summary>
        /// Returns result of SQLite database query as string or Empty string if 0 or more than one field is returned.
        /// </summary>
        /// <param name="query">SQLite query to be executed.</param>
        /// <returns></returns>
        public static async Task<string> GetSingleOrDefault(string query)
        {
            await SetConnection();
            sql_cmd = sql_con.CreateCommand();
            sql_cmd.CommandText = query;
            await sql_cmd.ExecuteNonQueryAsync();
            SQLiteDataReader reader = sql_cmd.ExecuteReader();
            string returnData = string.Empty;
            int i = 0;
            while (reader.Read())
            {
                try
                {
                    returnData = reader[0].ToString();
                    i++;
                }
                catch (IndexOutOfRangeException outOfRange)
                {
                    Console.WriteLine("Error 'Reader out of range' occured: '{0}'", outOfRange);
                }
            }
            sql_con.Close();
            sql_con.Dispose();
            GlobalVarPool.databaseIsInUse = false;
            return i != 1 ? string.Empty : returnData; 
        }
        /// <summary>
        /// Returns result of SQLite database query as DataTable Object.
        /// </summary>
        /// <param name="query">SQLite query to be executed.</param>
        /// <param name="columns">Number of by query returned columns.</param>
        /// <returns></returns>
        public static async Task<DataTable> GetDataAsDataTable(string query, int columns)
        {
            await SetConnection();
            sql_cmd = sql_con.CreateCommand();
            sql_cmd.CommandText = query;
            await sql_cmd.ExecuteNonQueryAsync();
            SQLiteDataReader reader = sql_cmd.ExecuteReader();
            DataTable ReturnData = new DataTable();
            for (int i = 0; i < columns; i++)
            {
                ReturnData.Columns.Add(i.ToString(), typeof(string));
            }
            while (reader.Read())
            {
                try
                {
                    DataRow NewRow = ReturnData.Rows.Add();
                    for (int i = 0; i < columns; i++)
                    {
                        NewRow[i.ToString()] = reader[i].ToString();
                    }
                }
                catch (IndexOutOfRangeException outOfRange)
                {
                    Console.WriteLine("Error 'Reader out of range' occured: '{0}'", outOfRange);
                }
            }
            sql_con.Close();
            sql_con.Dispose();
            GlobalVarPool.databaseIsInUse = false;
            return ReturnData;
        }
        /// <summary>
        /// Executes a SQLite query to manipulate data.
        /// </summary>
        /// <param name="query">SQLite query to be executed.</param>
        /// <returns></returns>
        public static async Task ModifyData(string query)
        {
            await SetConnection();
            sql_cmd = sql_con.CreateCommand();
            sql_cmd.CommandText = query;
            await sql_cmd.ExecuteNonQueryAsync();
            sql_con.Close();
            sql_con.Dispose();
            GlobalVarPool.databaseIsInUse = false;
        }
        /// <summary>
        /// Provides database security related methods.
        /// </summary>
        public static class Security
        {
            /// <summary>
            /// Turns an unsafe string into a SQL injection safe string.
            /// </summary>
            /// <param name="unsafeString">The string to check.</param>
            /// <returns>SQLI safe string.</returns>
            public static string SQLInjectionCheck(string unsafeString)
            {
                return unsafeString.Replace("\'", "\'\'").Replace("\"", "\"\"");
            }
            /// <summary>
            /// Turns an unsafe SQL query into a SQL injection safe query.
            /// </summary>
            /// <param name="unsafeQuery">The SQL query to check. NOTE: Every even idex is considered to be a SQL statement.</param>
            /// <returns>SQLI safe query.</returns>
            public static string SQLInjectionCheckQuery(string[] unsafeQuery)
            {
                int queryLength = unsafeQuery.Length;
                if (queryLength == 0 || queryLength % 2 == 0)
                {
                    throw new ArgumentException("There must be an odd number of arguments.");
                }
                for (int i = 1; i < queryLength; i += 2)
                {
                    unsafeQuery[i] = unsafeQuery[i].Replace("\'", "\'\'").Replace("\"", "\"\"");
                }
                string query = string.Empty;
                for (int i = 0; i < queryLength; i++)
                {
                    query += unsafeQuery[i];
                }
                return query;
            }
        }
    }
    /// <summary>
    /// Provides the amount of columns of database Tables.
    /// </summary>
    public enum ColumnCount
    {
        /// <summary>
        /// Gets the number of columns in Tbl_user.
        /// </summary>
        Tbl_user = 8,
        /// <summary>
        /// Gets the number of columns in Tbl_data.
        /// </summary>
        Tbl_data = 10,
        /// <summary>
        /// Gets the number of columns in Tbl_commonPasswords.
        /// </summary>
        Tbl_commonPasswords = 2,
        /// <summary>
        /// Gets the number of columns in Tbl_settings.
        /// </summary>
        Tbl_settings = 5,
        /// <summary>
        /// Gets the number of columns in Tbl_delete.
        /// </summary>
        Tbl_delete = 2,
        /// <summary>
        /// Returns 1.
        /// </summary>
        SingleColumn = 1
    }
}
