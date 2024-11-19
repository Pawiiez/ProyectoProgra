using SQLite;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ProyectoProgra.Models;

namespace ProyectoProgra
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _database;
        private readonly string _mySqlConnectionString = "server=localhost;userid=root;password=1234;database=proyectoprogra;";
        private readonly string jsonFilePath;

        public DatabaseService(string dbPath, string jsonPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Usuario>().Wait();
            _database.CreateTableAsync<UserReport>().Wait();
            jsonFilePath = jsonPath;
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

        public async Task<int> SaveReportAsync(UserReport report)
        {
            if (report == null)
            {
                throw new ArgumentNullException(nameof(report));
            }
            Console.WriteLine($"Guardando reporte: {report.TipoProblema}, {report.Descripcion}");
            int result = await _database.InsertAsync(report);
            await SaveReportToJsonAsync(report);
            return result;
        }

        public async Task<int> UpdateReportAsync(UserReport report)
        {
            if (report == null)
            {
                throw new ArgumentNullException(nameof(report));
            }
            Console.WriteLine($"Actualizando reporte: {report.TipoProblema}, {report.Descripcion}, {report.Estado_reporte}");
            int result = await _database.UpdateAsync(report);
            await SaveReportToJsonAsync(report);  // Actualiza el JSON también
            return result;
        }

        private async Task SaveReportToJsonAsync(UserReport report)
        {
            try
            {
                List<UserReport> reports;
                if (File.Exists(jsonFilePath))
                {
                    var json = await File.ReadAllTextAsync(jsonFilePath);
                    reports = JsonSerializer.Deserialize<List<UserReport>>(json) ?? new List<UserReport>();
                }
                else
                {
                    reports = new List<UserReport>();
                }

                var existingReport = reports.FirstOrDefault(r => r.Id == report.Id);
                if (existingReport != null)
                {
                    existingReport.Estado_reporte = report.Estado_reporte;
                    existingReport.TipoProblema = report.TipoProblema;
                    existingReport.Descripcion = report.Descripcion;
                    existingReport.Estado = report.Estado;
                    existingReport.Municipio = report.Municipio;
                    existingReport.Colonia = report.Colonia;
                    existingReport.Calle = report.Calle;
                    existingReport.NumeroExterior = report.NumeroExterior;
                }

                var serializedReports = JsonSerializer.Serialize(reports, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(jsonFilePath, serializedReports);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar reporte en JSON: {ex.Message}");
            }
        }

        public Task<List<UserReport>> GetReportsAsync()
        {
            return _database.Table<UserReport>().ToListAsync();
        }

        public Task<List<UserReport>> GetReportsByUserAsync(string email)
        {
            return _database.Table<UserReport>()
                            .Where(r => r.Correo == email)  // Usar Correo en lugar de UserEmail
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
