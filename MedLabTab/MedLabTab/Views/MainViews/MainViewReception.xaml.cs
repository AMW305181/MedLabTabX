using System.Windows;
using System.Windows.Controls;
using MedLabTab.Views;
using MedLabTab.Views.OtherViews;

namespace MedLabTab.Views.MainViews
{
    public partial class MainViewReception : Window
    {
        public MainViewReception()
        {
            InitializeComponent();
            AllVisits allVisits = new AllVisits();
            allVisits.Show();
        }

        private void BtnAllVisits_Click(object sender, RoutedEventArgs e)
        {
            AllVisits allVisits = new AllVisits();
            allVisits.Show();
        }

        private void BtnNewVisit_Click(object sender, RoutedEventArgs e)
        {   
            NewVisit newVisit = new NewVisit();
            newVisit.Show();
        }

        private void BtnAllExams_Click(object sender, RoutedEventArgs e)
        {
            AllTests allTests = new AllTests();
            allTests.Show();
        }

        private void BtnNewExam_Click(object sender, RoutedEventArgs e)
        {
            NewTest newTest = new NewTest();
            newTest.Show();
        }

        private void BtnAllUsers_Click(object sender, RoutedEventArgs e)
        {
            AllUsers allUsers = new AllUsers();
            allUsers.Show();
            this.Close();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            Registration registration = new Registration();
            registration.Show();
        }

        private void BtnReports_Click(object sender, RoutedEventArgs e)
        {
            AllReports allReports = new AllReports();
            allReports.Show();
        }

        private void BtnStats_Click(object sender, RoutedEventArgs e)
        {
            Statistics statistics = new Statistics();
            statistics.Show();
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