using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
using MedLabTab.ViewModels;
using MedLabTab.Views.OtherViews;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MedLabTab.Views.MainViews
{
    /// <summary>
    /// Interaction logic for MainViewPatient.xaml
    /// </summary>
    public partial class MainViewPatient : Window
    {
        private SignedInUser currentUser;
        private readonly MedLabContext _context;
        private ObservableCollection<Visit> UpcomingVisits { get; set; }

        public MainViewPatient(SignedInUser user)
        {
            InitializeComponent();
            //LoadVisits(); // Załaduj dane po inicjalizacji okna

            // Ensure user is not null before proceeding
            if (user == null)
            {
                MessageBox.Show("Błąd: Nie można załadować danych użytkownika.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            currentUser = user;

            try
            {
                _context = new MedLabContext();
                UpcomingVisits = new ObservableCollection<Visit>();
                DataContext = this;

                LoadVisits();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas inicjalizacji: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        public void LoadVisits()
        {
            var visits = DbManager.GetMyVisits(currentUser.id); 

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

        private void BtnExams_Click(object sender, RoutedEventArgs e)
        {
            AllTests allTests = new AllTests(currentUser, this);
            allTests.Show();
            this.Hide();
        }

        private void BtnVisits_Click(object sender, RoutedEventArgs e)
        {
            MyVisits allVisits = new MyVisits(currentUser, this);
            allVisits.Show();
            this.Hide();
        }

        private void BtnNewVisit_Click(object sender, RoutedEventArgs e)
        {
            NewVisit newVisit = new NewVisit(currentUser, this);
            newVisit.Show();
            this.Hide();
        }

        private void BtnResults_Click(object sender, RoutedEventArgs e)
        {

            AllReports allReports = new AllReports(currentUser, this);
            allReports.Show();
            this.Hide();
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            var profile = new Profile(currentUser, this);
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