using MedLabTab.DatabaseManager;
using MedLabTab.DatabaseModels;
using System;
using System.Collections.Generic;
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
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace MedLabTab.Views.OtherViews
{
    /// <summary>
    /// Interaction logic for NewVisit.xaml
    /// </summary>
    public partial class NewVisitAdmin : Window
    {
        private Window _parentWindow;
        private float visitCost;
        private int visitTime;
        public NewVisitAdmin(Window parentWindow)
        {
            InitializeComponent();
            ClearForm();
            LoadData();
            _parentWindow = parentWindow;
        }

        private void LoadData()
        {
            visitCost = 0;
            visitTime = 15; // zakładamy że każda wizyta trwa 15 minut

            //załadowanie listy pacjentów
            PatientComboBox.Items.Clear();
            var users = DbManager.GetActivePatients();
            foreach (var user in users)
            {
                PatientComboBox.Items.Add(new ComboBoxItem
                {
                    Content = user.Name + " " + user.Surname,
                    Tag = user.PESEL
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

            VisitCalendar.SelectedDate = DateTime.Today;
        }

        private void ClearForm()
        {
            CostTextBlock.Text = $"Koszt: {visitCost} zł";
            TimeTextBlock.Text = $"Czas trwania: {visitTime} min";
            IsPaidCheckBox.IsChecked = false;
            IsActiveCheckBox.IsChecked = true;
        }

        private void UpdateValues()
        {
            CostTextBlock.Text = $"Koszt: {visitCost} zł";
            TimeTextBlock.Text = $"Czas trwania: {visitTime} min";
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInputs())
            {
                //do uzupełnienia po dodaniu harmonogramu

                //var selectedPatient = (ComboBoxItem)PatientComboBox.SelectedItem;
                //string patientPESEL = (string)selectedPatient.Tag;

                //var selectedTimeSlot = (ComboBoxItem)DateComboBox.SelectedItem;
                //int timeSlotId = (int)selectedTimeSlot.Tag;

                //var newVisit = new Visit
                //{
                //    Cost = visitCost,
                //    PaymentStatus = IsPaidCheckBox.IsChecked == true,
                //    IsActive = IsActiveCheckBox.IsChecked == true,
                //    PatientId = (DbManager.GetUser(patientPESEL)).id,
                //    TimeSlotId = timeSlotId,
                //};

                //bool addedVisit = DbManager.AddVisit(newVisit);
                //bool addedAllTests = true;


                //foreach (ListBoxItem item in TestsListBox.Items)
                //{
                //    if (item.Tag is int testId)
                //    {
                //        var test = DbManager.GetTest(testId);

                //        var newTestHistory = new TestHistory
                //        {
                //            VisitId = newVisit.id,
                //            TestId = test.id,
                //            PatientId = DbManager.GetUser(patientPESEL).id,
                //            Status = IsPaidCheckBox.IsChecked == true ? 2 : 1, // jezeli jest zaplacone to do etapu 2 a jak nie to czeka na 1
                //            AnalystId = null,
                //        };

                //        bool added = DbManager.AddTestHistory(newTestHistory);

                //        if (!added)
                //        {
                //            addedAllTests = false;
                //        }
                //    }
                //}

                //if (addedVisit && addedAllTests)
                //{
                //    MessageBox.Show("Wizyta została dodana pomyślnie!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                //    this.Close();
                //    _parentWindow?.Show();
                //}
                //else
                //{
                //    MessageBox.Show("Wystąpił błąd podczas dodawania wizyty.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                //}
            }
        }

        private void TestsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TestsComboBox.SelectedIndex != 0)
            {
                var selectedTest = (ComboBoxItem)TestsComboBox.SelectedItem;
                int testId = (int)selectedTest.Tag;

                TestsListBox.Items.Add(new ListBoxItem
                {
                    Content = selectedTest.Content,
                    Tag = selectedTest.Tag
                });

                Test test = DbManager.GetTest(testId);

                visitCost += test.Price;

                UpdateValues();

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

        private void PatientComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TestsComboBox.SelectedIndex != 0)
            {

            }
        }

        private void RemoveSelectedTest_Click(object sender, RoutedEventArgs e)
        {
            if (TestsListBox.SelectedItem is ListBoxItem selectedItem)
            {
                if (selectedItem.Tag is int testId)
                {
                    var test = DbManager.GetTest(testId);

                    visitCost -= test.Price;

                    TestsListBox.Items.Remove(selectedItem);
                    UpdateValues();
                }
            }
            else
            {
                MessageBox.Show("Zaznacz badanie do usunięcia.", "Brak zaznaczenia", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private bool ValidateInputs()
        {
            if (!(TestsListBox.HasItems) || !VisitCalendar.SelectedDate.HasValue)
            {
                MessageBox.Show("Wszystkie pola muszą być wypełnione.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void BtnAllVisits_Click(object sender, RoutedEventArgs e)
        {
            AllVisitsAdmin allVisits = new AllVisitsAdmin(this);
            allVisits.Show();
            this.Hide();
        }

        private void BtnNewVisit_Click(object sender, RoutedEventArgs e)
        {
            NewVisitAdmin newVisitAdmin = new NewVisitAdmin(this);
            newVisitAdmin.Show();
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
            Statistics statistics = new Statistics(this);
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

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            _parentWindow?.Show();
        }
    }
}
