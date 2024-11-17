using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.Maui.Controls;

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

            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                var query = "INSERT INTO reporte (TipoProblema, Descripcion, Estado, Municipio, Colonia, Calle, NumeroExterior, Estado_reporte) " +
                            "VALUES (@TipoProblema, @Descripcion, 'Pendiente', @Municipio, @Colonia, @Calle, @NumeroExterior, 'Pendiente')";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TipoProblema", problemType);
                    command.Parameters.AddWithValue("@Descripcion", problemDescription);
                    command.Parameters.AddWithValue("@Municipio", municipio);
                    command.Parameters.AddWithValue("@Colonia", colonia);
                    command.Parameters.AddWithValue("@Calle", calle);
                    command.Parameters.AddWithValue("@NumeroExterior", numeroExterior);

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