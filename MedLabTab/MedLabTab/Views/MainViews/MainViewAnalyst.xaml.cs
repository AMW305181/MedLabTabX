using System.Windows;
using System.Windows.Controls;
using MedLabTab.ViewModels;
using MedLabTab.Views;
using MedLabTab.Views.OtherViews;

namespace MedLabTab.Views.MainViews
{
    public partial class MainViewAnalyst : Window
    {
        private SignedInUser currentUser;
        public MainViewAnalyst(SignedInUser user)
        {
            InitializeComponent();
            currentUser = user;
        }

        private void BtnExams_Click(object sender, RoutedEventArgs e)
        {
            AllTests allTests = new AllTests(currentUser, this);
            allTests.Show();
            this.Hide();
        }

        private void BtnSamples_Click(object sender, RoutedEventArgs e)
        {
            SamplesAnalyst samples = new SamplesAnalyst(currentUser);
            samples.Show();
            this.Hide();
        }

        private void BtnReport_Click(object sender, RoutedEventArgs e)
        {
            AllReports newReport = new AllReports(currentUser, this);
            newReport.Show();
            this.Hide();
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            Profile profile = new Profile(currentUser, this);
            profile.Show();
            this.Hide();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Czy na pewno chcesz się wylogować?", "Wylogowanie",
                                       MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var loginWindow = new Login();
                loginWindow.Show();
                this.Close();
            }
        }
    }
}