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
    public partial class ShowReport : Window
    {
        private Window _parentWindow;
        private User _currentUser;
        private readonly TestHistory _testHistory;
        public ShowReport(TestHistory testHistory, User currentUser, Window parentWindow)
        {
            InitializeComponent();
            _parentWindow = parentWindow;
            _currentUser = currentUser;
            _testHistory = testHistory;
            FillReportWithData();
        }
        private void FillReportWithData()
        {
            TestTextBlock.Text = _testHistory.Test?.TestName;
            PatientTextBlock.Text = _testHistory.Patient?.Name + " " + _testHistory.Patient?.Surname;
            NurseTextBlock.Text = _testHistory.Visit?.TimeSlot?.Nurse?.Name + " " + _testHistory.Visit?.TimeSlot?.Nurse?.Surname;
            AnalystTextBlock.Text = _testHistory.Analyst != null ? $"{_testHistory.Analyst.Name} {_testHistory.Analyst.Surname}" : "Brak analityka";
            DateTextBlock.Text = $"{_testHistory.Visit?.TimeSlot?.Date.ToString("dd.MM.yyyy") ?? "Brak daty"} {_testHistory.Visit?.TimeSlot?.Time.ToString(@"hh\:mm") ?? "Brak godziny"}";
          //  ResultTextBox.Text = DbManager.GetReport(_testHistory.id).Results;
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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            _parentWindow?.Show();
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
