using MedLabTab.DatabaseManager;
using MedLabTab.DatabaseModels;
using MedLabTab.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for Samples.xaml
    /// </summary>
    public partial class Samples : Window
    {
        private SignedInUser _currentUser;
        public Samples(SignedInUser currentUser)
        {
            InitializeComponent();
            LoadSamples();
            _currentUser = currentUser;
        }
        public void LoadSamples()
        {
            var tests = DbManager.GetAllTestHistories();

            if (tests != null)
            {
                var samples = tests.Select(t => new
                {
                    TestName = t.Test?.TestName,
                    Date = t.Visit?.TimeSlot?.Date.ToString("dd.MM.yyyy"),
                    Time = t.Visit?.TimeSlot?.Time.ToString(@"HH\:mm"),
                    Patient = $"{t.Patient.Name} {t.Patient.Surname}",
                    TestCategory = t.Test?.CategoryNavigation?.CategoryName,
                    Status = t.StatusNavigation.StatusName, 
                    OriginalTest = t
                }).ToList();

                SamplesDataGrid.ItemsSource = samples;
            }
            else
            {
                MessageBox.Show("Błąd podczas ładowania danych.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAllVisits_Click(object sender, RoutedEventArgs e)
        {
            AllVisitsAdmin allVisits = new AllVisitsAdmin(_currentUser);
            allVisits.Show();
            this.Hide();
        }
        //na newVisitAdmin
        private void BtnNewVisit_Click(object sender, RoutedEventArgs e)
        {
            NewVisitAdmin newVisit = new NewVisitAdmin(_currentUser, this);
            newVisit.Show();
            this.Hide();
        }

        private void BtnSchedule_Click(object sender, RoutedEventArgs e)
        {
            EditSchedule editschedule = new EditSchedule(_currentUser);
            editschedule.Show();
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
