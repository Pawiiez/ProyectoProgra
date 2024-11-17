using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using ProyectoProgra.Models;
using System.Linq;

namespace ProyectoProgra
{
    public partial class AdminPage : ContentPage
    {
        private ObservableCollection<UserReport> reports;
        private ObservableCollection<UserReport> filteredReports;

        public AdminPage()
        {
            InitializeComponent();

            // Inicializar el campo reports
            reports = new ObservableCollection<UserReport>();

            LoadReports();
        }

        private async void LoadReports()
        {
            var reportList = await App.Database.GetReportsAsync();
            reports = new ObservableCollection<UserReport>(reportList);
            filteredReports = new ObservableCollection<UserReport>(reports);
            ReportsCollectionView.ItemsSource = filteredReports;
        }

        private void OnStatusPickerChanged(object sender, EventArgs e)
        {
            var selectedStatus = StatusPicker.SelectedItem?.ToString();

            if (selectedStatus == "Todos")
            {
                filteredReports = new ObservableCollection<UserReport>(reports);
            }
            else
            {
                filteredReports = new ObservableCollection<UserReport>(reports.Where(r => r.Status == selectedStatus));
            }

            ReportsCollectionView.ItemsSource = filteredReports;
        }

        private async void OnSaveChangesButtonClicked(object sender, EventArgs e)
        {
            foreach (var report in reports)
            {
                await App.Database.SaveReportAsync(report);
            }
            await DisplayAlert("Éxito", "Los cambios se han guardado.", "OK");
        }
    }
}
