using Microsoft.Maui.Controls;
using System.Collections.Generic;
using ProyectoProgra.Models;

namespace ProyectoProgra
{
    public partial class ReportStatusPage : ContentPage
    {
        private List<UserReport> reportList;
        private string connectionString = "server=localhost;userid=root;password=1234;database=proyectoprogra;";

        public ReportStatusPage(List<UserReport> reports)
        {
            InitializeComponent();
            reportList = reports;
            ReportsCollectionView.ItemsSource = reportList;
        }
    }
}