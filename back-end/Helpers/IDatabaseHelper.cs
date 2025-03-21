using Microsoft.Data.Sqlite;

namespace back_end.Helpers
{
    public interface IDatabaseHelper
    {
        SqliteConnection GetConnection();
    }
}
