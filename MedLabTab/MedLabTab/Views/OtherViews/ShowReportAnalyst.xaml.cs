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
    /// Interaction logic for Report.xaml
    /// </summary>
    public partial class ShowReportAnalyst : Window
    {
        private Window _parentWindow;
        private User _currentUser;
        private readonly TestHistory _testHistory;
        public ShowReportAnalyst(TestHistory testHistory, User currentUser, Window parentWindow)
        {
            InitializeComponent();
            _parentWindow = parentWindow;
            _currentUser = currentUser;
            _testHistory = testHistory;
            FillReportWithData();

            switch (_currentUser.UserType)
            {
                case 1:
                    ReceptionMenu.Visibility = Visibility.Visible;
                    break;
                case 3:
                    AnalystMenu.Visibility = Visibility.Visible;
                    break;
            }
        }
        private void FillReportWithData()
        {
            TestTextBlock.Text = _testHistory.Test?.TestName;
            PatientTextBlock.Text = _testHistory.Patient?.Name + " " + _testHistory.Patient?.Surname;
            NurseTextBlock.Text = _testHistory.Visit?.TimeSlot?.Nurse?.Name + " " + _testHistory.Visit?.TimeSlot?.Nurse?.Surname;
            AnalystTextBlock.Text = _testHistory.Analyst != null ? $"{_testHistory.Analyst.Name} {_testHistory.Analyst.Surname}" : "Brak analityka";
            DateTextBlock.Text = $"{_testHistory.Visit?.TimeSlot?.Date.ToString("dd.MM.yyyy") ?? "Brak daty"} {_testHistory.Visit?.TimeSlot?.Time.ToString(@"hh\:mm") ?? "Brak godziny"}";
            ResultTextBox.Text = DbManager.GetReport(_testHistory.id).Results;
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInputs())
            {
                string newResults = ResultTextBox.Text.Trim();
                var oldReport = DbManager.GetReport(_testHistory.id);
                var now = DateTime.Now;

                var newReport = new Report
                {
                    id = oldReport.id,
                    SampleId = oldReport.SampleId,
                    LastUpdateDate = DateOnly.FromDateTime(now),
                    LastUpdateTime = TimeOnly.FromDateTime(now),
                    Results = newResults,
                };

                bool updated = DbManager.EditReport(oldReport, newReport);

                if (updated)
                {
                    MessageBox.Show("Raport został zaktualizowany!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                    _parentWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Wystąpił błąd podczas aktualizacji.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(ResultTextBox.Text))

            {
                MessageBox.Show("Pole z wynikami musi być wypełnione.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _parentWindow.Show();
            this.Close();
        }
        private void BtnAllVisits_Click(object sender, RoutedEventArgs e)
        {
            AllVisitsAdmin allVisits = new AllVisitsAdmin(_currentUser);
            allVisits.Show();
            this.Hide();
        }

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

        private void BtnSamplesAnalyst_Click(object sender, RoutedEventArgs e)
        {
            SamplesAnalyst samples = new SamplesAnalyst(_currentUser);
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
