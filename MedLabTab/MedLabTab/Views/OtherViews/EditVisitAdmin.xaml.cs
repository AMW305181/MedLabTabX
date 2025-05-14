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

namespace MedLabTab.Views.OtherViews
{
    /// <summary>
    /// Interaction logic for EditVisitAdmin.xaml
    /// </summary>
    public partial class EditVisitAdmin : Window
    {
        DateOnly date;
        private Visit _originalVisit;
        private Window _parentWindow;
        private User _currentUser;
        private float visitCost;
        private int visitTime=15;
        int selectedSchedule = -1;
        public EditVisitAdmin(Visit visitToEdit, User currentUser,AllVisitsAdmin parentWindow)
        {
            InitializeComponent();
            _originalVisit = visitToEdit;
            _parentWindow = parentWindow;
            _currentUser = currentUser;
            LoadData();

            switch (_currentUser.UserType)
            {
                case 4:
                    IsPaidCheckBox.Visibility = Visibility.Collapsed;
                    PatientComboBox.IsEnabled = false;
                    
                    break;
            }
        }
        public EditVisitAdmin(Visit visitToEdit, User currentUser, MyVisits parentWindow)
        {
            InitializeComponent();
            _originalVisit = visitToEdit;
            _parentWindow = parentWindow;
            _currentUser = currentUser;
            LoadData();

            switch (_currentUser.UserType)
            {
                case 4:
                    IsPaidCheckBox.Visibility = Visibility.Collapsed;
                    PatientComboBox.IsEnabled = false;

                    break;
            }
        }

        private void LoadData()
        {
            visitCost = _originalVisit.Cost;
           

            //załadowanie listy pacjentów
            PatientComboBox.Items.Clear();
            var users = DbManager.GetActivePatients();
            string selectedPesel = DbManager.GetUserById(_originalVisit.PatientId)?.PESEL;

            foreach (var user in users)
            {
                var item = new ComboBoxItem
                {
                    Content = user.Name + " " + user.Surname,
                    Tag = user.PESEL
                };

                PatientComboBox.Items.Add(item);

                if (user.PESEL == selectedPesel)
                {
                    PatientComboBox.SelectedItem = item;
                }
            }

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
            
            //załadowanie badań
            TestsListBox.Items.Clear();
            var selectedTests = DbManager.GetTestsInVisit(_originalVisit.id)
                        .Select(th => DbManager.GetTest(th.TestId));
            foreach (var test in selectedTests)
            {
                TestsListBox.Items.Add(new ListBoxItem
                {
                    Content = test.TestName,
                    Tag = test.id
                });
            }

            UpdateValues();

            //załadowanie listy dostepnych terminow
            DateComboBox.Items.Clear();
            var dates = DbManager.GetAvailableSlotsForDate(date); 
            int selectedSchedule = -1;
            if (_originalVisit.TimeSlotId.HasValue)
            {
                var schedule = DbManager.GetSchedule(_originalVisit.TimeSlotId.Value);
                if (schedule != null)
                {
                    selectedSchedule = schedule.id;
                }
            }
            //foreach (var date in dates)
            //{
            //    var item = new ComboBoxItem
            //    {
            //        Content = date.Date + " " + date.Time,
            //        Tag = date.id
            //    };

            //    DateComboBox.Items.Add(item);

            //    if (date.id == selectedSchedule)
            //    {
            //        DateComboBox.SelectedItem = item;
            //    }
            //}
            foreach (var date in dates)
            {
                DateComboBox.Items.Add(new ComboBoxItem
                {
                    Content = date.Date + " " + date.Time,
                    Tag = date.id
                });
            }

            if (DateComboBox.Items.Count > 0)
                DateComboBox.SelectedIndex = 0;

            IsActiveCheckBox.IsChecked = _originalVisit.IsActive;
            IsPaidCheckBox.IsChecked = _originalVisit.PaymentStatus;

            CostTextBlock.Text = $"Koszt: {visitCost} zł";
            TimeTextBlock.Text = $"Czas trwania: {visitTime} min";
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

                var selectedPatient = (ComboBoxItem)PatientComboBox.SelectedItem;
                string patientPESEL = (string)selectedPatient.Tag;

                var selectedTimeSlot = (ComboBoxItem)DateComboBox.SelectedItem;
                bool editedVisit = false;
                if (selectedTimeSlot.Tag is int timeId)
                {
                    var newVisit = new Visit
                    {
                        //id = _originalVisit.id,
                        Cost = visitCost,
                        PaymentStatus = IsPaidCheckBox.IsChecked == true,
                        IsActive = IsActiveCheckBox.IsChecked == true,
                        PatientId = (DbManager.GetUser(patientPESEL)).id,
                        TimeSlotId = timeId,

                    };
                     editedVisit= DbManager.EditVisit(_originalVisit, newVisit);
                }
  
                bool addedAllTests = true;

                if (editedVisit)
                {
                    DbManager.RemoveTestHistory(_originalVisit.id);

                    foreach (ListBoxItem item in TestsListBox.Items)
                    {
                        if (item.Tag is int testId)
                        {
                            var test = DbManager.GetTest(testId);

                            var newTestHistory = new TestHistory
                            {
                                VisitId = _originalVisit.id,
                                TestId = test.id,
                                PatientId = DbManager.GetUser(patientPESEL).id,
                                Status = IsPaidCheckBox.IsChecked == true ? 2 : 1, // jezeli jest zaplacone to do etapu 2 a jak nie to czeka na 1
                                AnalystId = null,
                            };

                            bool added = DbManager.AddTestHistory(newTestHistory);

                            if (!added)
                            {
                                addedAllTests = false;
                            }
                        }
                    }
                }

                if (editedVisit && addedAllTests)
                {
                    MessageBox.Show("Wizyta została zedytowana pomyślnie!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                    _parentWindow?.Show();
                }
                else
                {
                    MessageBox.Show("Wystąpił błąd podczas edycji wizyty.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
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
            if (string.IsNullOrWhiteSpace(PatientComboBox.Text) ||
                !(TestsListBox.HasItems) ||
                string.IsNullOrWhiteSpace(DateComboBox.Text))
            {
                MessageBox.Show("Wszystkie pola muszą być wypełnione.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
           

            return true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            _parentWindow?.Show();
        }

        private void VisitCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            // Sprawdź czy jakaś data została wybrana
            if (VisitCalendar.SelectedDate.HasValue)
            {
                DateTime selectedDate = VisitCalendar.SelectedDate.Value;

                DateOnly date = DateOnly.FromDateTime(selectedDate);
                UpdateAvailableTimeSlots(date);
            }
            else
            {
                DateComboBox.Items.Clear();
            }
        }
        private void UpdateAvailableTimeSlots(DateOnly date)
        {
            DateComboBox.Items.Clear();
            List<Schedule> availableTimeSlots = DbManager.GetAvailableSlotsForDate(date);

            if (availableTimeSlots == null || availableTimeSlots.Count == 0)
            {
                // Wyświetl MessageBox z informacją o braku terminów
                MessageBox.Show("Brak terminów tego dnia", "Informacja",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            foreach (var timeSlot in availableTimeSlots)
            {
                string timeString = timeSlot.Time.ToString("HH:mm");
                string displayText = timeString;
                ComboBoxItem item = new ComboBoxItem
                {
                    Content = displayText,
                    Tag = timeSlot.id,
                };
                DateComboBox.Items.Add(item);
            }

            if (DateComboBox.Items.Count > 0)
            {
                DateComboBox.SelectedIndex = 0;
            }
        }
    }
}