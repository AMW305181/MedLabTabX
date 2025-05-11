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
        private User _currentUser;
        public AllReports(User currentUser, Window parentWindow)
        {
            InitializeComponent();
            LoadCompletedTests();
            _parentWindow = parentWindow;
            _currentUser = currentUser;
        }

        private void LoadCompletedTests()
        {
            try
            {
                var tests = DbManager.GetCompletedTests();

                if (tests != null && tests.Any())
                {
                    var completedTests = tests.Select(t => new
                    {
                        Patient = DbManager.GetUserById(t.PatientId)?.Name + " " + DbManager.GetUserById(t.PatientId)?.Surname,
                        Date = t.Visit?.TimeSlot?.Date,
                        Time = t.Visit?.TimeSlot?.Time,
                        Test = t.Test?.TestName,
                        OriginalTest = t,
                    }).ToList();

                    RaportyDataGrid.ItemsSource = completedTests;
                }
                else
                {
                    MessageBox.Show("Brak wykonanych badań do wyświetlenia.", "Informacja",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas ładowania danych: {ex.Message}", "Błąd",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowReport_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                dynamic item = button.DataContext;

                TestHistory selectedTest = item?.OriginalTest as TestHistory;

                if (selectedTest != null)
                {
                    var viewReportWindow = new ShowReport(selectedTest, _currentUser, this);
                    viewReportWindow.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Nie udało się wczytać danych raportu.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void BtnExams_Click(object sender, RoutedEventArgs e)
        {
            AllTests allTests = new AllTests(_currentUser, this);
            allTests.Show();
            this.Hide();
        }

        private void BtnSamples_Click(object sender, RoutedEventArgs e)
        {
            SamplesAnalyst samples = new SamplesAnalyst(_currentUser);
            samples.Show();
            this.Hide();
        }

        private void BtnReport_Click(object sender, RoutedEventArgs e)
        {
            AllReports newReport = new AllReports(_currentUser, this);
            newReport.Show();
            this.Hide();
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            Profile profile = new Profile(_currentUser, this);
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
