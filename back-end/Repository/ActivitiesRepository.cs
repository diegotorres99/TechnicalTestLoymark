using back_end.DTOs;
using Microsoft.Data.Sqlite;
using back_end.Helpers;

namespace back_end.Repository
{
    public class ActivitiesRepository : IActivitiesRepository
    {
        private readonly IDatabaseHelper _databaseHelper;
        public ActivitiesRepository(IDatabaseHelper databaseHelper)
        {
            _databaseHelper = databaseHelper;
        }
        public async Task<IEnumerable<ActivityDto>> GetAll()
        {
            var activities = new List<ActivityDto>(); 

            try
            {
                await using var connection = _databaseHelper.GetConnection();
                await connection.OpenAsync();

                const string query = @"
                    SELECT a.create_date AS ActivityDate, 
                           u.nombre || ' ' || u.apellido AS FullName, 
                           a.actividad AS ActivityDetail
                    FROM actividades a
                    INNER JOIN usuarios u ON a.id_usuario = u.id
                    ORDER BY a.create_date DESC"
                ; 

                await using var command = new SqliteCommand(query, connection);
                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    activities.Add(new ActivityDto
                    {
                        ActivityDate = reader.GetDateTime(reader.GetOrdinal("ActivityDate")),
                        FullName = reader.GetString(reader.GetOrdinal("FullName")),
                        ActivityDetail = reader.GetString(reader.GetOrdinal("ActivityDetail"))
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return activities; 
        }
    }
}
