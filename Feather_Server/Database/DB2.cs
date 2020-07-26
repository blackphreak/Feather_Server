using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Threading;

namespace Feather_Server.Database
{
    public class DB2
    {
        public DB2()
        {
            try
            {
                string ConnectionStr = "Server=rdr.hkwtc.org; Port=4459; Database=feather; Uid=feather; Pwd=thisisastrongpassword,wasntIt?;";
                MyConnection = new MySqlConnection(ConnectionStr);
                MyConnection.Open();
                Instance = this;
                Console.WriteLine($"[+] Connected to {MyConnection.DataSource}");
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("[-] Could not connect to the MySQL server. Reason: " + ex.Message);
                Environment.Exit(1);
            }
        }

        public MySqlConnection MyConnection { get; private set; } = null;
        private static DB2 Instance = null;

        /// <summary>
        ///   Returns the Database instance or creates a new connection.
        /// </summary>
        /// <returns></returns>
        public static DB2 GetInstance()
        {
            if (Instance == null)
            {
                Instance = new DB2();
            }
            return Instance;
        }

        /// <summary>
        /// Checks the status of the MySQL connection.
        /// </summary>
        /// <returns>true if connected, false if not.</returns>
        public bool isConnected()
        {
            if (MyConnection == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void Close()
        {
            MyConnection.Close();
        }

        public uint getLastInsertedID(string table, string colName)
        {
            var Result = Select("MAX(" + colName + ") AS " + colName, table, null);
            if (Result != null)
                return uint.Parse(Result[0][0] ?? "10000");
            else
                return 0;
        }

        /// <summary>
        /// Executes a SELECT query on the MySQL server.
        /// </summary>
        /// <param name="What">What to select. Eg: * </param>
        /// <param name="Table">FROM which table.</param>
        /// <param name="Condition">WHERE condition, valueName with prefix @. Can be null</param>
        /// <param name="valueSet">Value that will be used in the query. [valueName WITHOUT prefix @, value]</param>
        /// <returns>A dictionary containing the COLUMN name as the key and the ROWS of the columns as List.</returns>
        public List<Dictionary<int, string>> Select(string What, string Table, string Condition = null, Dictionary<string, object> valueSet = null)
        {
            string QFinal = $"SELECT {What} FROM {Table} WHERE {Condition ?? "1"};";
            var cmd = new MySqlCommand(QFinal, MyConnection);
            var result = new List</*rowOfData*/Dictionary</*colIndex*/int, /*data*/string>>();

            try
            {
                if (valueSet != null)
                    foreach (var v in valueSet)
                        cmd.Parameters.AddWithValue($"@{v.Key}", v.Value);

                using var Reader = cmd.ExecuteReader();
                while (Reader.Read())
                {
                    if (!Reader.HasRows || Reader.FieldCount <= 0)
                        return null;

                    var tmp_data = new Dictionary<int, string>();
                    for (int i = 0; i < Reader.FieldCount; i++)
                    {
                        try
                        {
                            tmp_data.Add(i, Reader.GetString(i));
                        }
                        catch (SqlNullValueException)
                        {
                            tmp_data.Add(i, null);
                        }
                    }
                    result.Add(tmp_data);
                }

                return result;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"[-] MySQL Exception: {ex.Message}\n" + ex.StackTrace);
                Console.WriteLine("\tSQL: " + cmd.CommandText);
                return null;
            }
        }

        /// <summary>
        /// Executes an UPDATE query on the database
        /// </summary>
        /// <param name="table">Name of the TABLE to update</param>
        /// <param name="WhatToUpdate">A Dictionary<columnName, row> containing a collection of what to update.</param>
        /// <param name="Condition">WHERE condition, valueName with prefix @. Can be null</param>
        /// <param name="valueSet">Value that will be used in the query. [valueName WITHOUT prefix @, value]</param>
        /// <returns>True if query was successful, false on query error.</returns>
        public bool Update(string table, Dictionary<string, object> WhatToUpdate, string Condition = null, Dictionary<string, object> valueSet = null)
        {
            StringBuilder sb = new StringBuilder($"UPDATE {table} SET ");

            foreach (var key in WhatToUpdate.Keys.SkipLast(1))
                sb.Append($"{key} = @v_{key},");

            var lastKey = WhatToUpdate.Keys.Last();
            sb.Append($"{lastKey} = @v_{lastKey}");

            sb.Append(" WHERE " + Condition);

            var query = new MySqlCommand(sb.ToString(), MyConnection);
            try
            {
                foreach (var obj in WhatToUpdate)
                    query.Parameters.AddWithValue($"@v_{obj.Key}", obj.Value);
                foreach (var obj in valueSet)
                    query.Parameters.AddWithValue($"@{obj.Key}", obj.Value);

                var Res = query.ExecuteNonQueryAsync();
                Console.WriteLine("[+] Rows affected: " + Res.Result);
                
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"[-] MySQL Exception: {ex.Message}\n" + ex.StackTrace);
                Console.WriteLine("\tSQL: " + query.CommandText);
                return false;
            }
        }

        /// <summary>
        /// Executes an INSERT on the database.
        /// </summary>
        /// <param name="table">TABLE name</param>
        /// <param name="What">A dictionary containing <ColumnName, row></param>
        /// <returns>True on success, false on failure.</returns>
        public bool Insert(string table, Dictionary<string, object> What)
        {
            var cmd = new MySqlCommand($"INSERT INTO {table} ({string.Join(",", What.Keys)}) VALUES (@{string.Join(",@", What.Keys)});", MyConnection);
            try
            {
                foreach (var kvp in What)
                    cmd.Parameters.AddWithValue($"@{kvp.Key}", kvp.Value);

                var Res = cmd.ExecuteNonQueryAsync();
                Console.WriteLine("[+] Rows affected: " + Res.Result);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[-] MySQL Exception: {ex.Message}\n" + ex.StackTrace);
                Console.WriteLine("\tSQL: " + cmd.CommandText);
                return false;
            }
        }

        //This is probably not correctly implemented. Did it in a hurry last night. Might only return 1 hero.
        public bool LoginUser(string Username, string Passwd, out List<uint> heroIDs)
        {
            heroIDs = new List<uint>();
            //Empty username, no point in sending query
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Passwd))
                return false;

            var Res = Select(
                "hero1, hero2, hero3, hero4, hero5, hero6", "Login", "username = @uname AND hashedPassword = @passwd LIMIT 1;",
                new Dictionary<string, object>()
                {
                    { "uname", Username },
                    { "passwd", Passwd },
                }
            );

            if (Res == null || Res.Count != 1)
                return false;

            for (int i = 0; i < 6; i++)
                heroIDs.Add(Res[0][i] == null ? 0u : uint.Parse(Res[0][i]));

            return true;

        }



    }
}
