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
            _parentWindow = parentWindow;
            _currentUser = currentUser;

            LoadCompletedTests(_currentUser.UserType == 4 ? _currentUser.id : (int?)null);

            switch (_currentUser.UserType)
            {
                case 1:
                    ReceptionMenu.Visibility = Visibility.Visible;
                    break;
                case 3:
                    AnalystMenu.Visibility = Visibility.Visible;
                    break;
                case 4:
                    PatientMenu.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void LoadCompletedTests(int? patientId = null)
        {
            try
            {
                var tests = DbManager.GetCompletedTests();

                if (patientId.HasValue)
                {
                    tests = tests.Where(t => t.PatientId == patientId.Value).ToList();
                }

                if (tests != null && tests.Any())
                {
                    var completedTests = tests.Select(t => new
                    {
                        Patient = DbManager.GetUserById(t.PatientId)?.Name + " " + DbManager.GetUserById(t.PatientId)?.Surname,
                        Date = t.Visit?.TimeSlot?.Date.ToString("dd.MM.yyyy"),
                        Time = t.Visit?.TimeSlot?.Time.ToString(@"HH\:mm"),
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
                    if (_currentUser.UserType == 3) //jezeli to analityk
                    {
                        var viewReportWindow = new ShowReportAnalyst(selectedTest, _currentUser, this);
                        viewReportWindow.Show();
                        this.Hide();
                    }
                    else
                    {
                        var viewReportWindow = new ShowReport(selectedTest, _currentUser, this);
                        viewReportWindow.Show();
                        this.Hide();
                    }
                }
                else
                {
                    MessageBox.Show("Nie udało się wczytać danych raportu.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnAllVisits_Click(object sender, RoutedEventArgs e)
        {
            AllVisitsAdmin allVisits = new AllVisitsAdmin(_currentUser);
            allVisits.Show();
            this.Hide();
        }

        private void BtnNewVisit_Click(object sender, RoutedEventArgs e)
        {
            NewVisit newVisit = new NewVisit(_currentUser, this);
            newVisit.Show();
            this.Hide();
        }

        private void BtnSamples_Click(object sender, RoutedEventArgs e)
        {
            Samples samples = new Samples(_currentUser);
            samples.Show();
            this.Hide();
        }

        private void BtnAllExams_Click(object sender, RoutedEventArgs e)
        {
            AllTestsAdmin allTests = new AllTestsAdmin(_currentUser, this);
            allTests.Show();
            this.Hide();
        }

        private void BtnNewExam_Click(object sender, RoutedEventArgs e)
        {
            NewTest newTest = new NewTest(_currentUser, this);
            newTest.Show();
            this.Hide();
        }

        private void BtnAllUsers_Click(object sender, RoutedEventArgs e)
        {
            AllUsers allUsers = new AllUsers(_currentUser);
            allUsers.Show();
            this.Close();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            Registration registration = new Registration(_currentUser);
            registration.Show();
            this.Close();
        }

        private void BtnReports_Click(object sender, RoutedEventArgs e)
        {
            AllReports allReports = new AllReports(_currentUser, this);
            allReports.Show();
            this.Hide();
        }

        private void BtnStats_Click(object sender, RoutedEventArgs e)
        {
            Statistics statistics = new Statistics(_currentUser);
            statistics.Show();
            this.Hide();
        }

        private void BtnExams_Click(object sender, RoutedEventArgs e)
        {
            AllTests allTests = new AllTests(_currentUser, this);
            allTests.Show();
            this.Hide();
        }

        private void BtnSamplesAnalyst_Click(object sender, RoutedEventArgs e)
        {
            SamplesAnalyst samples = new SamplesAnalyst(_currentUser);
            samples.Show();
            this.Hide();
        }


        private void BtnVisits_Click(object sender, RoutedEventArgs e)
        {
            MyVisits allVisits = new MyVisits(_currentUser, this);
            allVisits.Show();
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
