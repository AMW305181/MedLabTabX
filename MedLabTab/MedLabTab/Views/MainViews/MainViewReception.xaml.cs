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
            MainContent.Content = new AllVisits();
        }

        private void BtnAllVisits_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new AllVisits();
        }

        private void BtnNewVisit_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new NewVisit();
        }

        private void BtnAllExams_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new AllTests();
        }

        private void BtnNewExam_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new NewTest();
        }

        private void BtnAllUsers_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new AllUsers();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new Registration();
        }

        private void BtnReports_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new AllReports();
        }

        private void BtnStats_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new Statistics();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }
    }
}