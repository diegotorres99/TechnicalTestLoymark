using Microsoft.Data.Sqlite;
using back_end.DTOs;
using back_end.Helpers;
using System.Text.Json;

namespace back_end.Repository
{
    public class UsersRepository : IUsersRepository
    {
        private readonly IDatabaseHelper _databaseHelper;
        private readonly HttpClient _httpClient;
        public UsersRepository(IDatabaseHelper databaseHelper, HttpClient httpClient)
        {
            _databaseHelper = databaseHelper;
            _httpClient = httpClient;
        }

        public async Task<UserDto> Create(UserDto userDto)
        {
            try
            {
                using (var connection = _databaseHelper.GetConnection())
                {
                    await connection.OpenAsync();

                    const string query = @"
                    INSERT INTO usuarios (nombre, apellido, correo_electronico, fecha_nacimiento, 
                                         telefono, pais_residencia, pregunta_contacto)
                    VALUES (@nombre, @apellido, @correo, @fecha_nacimiento, @telefono, @pais, @pregunta_contacto);
                    SELECT last_insert_rowid();";  

                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@nombre", userDto.Name);
                        command.Parameters.AddWithValue("@apellido", userDto.LastName);
                        command.Parameters.AddWithValue("@correo", userDto.Email);
                        command.Parameters.AddWithValue("@fecha_nacimiento", userDto.BirthDate.ToString("yyyy-MM-dd"));
                        command.Parameters.AddWithValue("@telefono", (object?)userDto.Phone ?? DBNull.Value);
                        command.Parameters.AddWithValue("@pais", userDto.Country);
                        command.Parameters.AddWithValue("@pregunta_contacto", userDto.ContactQuestion);

                        userDto.Id = Convert.ToInt32(await command.ExecuteScalarAsync()); // Retrieve inserted ID
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating user: {ex.Message}", ex);
            }

            return userDto;
        }

        public async Task<IEnumerable<UserDto>> GetAll()
        {
            var users = new List<UserDto>();

            try
            {
                using (var connection = _databaseHelper.GetConnection())
                {
                    await connection.OpenAsync();

                    const string query = "SELECT Id, Name FROM Users";

                    using (var command = new SqliteCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                users.Add(new UserDto
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching users: {ex.Message}", ex);
            }

            return users;
        }

        public async Task<UserDto?> GetById(int id)
        {
            try
            {
                using (var connection = _databaseHelper.GetConnection())
                {
                    await connection.OpenAsync();

                    const string query = @"
                    SELECT id, nombre, apellido, correo_electronico, fecha_nacimiento, 
                           telefono, pais_residencia, pregunta_contacto
                    FROM usuarios WHERE id = @id";

                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new UserDto
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                                    Name = reader.GetString(reader.GetOrdinal("nombre")),
                                    LastName = reader.GetString(reader.GetOrdinal("apellido")),
                                    Email = reader.GetString(reader.GetOrdinal("correo_electronico")),
                                    BirthDate = DateTime.Parse(reader.GetString(reader.GetOrdinal("fecha_nacimiento"))),
                                    Phone = reader.IsDBNull(reader.GetOrdinal("telefono"))
                                        ? null : reader.GetString(reader.GetOrdinal("telefono")),
                                    Country = reader.GetString(reader.GetOrdinal("pais_residencia")),
                                    ContactQuestion = reader.GetBoolean(reader.GetOrdinal("pregunta_contacto"))
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching user by ID: {ex.Message}", ex);
            }

            return null;
        }

        public async Task<bool> Update(UserDto userDto)
        {
            try
            {
                using (var connection = _databaseHelper.GetConnection())
                {
                    await connection.OpenAsync();

                    const string query = @"
                    UPDATE usuarios 
                    SET nombre = @nombre, 
                        apellido = @apellido, 
                        correo_electronico = @correo, 
                        fecha_nacimiento = @fecha_nacimiento, 
                        telefono = @telefono, 
                        pais_residencia = @pais, 
                        pregunta_contacto = @pregunta_contacto
                    WHERE id = @id";

                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", userDto.Id);
                        command.Parameters.AddWithValue("@nombre", userDto.Name);
                        command.Parameters.AddWithValue("@apellido", userDto.LastName);
                        command.Parameters.AddWithValue("@correo", userDto.Email);
                        command.Parameters.AddWithValue("@fecha_nacimiento", userDto.BirthDate.ToString("yyyy-MM-dd"));
                        command.Parameters.AddWithValue("@telefono", (object?)userDto.Phone ?? DBNull.Value);
                        command.Parameters.AddWithValue("@pais", userDto.Country);
                        command.Parameters.AddWithValue("@pregunta_contacto", userDto.ContactQuestion);

                        var rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating user: {ex.Message}", ex);
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                using (var connection = _databaseHelper.GetConnection())
                {
                    await connection.OpenAsync();

                    const string query = "DELETE FROM usuarios WHERE id = @id";

                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        var rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting user: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<CountryDto>> GetCountries()
        {
            var countries = new List<CountryDto>();

            try
            {
                var response = await _httpClient.GetAsync("https://restcountries.com/v3.1/all");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    var countryData = JsonSerializer.Deserialize<List<CountryApiResponse>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<CountryApiResponse>(); 

                    if (countryData.Count > 0) 
                    {
                        countries = countryData
                            .Where(c => !string.IsNullOrEmpty(c.cca3) && c.name?.common != null)
                            .Select(c => new CountryDto
                            {
                                Code = c.cca3 ?? "N/A", 
                                Name = c.name?.common ?? "Unknown"  
                            })
                            .OrderBy(c => c.Name)
                            .ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching country list: {ex.Message}");
            }

            return countries;
        }
    }
}


