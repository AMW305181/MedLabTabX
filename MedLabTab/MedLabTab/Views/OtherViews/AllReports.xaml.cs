using MedLabTab.DatabaseManager;
using MedLabTab.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MedLabTab.Views.OtherViews
{
    /// <summary>
    /// Logika interakcji dla klasy AllReports.xaml
    /// </summary>
    public partial class AllReports : Window
    {
        private Window _parentWindow;
        public AllReports(Window parentWindow)
        {
            InitializeComponent();
            LoadDoneTests();
            _parentWindow = parentWindow;
        }

        private void LoadDoneTests()
        {
            var completedVisits = DbManager.GetCompletedVisits();
            if (completedVisits != null && completedVisits.Any())
            {
                RaportyDataGrid.ItemsSource = completedVisits;
            }
            else
            {
                MessageBox.Show("Brak zakończonych wizyt do wyświetlenia.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ShowReport_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            Test selectedTest = button?.DataContext as Test;

            if (selectedTest != null)
            {
                var viewReportWindow = new ShowReport(/*selectedTest,*/ this);
                viewReportWindow.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Nie udało się wczytać danych raportu.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void BtnAllVisits_Click(object sender, RoutedEventArgs e)
        {
            AllVisitsAdmin allVisits = new AllVisitsAdmin(this);
            allVisits.Show();
            this.Hide();
        }

        private void BtnNewVisit_Click(object sender, RoutedEventArgs e)
        {
            NewVisit newVisit = new NewVisit(this);
            newVisit.Show();
            this.Hide();
        }

        private void BtnAllExams_Click(object sender, RoutedEventArgs e)
        {
            AllTestsAdmin allTests = new AllTestsAdmin(this);
            allTests.Show();
            this.Hide();
        }

        private void BtnNewExam_Click(object sender, RoutedEventArgs e)
        {
            NewTest newTest = new NewTest(this);
            newTest.Show();
            this.Hide();
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
            this.Close();
        }

        private void BtnReports_Click(object sender, RoutedEventArgs e)
        {
            AllReports allReports = new AllReports(this);
            allReports.Show();
            this.Hide();
        }

        private void BtnStats_Click(object sender, RoutedEventArgs e)
        {
            Statistics statistics = new Statistics();
            statistics.Show();
            this.Close();
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
