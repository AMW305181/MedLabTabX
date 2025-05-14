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

namespace MedLabTab.Views.OtherViews
{
    public partial class AllTests : Window
    {
        private Window _parentWindow;
        private User _currentUser;
        private List<dynamic> _allTests;
        private List<dynamic> _filteredTests;
        public AllTests(User currentUser, Window parentWindow)
        {
            InitializeComponent();
            
            _parentWindow = parentWindow;
            _currentUser = currentUser;
            LoadTests(); // Załaduj dane po inicjalizacji okna
            txtSearch.TextChanged += txtSearch_TextChanged;

            switch (_currentUser.UserType)
            {
                case 3:
                    AnalystMenu.Visibility = Visibility.Visible;
                    break;
                case 4:
                    PatientMenu.Visibility = Visibility.Visible;
                    ScheduleButton.Visibility = Visibility.Visible;
                    break;
            }
        }
        public void LoadTests()
        {
            var tests = DbManager.GetActiveTests();
            var categoryDict = DbManager.GetCategoriesDictionary();

            if (tests != null && categoryDict != null)
            {
                _allTests = tests.Select(t => new
                {
                    t.TestName,
                    t.Description,
                    Price = t.Price.ToString("0.00"),
                    Category = categoryDict.TryGetValue(t.Category, out var catName) ? catName : "Nieznana",
                    OriginalTest = t
                }).ToList<dynamic>();

                _filteredTests = new List<dynamic>(_allTests);
                BadaniaDataGrid.ItemsSource = _allTests;
            }
            else
            {
                MessageBox.Show("Błąd podczas ładowania danych.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_allTests == null || _filteredTests == null) return;

            var searchText = txtSearch.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                BadaniaDataGrid.ItemsSource = _filteredTests;
                return;
            }

            BadaniaDataGrid.ItemsSource = _filteredTests.Where(t =>
            {
                string testName = t.TestName?.ToString().ToLower() ?? string.Empty;
                string description = t.Description?.ToString().ToLower() ?? string.Empty;
                string category = t.Category?.ToString().ToLower() ?? string.Empty;

                return testName.Contains(searchText) ||
                       description.Contains(searchText) ||
                       category.Contains(searchText);
            }).ToList();
        }

        private void BtnExams_Click(object sender, RoutedEventArgs e)
        {
            AllTests allTests = new AllTests(_currentUser, this);
            allTests.Show();
            this.Hide();
        }
        private void BtnResults_Click(object sender, RoutedEventArgs e)
        {
            AllReports reports = new AllReports(_currentUser, this);
            reports.Show();
            this.Hide();
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            var profile = new Profile(_currentUser, this);
            profile.Show();
            this.Hide();
        }

        private void BtnSamples_Click(object sender, RoutedEventArgs e)
        {
            SamplesAnalyst samples = new SamplesAnalyst(_currentUser);
            samples.Show();
            this.Close();
        }

        private void BtnReport_Click(object sender, RoutedEventArgs e)
        {
            AllReports newReport = new AllReports(_currentUser, this);
            newReport.Show();
            this.Hide();
        }

        private void BtnVisits_Click(object sender, RoutedEventArgs e)
        {
            MyVisits allVisits = new MyVisits(_currentUser, this);
            allVisits.Show();
            this.Hide();
        }

        private void BtnNewVisit_Click(object sender, RoutedEventArgs e)
        {
            NewVisit newVisit = new NewVisit(_currentUser,this);
            newVisit.Show();
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
