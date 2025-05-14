using MedLabTab.DatabaseManager;
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
using MedLabTab.DatabaseManager;
using MedLabTab.DatabaseModels;
using MedLabTab.ViewModels;

namespace MedLabTab.Views.OtherViews
{
    /// <summary>
    /// Interaction logic for Statistics.xaml
    /// </summary>
    public partial class Statistics : Window
    {
        private SignedInUser _currentUser;
        public Statistics(SignedInUser currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            FillData();
        }

        private void FillData()
        {
            var tests = DbManager.GetCompletedTests();

            if (tests == null || !tests.Any())
            {
                Income.Text = "";
                Test.Text = "";
                Nurse.Text = "";
                Analyst.Text = "";
                Patient.Text = "";
                return;
            }

            // Income
            float income = tests.Sum(t => DbManager.GetTest(t.TestId)?.Price ?? 0);
            Income.Text = income.ToString();
            Income.Text = string.Format("{0:N2} zł", income);

            // Most common test
            var mostCommonTest = tests
                .GroupBy(t => t.TestId)
                .Select(g => new { TestId = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .FirstOrDefault();

            var testInfo = mostCommonTest != null ? DbManager.GetTest(mostCommonTest.TestId) : null;
            Test.Text = testInfo?.TestName ?? "Brak danych";

            // Most common nurse
            var mostCommonNurse = tests
                .GroupBy(t => DbManager.GetNurseIdFromTestHistory(t))
                .Select(g => new { NurseId = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .FirstOrDefault();

            var nurseInfo = mostCommonNurse != null ? DbManager.GetUserById(mostCommonNurse.NurseId) : null;
            Nurse.Text = nurseInfo != null ? $"{nurseInfo.Name} {nurseInfo.Surname}" : "Brak danych";

            // Most common analyst
            var mostCommonAnalyst = tests
                .GroupBy(t => t.AnalystId)
                .Select(g => new { AnalystId = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .FirstOrDefault();

            var analystInfo = mostCommonAnalyst?.AnalystId != null ? DbManager.GetUserById(mostCommonAnalyst.AnalystId.Value) : null;
            Analyst.Text = analystInfo != null ? $"{analystInfo.Name} {analystInfo.Surname}" : "Brak danych";

            // Most common patient
            var mostCommonPatient = tests
                .GroupBy(t => t.PatientId)
                .Select(g => new { PatientId = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .FirstOrDefault();

            var patientInfo = mostCommonPatient != null ? DbManager.GetUserById(mostCommonPatient.PatientId) : null;
            Patient.Text = patientInfo != null ? $"{patientInfo.Name} {patientInfo.Surname}" : "Brak danych";
        }

        private void BtnAllVisits_Click(object sender, RoutedEventArgs e)
        {
            AllVisitsAdmin allVisits = new AllVisitsAdmin(_currentUser);
            allVisits.Show();
            this.Hide();
        }

        private void BtnNewVisit_Click(object sender, RoutedEventArgs e)
        {
            NewVisitAdmin newVisit = new NewVisitAdmin(_currentUser,this);
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
