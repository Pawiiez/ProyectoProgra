using SQLite;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProyectoProgra.Models;

namespace ProyectoProgra
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _database;
        private readonly string _mySqlConnectionString = "server=localhost;userid=root;password=1234;database=proyectoprogra;";

        public DatabaseService(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Usuario>().Wait();
            _database.CreateTableAsync<UserReport>().Wait();
        }

        // Métodos para SQLite
        public Task<List<Usuario>> GetUsersAsync()
        {
            return _database.Table<Usuario>().ToListAsync();
        }

        public Task<Usuario> GetUserAsync(string email)
        {
            return _database.Table<Usuario>()
                            .Where(u => u.Email == email)
                            .FirstOrDefaultAsync();
        }

        public async Task<int> SaveUserAsync(Usuario user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            Console.WriteLine("Guardando usuario en SQLite...");
            int result = await _database.InsertAsync(user);
            Console.WriteLine("Usuario guardado en SQLite con resultado: " + result);
            return result;
        }

        public Task<int> SaveReportAsync(UserReport report)
        {
            if (report == null)
            {
                throw new ArgumentNullException(nameof(report));
            }
            Console.WriteLine($"Guardando reporte: {report.ProblemType}, {report.Description}");
            return _database.InsertOrReplaceAsync(report);
        }

        public Task<List<UserReport>> GetReportsAsync()
        {
            return _database.Table<UserReport>().ToListAsync();
        }

        public Task<List<UserReport>> GetReportsByUserAsync(string email)
        {
            return _database.Table<UserReport>()
                            .Where(r => r.UserEmail == email)
                            .ToListAsync();
        }

        // Métodos para MySQL
        public async Task SaveUserToMySqlAsync(Usuario user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            try
            {
                Console.WriteLine("Guardando usuario en MySQL...");
                using (var connection = new MySqlConnection(_mySqlConnectionString))
                {
                    await connection.OpenAsync();

                    var query = "INSERT INTO usuario (NombreCompleto, Correo, Contrasena, FechaNacimiento) VALUES (@NombreCompleto, @Correo, @Contrasena, @FechaNacimiento)";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NombreCompleto", user.FullName);
                        command.Parameters.AddWithValue("@Correo", user.Email);
                        command.Parameters.AddWithValue("@Contrasena", user.Password);
                        command.Parameters.AddWithValue("@FechaNacimiento", user.BirthDate);

                        await command.ExecuteNonQueryAsync();
                    }
                }
                Console.WriteLine("Usuario guardado en MySQL.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar en MySQL: {ex.Message}");
                throw;
            }
        }
    }
}
