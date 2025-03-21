using Microsoft.Data.Sqlite;

namespace TaskManager.Model.DataContext
{
    public class DataContext : IDisposable
    {
        private readonly string _connectionString;
        private SqliteConnection? _connection;

        public DataContext(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));

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
