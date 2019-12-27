using System.Linq;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using System;

namespace Feather_Server.Database
{
    public class DB
    {
        private static DB _instance = null;
        private static SQLiteConnection conn;

        public DB()
        {
            try
            {
                string cs = @$"URI=file:{Lib.dbPath}/user.db3";

                conn = new SQLiteConnection(cs);
                conn.Open();

                Console.WriteLine($"[+] Database Ready.");
            }
            catch (Exception e)
            {
                Console.WriteLine("[-] Critical Error. Failed to open database file!");
                Console.WriteLine($"\t Detail:");
                Console.WriteLine($"\t  [Msg]: {e.Message}");
                Console.WriteLine($"\t[Trace]: {e.StackTrace}");
                Console.WriteLine("-- Press ENTER to exit --");
                Console.ReadLine();
                Environment.Exit(1);
            }

            _instance = this;
        }

        public bool userLogin(string username, string hashedPassword, out List<int> heroInfos)
        {
            SQLiteDataReader rdr;
            select("SELECT * FROM Login WHERE username = @username AND hashed = @hashed;",
                new Dictionary<string, object>()
                {
                    { "@username", username },
                    { "@hashed", hashedPassword },
                },
                out rdr);

            heroInfos = new List<int>();
            if (!rdr.HasRows)
            {
                return false;
            }
            else
            {
                rdr.Read();

                byte i = 4;

                object hero = rdr.GetValue(i++);
                while (i < rdr.FieldCount && !(hero is DBNull))
                {
                    heroInfos.Add((int)((long)hero & 0xFFFFFFFF));
                    hero = rdr.GetValue(i++);
                }

                return true;
            }
        }

        public void select(string sql, Dictionary<string, object> conditions, out SQLiteDataReader dataReader)
        {
            using var cmd = new SQLiteCommand(conn);

            if (conditions != null)
            {
                var cols = new StringBuilder();
                int idx = conditions.Count - 1;
                conditions.ToList().ForEach(kvp =>
                {
                    cmd.Parameters.AddWithValue($"{kvp.Key}", kvp.Value);
                    idx--;
                });
            }

            cmd.CommandText = sql;
            cmd.Prepare();

            dataReader = cmd.ExecuteReader();
        }

        public long insert(string table, Dictionary</*col*/string, /*data*/object> data)
        {
            using var cmd = new SQLiteCommand(conn);

            StringBuilder sb = new StringBuilder();
            int idx = data.Count - 1;

            data.ToList().ForEach(kvp =>
            {
                sb.Append($"@{kvp.Key}{(idx == 0 ? "" : ", ")}");
                cmd.Parameters.AddWithValue($"@{kvp.Key}", kvp.Value);
                idx--;
            });
            var cols = sb.ToString();

            cmd.CommandText = $"INSERT INTO {table}({cols.Replace("@", "")}) VALUES({cols})";
            cmd.Prepare();

            cmd.ExecuteNonQuery();

            return conn.LastInsertRowId;
        }

        public void update(string sql, Dictionary</*col*/string, /*data*/object> data, Dictionary</*col*/string, /*data*/object> conditions)
        {
            using var cmd = new SQLiteCommand(conn);

            StringBuilder sb = new StringBuilder();
            int idx = data.Count - 1;

            data.ToList().ForEach(kvp =>
            {
                sb.Append($"{kvp.Key} = @{kvp.Key}{(idx == 0 ? "" : ", ")}");
                cmd.Parameters.AddWithValue($"@{kvp.Key}", kvp.Value);
                idx--;
            });
            var cols = sb.ToString();

            conditions.ToList().ForEach(kvp =>
            {
                cmd.Parameters.AddWithValue($"{kvp.Key}", kvp.Value);
                idx--;
            });

            cmd.CommandText = string.Format(sql, cols);
            cmd.Prepare();

            cmd.ExecuteNonQuery();
        }

        public long getLastInsertedID(string table, string colName)
        {
            using var cmd_id = new SQLiteCommand(conn);
            cmd_id.CommandText = $"select MAX({colName}) as lastID from {table};";
            var id = cmd_id.ExecuteScalar();

            if (id is DBNull)
                return 0;

            return (long)id;
        }

        public static DB getInstance()
        {
            return _instance ?? new DB();
        }
    }
}
