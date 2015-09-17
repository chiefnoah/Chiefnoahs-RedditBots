using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditBots {
    public class DatabaseHandler : AbstractHandler {

        SqliteConnection connection;

        public DatabaseHandler() {
            connection = new SqliteConnection(SQLITE_CONNECTION_STRING);
            connection.Open();
            InitializeDatabase();
        }

        private void InitializeDatabase() {
            string sql = "CREATE TABLE IF NOT EXISTS BotData (id INTEGER PRIMARY KEY AUTOINCREMENT, botId INTEGER, botName TEXT, postID TEXT);";
            sql += "CREATE TABLE IF NOT EXISTS AnimeTitle (id INTEGER PRIMARY KEY AUTOINCREMENT, animeID INTEGER NOT NULL, type TEXT NOT NULL, language TEXT NOT NULL, title TEXT NOT NULL);";
            sql += "CREATE TABLE IF NOT EXISTS CheckedComments (id INTEGER PRIMARY KEY AUTOINCREMENT, botId INTEGER, botName TEXT, commentId TEXT NOT NULL);";

            using (SqliteCommand command = connection.CreateCommand()) {
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
        }

        public SqliteDataReader ExecuteSQLQuery(string sql) {
            using (SqliteCommand command = new SqliteCommand(sql, connection)) {
                return command.ExecuteReader();
            }            
        }

        public void ExecuteSQLNonQuery(string sql) {
            using (SqliteCommand command = new SqliteCommand(sql, connection)) {
                command.ExecuteNonQuery();
            }
        } 

        public SqliteTransaction StartSQLTransaction() {
            return connection.BeginTransaction();
        }

        public SqliteConnection GetConnection() {
            return connection;
        }

        public void Close() {
            connection.Close();
            connection.Dispose();
            connection = null;
        }

    }
}