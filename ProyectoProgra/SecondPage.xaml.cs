using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.Maui.Controls;
using ProyectoProgra.Models;

namespace ProyectoProgra
{
    public partial class SecondPage : ContentPage
    {
        private string userEmail;
        private string connectionString = "server=localhost;userid=root;password=1234;database=proyectoprogra;";

        public SecondPage(string email)
        {
            InitializeComponent();
            userEmail = email;
        }

        async void OnReportButtonClicked(object sender, EventArgs e)
        {
            var problemType = problemTypePicker.SelectedItem?.ToString();
            var problemDescription = descriptionEditor.Text;
            var municipio = MunicipioEntry.Text;
            var colonia = ColoniaEntry.Text;
            var calle = CalleEntry.Text;
            var numeroExterior = NumeroExteriorEntry.Text;

            if (string.IsNullOrEmpty(problemType) || string.IsNullOrEmpty(problemDescription) || string.IsNullOrEmpty(municipio) || string.IsNullOrEmpty(colonia) || string.IsNullOrEmpty(calle))
            {
                await DisplayAlert("Error", "Por favor completa todos los campos.", "OK");
                return;
            }

            var newReport = new UserReport
            {
                Correo = userEmail,  // Usar Correo en lugar de UserEmail
                TipoProblema = problemType,
                Descripcion = problemDescription,
                Estado = "Pendiente",
                Municipio = municipio,
                Colonia = colonia,
                Calle = calle,
                NumeroExterior = numeroExterior,
                Estado_reporte = "Pendiente"
            };

            // Guardar en SQLite y JSON
            await App.Database.SaveReportAsync(newReport);

            // Guardar en MySQL
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                var query = "INSERT INTO reporte (Correo, TipoProblema, Descripcion, Estado, Municipio, Colonia, Calle, NumeroExterior, Estado_reporte) " +
                            "VALUES (@Correo, @TipoProblema, @Descripcion, @Estado, @Municipio, @Colonia, @Calle, @NumeroExterior, @Estado_reporte)";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Correo", userEmail);
                    command.Parameters.AddWithValue("@TipoProblema", problemType);
                    command.Parameters.AddWithValue("@Descripcion", problemDescription);
                    command.Parameters.AddWithValue("@Estado", "Pendiente");
                    command.Parameters.AddWithValue("@Municipio", municipio);
                    command.Parameters.AddWithValue("@Colonia", colonia);
                    command.Parameters.AddWithValue("@Calle", calle);
                    command.Parameters.AddWithValue("@NumeroExterior", numeroExterior);
                    command.Parameters.AddWithValue("@Estado_reporte", "Pendiente");

                    await command.ExecuteNonQueryAsync();
                }
            }

            await DisplayAlert("Reporte Enviado", $"Problema: {problemType}\nDescripción: {problemDescription}\nEstado: {municipio} \nMunicipio: {municipio} \nColonia: {colonia} \nCalle: {calle} \nNúmero Exterior: {numeroExterior}", "OK");

            // Refrescar la página de reportes
            var reports = await App.Database.GetReportsByUserAsync(userEmail);
            await Navigation.PushAsync(new ReportStatusPage(reports));
        }

        async void OnViewReportStatusClicked(object sender, EventArgs e)
        {
            var reports = await App.Database.GetReportsByUserAsync(userEmail);
            await Navigation.PushAsync(new ReportStatusPage(reports));
        }
    }
}
