using Microsoft.Data.Sqlite;

namespace back_end.Helpers
{
    public class DatabaseHelper : IDatabaseHelper, IDisposable
    {
        private readonly string _connectionString;
        private SqliteConnection? _connection;

        public DatabaseHelper(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            _connectionString = connectionString;
        }

        public SqliteConnection GetConnection()
        {
            if (_connection == null)
            {
                _connection = new SqliteConnection(_connectionString);
                _connection.Open();
            }
            return _connection;
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    _connection.Close();
                }
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}
