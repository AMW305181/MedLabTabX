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
    public partial class AllTestsAdmin : Window
    {
        private Window _parentWindow;
        public AllTestsAdmin(Window parentWindow)
        {
            InitializeComponent();
            LoadTests();
            _parentWindow = parentWindow;
        }

        public void LoadTests()
        {
            var tests = DbManager.GetAllTests();
            var categoryDict = DbManager.GetCategoriesDictionary();

            if (tests != null && categoryDict != null)
            {
                var mappedTests = tests.Select(t => new
                {
                    t.TestName,
                    t.Description,
                    t.Price,
                    Category = categoryDict.TryGetValue(t.Category, out var catName) ? catName : "Nieznana",
                    IsActive = t.IsActive,
                    OriginalTest = t,
                    StatusText = t.IsActive ? "Dezaktywuj" : "Aktywuj",
                    StatusColor = t.IsActive ?
                new SolidColorBrush(Color.FromRgb(255, 85, 85)) :
                new SolidColorBrush(Color.FromRgb(76, 175, 80))
                }).ToList();

                BadaniaDataGrid.ItemsSource = mappedTests;
            }
            else
            {
                MessageBox.Show("Błąd podczas ładowania danych.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            _parentWindow?.Show();
        }

        private void BtnAllVisits_Click(object sender, RoutedEventArgs e)
        {
            AllVisitsAdmin allVisits = new AllVisitsAdmin(this);
            allVisits.Show();
            this.Hide();
        }

        private void BtnNewVisit_Click(object sender, RoutedEventArgs e)
        {
            NewVisit newVisit = new NewVisit(this);
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
            Statistics statistics = new Statistics();
            statistics.Show();
            this.Close();
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
