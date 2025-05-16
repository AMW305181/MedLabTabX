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
    public partial class AllTestsAdmin : Window
    {
        private Window _parentWindow;
        private SignedInUser _currentUser;
        private List<dynamic> _allTests;
        private List<dynamic> _filteredTests;
        public AllTestsAdmin(SignedInUser currentUser, Window parentWindow)
        {
            InitializeComponent();
            LoadTests();
            txtSearch.TextChanged += txtSearch_TextChanged;
            _parentWindow = parentWindow;
            _currentUser = currentUser;
        }
        public void LoadTests()
        {
            var tests = DbManager.GetAllTests();
            var categoryDict = DbManager.GetCategoriesDictionary();

            if (tests != null && categoryDict != null)
            {
                _allTests = tests.Select(t => new
                {
                    t.TestName,
                    t.Description,
                    Price = t.Price.ToString("0.00"),
                    Category = categoryDict.TryGetValue(t.Category, out var catName) ? catName : "Nieznana",
                    IsActive = t.IsActive,
                    OriginalTest = t,
                    StatusText = t.IsActive ? "Dezaktywuj" : "Aktywuj",
                    StatusColor = t.IsActive ?
                new SolidColorBrush(Color.FromRgb(205, 92, 92)) :
                new SolidColorBrush(Color.FromRgb(76, 175, 80))
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
                // Get property values safely with null checks
                string testName = t.TestName?.ToString().ToLower() ?? string.Empty;
                string description = t.Description?.ToString().ToLower() ?? string.Empty;
                string category = t.Category?.ToString().ToLower() ?? string.Empty;
                string price = t.Price?.ToString().ToLower() ?? string.Empty;
                string isActive = t.IsActive?.ToString().ToLower() ?? string.Empty;

                // Check if any of the properties contain the filter text
                return testName.Contains(searchText) ||
                       description.Contains(searchText) ||
                       category.Contains(searchText) ||
                       isActive.Contains(searchText) || price.Contains(searchText);
            }).ToList();
        }
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            Test selectedTest = (sender as Button)?.CommandParameter as Test;

            if (selectedTest != null)
            {
                var editTestWindow = new EditTest(selectedTest, this);
                editTestWindow.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Nie udało się wczytać danych badania.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ToggleActive_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Test selectedTest = (sender as Button)?.CommandParameter as Test;

            if (selectedTest != null)
            {
                if (selectedTest.IsActive == false)
                {
                    var result = MessageBox.Show(
                                       $"Czy na pewno chcesz przywrócić badanie \"{selectedTest.TestName}\"?",
                                       "Potwierdzenie przywrócenia",
                                       MessageBoxButton.YesNo,
                                       MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        bool deleted = DbManager.ChangeTestStatus(selectedTest);

                        if (deleted)
                        {
                            MessageBox.Show("Badanie zostało przywrócone.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadTests();
                        }
                        else
                        {
                            MessageBox.Show("Wystąpił błąd podczas przywracania badania.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    var result2 = MessageBox.Show(
                    $"Czy na pewno chcesz usunąć badanie \"{selectedTest.TestName}\"?",
                    "Potwierdzenie usunięcia",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                    if (result2 == MessageBoxResult.Yes)
                    {
                        bool deleted = DbManager.ChangeTestStatus(selectedTest);

                        if (deleted)
                        {
                            MessageBox.Show("Badanie zostało usunięte.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadTests(); // Odświeżenie tabeli
                        }
                        else
                        {
                            MessageBox.Show("Wystąpił błąd podczas usuwania badania.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
               
            }
            else
            {
                MessageBox.Show("Nie udało się pobrać wybranego badania.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
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
