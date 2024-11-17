using Microsoft.Maui;
using Microsoft.Maui.Controls;
using System.IO;

namespace ProyectoProgra
{
    public partial class App : Application
    {
        public static DatabaseService Database { get; private set; }

        public App()
        {
            InitializeComponent();


            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "reports.db3");
            Database = new DatabaseService(dbPath);

            MainPage = new NavigationPage(new MainPage());
        }
    }
}