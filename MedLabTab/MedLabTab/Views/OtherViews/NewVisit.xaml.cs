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
using static System.Net.Mime.MediaTypeNames;

namespace MedLabTab.Views.OtherViews
{
    /// <summary>
    /// Interaction logic for NewVisit.xaml
    /// </summary>
    public partial class NewVisit : Window
    {
        private Window _parentWindow;
        public NewVisit(Window parentWindow)
        {
            InitializeComponent();
            ClearForm();
            LoadData();
            _parentWindow = parentWindow;
        }

        private void LoadData()
        {
            //załadowanie listy pacjentów
            PatientComboBox.Items.Clear();
            var users = DbManager.GetActivePatients();
            foreach (var user in users)
            {
                PatientComboBox.Items.Add(new ComboBoxItem
                {
                    Content = user.Name + " " + user.Surname,
                    Tag = user.id
                });
            }

            if (PatientComboBox.Items.Count > 0)
                PatientComboBox.SelectedIndex = 0;

            //załadowanie listy badań
            TestsComboBox.Items.Clear();
            TestsComboBox.Items.Add(new ComboBoxItem
            {
                Content = "",
            });
            var tests = DbManager.GetActiveTests();
            foreach (var test in tests)
            {
                TestsComboBox.Items.Add(new ComboBoxItem
                {
                    Content = test.TestName,
                    Tag = test.id
                });
            }

            if (TestsComboBox.Items.Count > 0)
                TestsComboBox.SelectedIndex = 0;
        }

        private void ClearForm()
        {
            CostTextBlock.Text = $"Koszt: 0 zł";
            TimeTextBlock.Text = $"Czas trwania: 0 min";
            IsActiveCheckBox.IsChecked = false;
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DataComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void PatientComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TestsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TestsComboBox.SelectedIndex != 0)
            {
                var selectedTest = (ComboBoxItem)TestsComboBox.SelectedItem;
                int testId = (int)selectedTest.Tag;


                //TestsListBox.Add(selectedTest);
                TestsListBox.Items.Add(new ListBoxItem
                {
                    Content = selectedTest.Content,
                    Tag = selectedTest.Tag
                });

                TestsComboBox.SelectedIndex = 0;
            }
        }

        private void VisitCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VisitCalendar.SelectedDate.HasValue)
            {
                DateTime selectedDate = VisitCalendar.SelectedDate.Value;
                // TimeComboBox.ItemsSource = GetAvailableTimes(selectedDate);
            }
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

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            _parentWindow?.Show();
        }
    }
}
