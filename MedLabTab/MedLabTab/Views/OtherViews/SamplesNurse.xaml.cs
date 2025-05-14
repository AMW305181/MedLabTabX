using MedLabTab.DatabaseManager;
using MedLabTab.DatabaseModels;
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
    public partial class SamplesNurse : Window
    {
        private User _currentUser;
        public SamplesNurse(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            LoadSamples();
        }
        public void LoadSamples()
        {
            var visits = DbManager.GetNurseVisits(_currentUser.id);

            if (visits != null)
            {
                var samples = visits.Select(v => new
                {
                    Date = DbManager.GetSchedule(v.TimeSlotId.Value)?.Date.ToString("dd.MM.yyyy"),
                    Time = DbManager.GetSchedule(v.TimeSlotId.Value)?.Time.ToString(@"hh\:mm"),
                    Tests = string.Join(", ", DbManager.GetTestsInVisit(v.id)
                        .Select(th => DbManager.GetTest(th.TestId))
                        .Where(test => test != null && !string.IsNullOrEmpty(test.TestName))
                        .Select(test => test.TestName)),
                    Patient = DbManager.GetUserById(v.PatientId)?.Name + " " + DbManager.GetUserById(v.PatientId)?.Surname,
                    OriginalVisit = v,
                }).ToList();

                SamplesDataGrid.ItemsSource = samples;
            }
            else
            {
                MessageBox.Show("Błąd podczas ładowania danych.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void Ready_Click(object sender, RoutedEventArgs e)
        {

            var button = sender as Button;
            Visit visit = button?.CommandParameter as Visit;

            List<TestHistory> oldTests = DbManager.GetTestsInVisit(visit.id); // zakładamy że to lista TestHistory
            bool allUpdated = true;

            foreach (TestHistory oldTest in oldTests)
            {
                var newTest = new TestHistory
                {
                    id = oldTest.id, // klucz główny musi być ten sam, żeby zaktualizować
                    VisitId = oldTest.VisitId,
                    TestId = oldTest.TestId,
                    PatientId = oldTest.PatientId,
                    AnalystId = oldTest.AnalystId,
                    Status = 3 // moze leciec do analityka
                };

                bool updated = DbManager.EditTestHistory(oldTest, newTest);

                if (!updated)
                {
                    allUpdated = false;
                }
            }

            DbManager.DeactivateVisit(visit); //probki pobrane - wizyta sie odbyla

            if (allUpdated)
            {
                MessageBox.Show("Statusy badań zostały zaktualizowane.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Nie wszystkie badania udało się zaktualizować.", "Uwaga", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            LoadSamples(); // odśwież widok
        }

        private void BtnExams_Click(object sender, RoutedEventArgs e)
        {
            AllVisitsAdmin allVisits = new AllVisitsAdmin(_currentUser);
            allVisits.Show();
            this.Hide();
        }

        private void BtnSamples_Click(object sender, RoutedEventArgs e)
        {
            SamplesNurse samples = new SamplesNurse(_currentUser);
            samples.Show();
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
