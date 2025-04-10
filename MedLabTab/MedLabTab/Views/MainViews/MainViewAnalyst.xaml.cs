using System.Windows;
using System.Windows.Controls;
using MedLabTab.Views;
using MedLabTab.Views.OtherViews;

namespace MedLabTab.Views.MainViews
{
    public partial class MainViewAnalyst : Window
    {
        public MainViewAnalyst()
        {
            InitializeComponent();
        }

        private void BtnExams_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new AllTests();
        }

        private void BtnSamples_Click(object sender, RoutedEventArgs e)
        {
            // Przejdź do widoku próbek
            MainContent.Content = new Samples();
        }

        private void BtnReport_Click(object sender, RoutedEventArgs e)
        {
            // Przejdź do widoku nowego raportu
            MainContent.Content = new NewReport();
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            // Przejdź do widoku profilu
            MainContent.Content = new Profile();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            // Wylogowanie - wróć do ekranu logowania
            var loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }
    }
}