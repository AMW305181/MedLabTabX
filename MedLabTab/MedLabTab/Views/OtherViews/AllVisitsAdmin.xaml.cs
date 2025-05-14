using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using MedLabTab.DatabaseManager;
using MedLabTab.DatabaseModels;
using static System.Net.Mime.MediaTypeNames;

namespace MedLabTab.Views.OtherViews
{
    public partial class AllVisitsAdmin : Window
    {
        private User _currentUser;
        private List<dynamic> _allVisits;  
        private List<dynamic> _filteredVisits;

        public AllVisitsAdmin(User currentUser)
        {
            InitializeComponent();
            LoadVisits(); // Załaduj dane po inicjalizacji okna
            _currentUser = currentUser;
            txtSearch.TextChanged += TxtSearch_TextChanged;
            switch (_currentUser.UserType)
            {
                case 1:
                    ReceptionMenu.Visibility = Visibility.Visible;
                    break;
                case 2:
                    NurseMenu.Visibility = Visibility.Visible;
                    Actions.Visibility = Visibility.Collapsed;
                    break;
            }
        }
        public void LoadVisits()
        {
            var visits = DbManager.GetAllVisits();

            if (visits != null)
            {
                _allVisits = visits.Select(v => new
                {
                    Date = DbManager.GetSchedule(v.TimeSlotId.Value)?.Date.ToString("dd.MM.yyyy"),
                    Time = DbManager.GetSchedule(v.TimeSlotId.Value)?.Time.ToString(@"HH\:mm"),
                    Tests = string.Join(", ", DbManager.GetTestsInVisit(v.id)
                        .Select(th => DbManager.GetTest(th.TestId))
                        .Where(test => test != null && !string.IsNullOrEmpty(test.TestName))
                        .Select(test => test.TestName)),
                    Cost = v.Cost,
                    Patient = DbManager.GetUserById(v.PatientId)?.Name + " " + DbManager.GetUserById(v.PatientId)?.Surname,
                    Nurse = DbManager.GetUserById(DbManager.GetSchedule(v.TimeSlotId.Value).NurseId)?.Name + " " +
                        DbManager.GetUserById(DbManager.GetSchedule(v.TimeSlotId.Value).NurseId)?.Surname,
                    PaymentStatus = (v.PaymentStatus == true) ? "Opłacona" : "Nieopłacona",
                    v.IsActive,
                    OriginalVisit = v,
                }).ToList<dynamic>();

                _filteredVisits = new List<dynamic>(_allVisits);
                VisitsDataGrid.ItemsSource = _allVisits;
            }
            else
            {
                MessageBox.Show("Błąd podczas ładowania danych.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            Visit selectedVisit = (sender as Button)?.CommandParameter as Visit;

            if (selectedVisit.TestHistories.Any(t => t.Status <= 2))
            {
                if (selectedVisit != null)
                {
                    var editVisitWindow = new EditVisitAdmin(selectedVisit, this);
                    editVisitWindow.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Nie udało się wczytać danych wizyty.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Nie można edytować już odbytej wizyty.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Deactivate_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Visit selectedVisit = (sender as Button)?.CommandParameter as Visit;

            if (selectedVisit != null)
            {
                var result = MessageBox.Show(
                    $"Czy na pewno chcesz anulować wizytę \"{selectedVisit.id}\"?",
                    "Potwierdzenie anulowania",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    bool deleted = DbManager.DeactivateVisit(selectedVisit);

                    if (deleted)
                    {
                        MessageBox.Show("Wizyta została anulowana.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadVisits(); // Odświeżenie tabeli
                    }
                    else
                    {
                        MessageBox.Show("Wystąpił błąd podczas anulowania wizyty.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Nie udało się pobrać wybranej wizyty.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_allVisits == null) return;

            var searchText = txtSearch.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                _filteredVisits = new List<dynamic>(_allVisits);
            }
            else
            {
                var parts = searchText.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                _filteredVisits = _allVisits.Where(vw =>
                {
                    string date = vw.Date != null
                    ? $"{vw.Date:yyyy-MM-dd}|{vw.Date:d}|{vw.Date:dd.MM.yyyy}".ToLower()
                    : "";
                    string time = vw.Time?.ToString(@"HH\:mm") ?? "";
                    string tests = (vw.Tests as string)?.ToLower() ?? "";
                    string patient = (vw.Patient as string)?.ToLower() ?? "";
                    string nurse = (vw.Nurse as string)?.ToLower() ?? "";
                    string payment = (vw.PaymentStatus as string)?.ToLower() ?? "";
                    string isActive = (Convert.ToBoolean(vw.IsActive)) ? "aktywna" : "nieaktywna";

                    return parts.All(p =>
                        date.Contains(p) ||
                        time.Contains(p) ||
                        tests.Contains(p) ||
                        patient.Contains(p) ||
                        nurse.Contains(p) ||
                        payment.Contains(p) ||
                        isActive.Contains(p));
                }).ToList();

            }

            VisitsDataGrid.ItemsSource = _filteredVisits;
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

        private void BtnSamplesNurse_Click(object sender, RoutedEventArgs e)
        {
            SamplesNurse samples = new SamplesNurse(_currentUser);
            samples.Show();
            this.Hide();
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            var profile = new Profile(_currentUser, this);
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
