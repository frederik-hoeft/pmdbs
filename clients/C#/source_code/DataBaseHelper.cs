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
        private const string connectionString = "Data Source=" + @"Resources\localdata_windows.db";

        /// <summary>
        /// Returns result of SQLite database query as a list.
        /// </summary>
        /// <param name="query">SQLite query to be executed.</param>
        /// <param name="columns">Number of by query returned columns.</param>
        /// <returns></returns>
        public static async Task<List<string>> GetDataAsList(string query, int columns)
        {
            List<string> dataList = new List<string>();
            using (DataBaseThreadWatcher watcher = new DataBaseThreadWatcher())
            {
                await watcher.Wait();
                using (SQLiteConnection sqlConnection = new SQLiteConnection(connectionString))
                {
                    sqlConnection.Open();
                    using (SQLiteCommand sqlCommand = sqlConnection.CreateCommand())
                    {
                        sqlCommand.CommandText = query;
                        await sqlCommand.ExecuteNonQueryAsync();
                        using (SQLiteDataReader reader = (SQLiteDataReader)await sqlCommand.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                try
                                {
                                    for (int i = 0; i < columns; i++)
                                    {
                                        dataList.Add(reader[i].ToString());
                                    }
                                }
                                catch (IndexOutOfRangeException outOfRange)
                                {
                                    Console.WriteLine("Error 'Reader out of range' occured: '{0}'", outOfRange);
                                }
                            }
                        }
                    }
                }
            }
            return dataList;
        }
        /// <summary>
        /// Returns result of SQLite database query as 2d string list.
        /// </summary>
        /// <param name="query">SQLite query to be executed.</param>
        /// <param name="columns">Number of by query returned columns.</param>
        /// <returns></returns>
        public static async Task<List<List<string>>> GetDataAs2DList(string query, int columns)
        {
            List<List<string>> outerList = new List<List<string>>();
            using (DataBaseThreadWatcher watcher = new DataBaseThreadWatcher())
            {
                await watcher.Wait();
                using (SQLiteConnection sqlConnection = new SQLiteConnection(connectionString))
                {
                    sqlConnection.Open();
                    using (SQLiteCommand sqlCommand = sqlConnection.CreateCommand())
                    {
                        sqlCommand.CommandText = query;
                        await sqlCommand.ExecuteNonQueryAsync();
                        using (SQLiteDataReader reader = (SQLiteDataReader)await sqlCommand.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                try
                                {
                                    List<string> innerList = new List<string>();
                                    for (int i = 0; i < columns; i++)
                                    {
                                        innerList.Add(reader[i].ToString());
                                    }
                                    outerList.Add(innerList);
                                }
                                catch (IndexOutOfRangeException outOfRange)
                                {
                                    Console.WriteLine("Error 'Reader out of range' occured: '{0}'", outOfRange);
                                }
                            }
                        }
                    }
                }
            }
            return outerList;
        }
        /// <summary>
        /// Returns result of SQLite database query as string or Empty string if 0 or more than one field is returned.
        /// </summary>
        /// <param name="query">SQLite query to be executed.</param>
        /// <returns></returns>
        public static async Task<string> GetSingleOrDefault(string query)
        {
            string returnData = string.Empty;
            using (DataBaseThreadWatcher watcher = new DataBaseThreadWatcher())
            {
                await watcher.Wait();
                using (SQLiteConnection sqlConnection = new SQLiteConnection(connectionString))
                {
                    sqlConnection.Open();
                    using (SQLiteCommand sqlCommand = sqlConnection.CreateCommand())
                    {
                        sqlCommand.CommandText = query;
                        await sqlCommand.ExecuteNonQueryAsync();
                        using (SQLiteDataReader reader = (SQLiteDataReader)await sqlCommand.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                try
                                {
                                    returnData = reader[0].ToString();
                                    break;
                                }
                                catch (IndexOutOfRangeException outOfRange)
                                {
                                    Console.WriteLine("Error 'Reader out of range' occured: '{0}'", outOfRange);
                                }
                            }
                        }
                    }
                }
            }
            return returnData;
        }
        /// <summary>
        /// Returns result of SQLite database query as DataTable Object.
        /// </summary>
        /// <param name="query">SQLite query to be executed.</param>
        /// <param name="columns">Number of by query returned columns.</param>
        /// <returns></returns>
        public static async Task<DataTable> GetDataAsDataTable(string query, int columns)
        {
            DataTable returnData = new DataTable();
            for (int i = 0; i < columns; i++)
            {
                returnData.Columns.Add(i.ToString(), typeof(string));
            }
            using (DataBaseThreadWatcher watcher = new DataBaseThreadWatcher())
            {
                await watcher.Wait();
                using (SQLiteConnection sqlConnection = new SQLiteConnection(connectionString))
                {
                    sqlConnection.Open();
                    using (SQLiteCommand sqlCommand = sqlConnection.CreateCommand())
                    {
                        sqlCommand.CommandText = query;
                        await sqlCommand.ExecuteNonQueryAsync();
                        using (SQLiteDataReader reader = (SQLiteDataReader)await sqlCommand.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                try
                                {
                                    DataRow NewRow = returnData.Rows.Add();
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
                        }
                    }
                }
            }
            return returnData;
        }
        /// <summary>
        /// Executes a SQLite query to manipulate data.
        /// </summary>
        /// <param name="query">SQLite query to be executed.</param>
        /// <returns></returns>
        public static async Task ModifyData(string query)
        {
            using (DataBaseThreadWatcher watcher = new DataBaseThreadWatcher())
            {
                await watcher.Wait();
                using (SQLiteConnection sqlConnection = new SQLiteConnection(connectionString))
                {
                    sqlConnection.Open();
                    using (SQLiteCommand sqlCommand = sqlConnection.CreateCommand())
                    {
                        sqlCommand.CommandText = query;
                        await sqlCommand.ExecuteNonQueryAsync();
                    }
                }
            }
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
                    unsafeQuery[i] = SQLInjectionCheck(unsafeQuery[i]);
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
        Tbl_data = 11,
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
