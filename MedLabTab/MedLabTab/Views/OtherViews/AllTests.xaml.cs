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
        public AllTests(User currentUser, Window parentWindow)
        {
            InitializeComponent();
            LoadTests(); // Załaduj dane po inicjalizacji okna
            _parentWindow = parentWindow;
            _currentUser = currentUser;
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
                var mappedTests = tests.Select(t => new
                {
                    t.TestName,
                    t.Description,
                    t.DisplayPrice,
                    Category = categoryDict.TryGetValue(t.Category, out var catName) ? catName : "Nieznana",
                    OriginalTest = t
                }).ToList();

                BadaniaDataGrid.ItemsSource = mappedTests;
            }
            else
            {
                MessageBox.Show("Błąd podczas ładowania danych.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void BtnExams_Click(object sender, RoutedEventArgs e)
        {
            AllTests allTests = new AllTests(_currentUser, this);
            allTests.Show();
            this.Hide();
        }
        private void BtnResults_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            var profile = new Profile(_currentUser, this);
            profile.Show();
            this.Hide();
        }

        private void BtnSamples_Click(object sender, RoutedEventArgs e)
        {
            Samples samples = new Samples();
            samples.Show();
            this.Close();
        }

        private void BtnReport_Click(object sender, RoutedEventArgs e)
        {
            AllReports newReport = new AllReports(this);
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
