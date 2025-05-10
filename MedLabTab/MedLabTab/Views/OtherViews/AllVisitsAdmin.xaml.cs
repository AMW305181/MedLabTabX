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
        private Window _parentWindow;
        public AllVisitsAdmin(Window parentWindow)
        {
            InitializeComponent();
            LoadVisits(); // Załaduj dane po inicjalizacji okna
            _parentWindow = parentWindow;
        }
        public void LoadVisits()
        {
            var visits = DbManager.GetAllVisits();

            if (visits != null)
            {
                var visitsWithNames = visits.Select(v => new
                {
                    Date = DbManager.GetSchedule(v.TimeSlotId.Value)?.Date,
                    Time = DbManager.GetSchedule(v.TimeSlotId.Value)?.Time,
                    Tests = string.Join(", ", DbManager.GetTestsInVisit(v.id)
                        .Select(th => DbManager.GetTest(th.TestId))
                        .Where(test => test != null && !string.IsNullOrEmpty(test.TestName))
                        .Select(test => test.TestName)),
                    v.Cost,
                    Patient = DbManager.GetUserById(v.PatientId)?.Name + " " + DbManager.GetUserById(v.PatientId)?.Surname,
                    Nurse = DbManager.GetUserById(DbManager.GetSchedule(v.TimeSlotId.Value).NurseId)?.Name + " " +
                        DbManager.GetUserById(DbManager.GetSchedule(v.TimeSlotId.Value).NurseId)?.Surname,
                    PaymentStatus = (v.PaymentStatus == true) ? "Opłacona" : "Nieopłacona",
                    v.IsActive,
                    OriginalVisit = v,
                }).ToList();

                VisitsDataGrid.ItemsSource = visitsWithNames;
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

        private void BtnAllVisits_Click(object sender, RoutedEventArgs e)
        {
            AllVisitsAdmin allVisits = new AllVisitsAdmin(this);
            allVisits.Show();
            this.Hide();
        }
        //na newVisitAdmin
        private void BtnNewVisit_Click(object sender, RoutedEventArgs e)
        {
            NewVisitAdmin newVisit = new NewVisitAdmin(this);
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
            Statistics statistics = new Statistics(this);
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
