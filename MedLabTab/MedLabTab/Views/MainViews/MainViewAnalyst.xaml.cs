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
            AllTests allTests = new AllTests();
            allTests.Show();
        }

        private void BtnSamples_Click(object sender, RoutedEventArgs e)
        {
            Samples samples = new Samples();
            samples.Show();
        }

        private void BtnReport_Click(object sender, RoutedEventArgs e)
        {
            NewReport newReport = new NewReport();
            newReport.Show();
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            Profile profile = new Profile(currentUser);
            profile.Show();
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