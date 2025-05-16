using MedLabTab.DatabaseManager;
using MedLabTab.DatabaseModels;
using MedLabTab.ViewModels;
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
        private SignedInUser _currentUser;
        private float visitCost;
        private int visitTime;
        private int? _selectedSlotId;
        private User _selectedUser; //selected user (nie ten zalogowany)
        private List<Schedule> _AvaibleSlots;
        private bool _isInitialLoad = true;

        public NewVisitAdmin(SignedInUser currentUser, Window parentWindow)
        {
            InitializeComponent();
            ClearForm();
            LoadData();
            _parentWindow = parentWindow;
            _currentUser = currentUser;
        }

        private void LoadData()
        {
            visitCost = 0;
            visitTime = 0; // zakładamy że każda wizyta trwa 15 minut

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
                // First, refresh available slots to ensure the selected slot is still available
                if (VisitCalendar.SelectedDate.HasValue)
                {
                    DateOnly selectedDate = DateOnly.FromDateTime(VisitCalendar.SelectedDate.Value);
                    // Store previously selected slot ID before refreshing
                    int? previouslySelectedSlotId = _selectedSlotId;

                    // Reload available slots
                    _AvaibleSlots = DbManager.GetAvailableSlotsForDate(selectedDate);

                    // Check if the previously selected slot is still available
                    bool slotStillAvailable = false;
                    if (_AvaibleSlots != null && _AvaibleSlots.Count > 0)
                    {
                        slotStillAvailable = _AvaibleSlots.Any(slot => slot.id == previouslySelectedSlotId);
                    }

                    if (!slotStillAvailable)
                    {
                        MessageBox.Show("Ten termin nie jest już dostępny. Proszę o wybranie innego.",
                                        "Informacja", MessageBoxButton.OK, MessageBoxImage.Warning);

                        // Refresh the UI to show updated slots
                        RefreshTimeSlots();
                        return; // Exit the method without proceeding to registration
                    }
                }

                try
                {
                    using (var db = new MedLabContext())
                    {
                        using (var transaction = db.Database.BeginTransaction())
                        {
                            try
                            {
                                VisitsManager visitsManager = new VisitsManager();
                                float testCost = visitCost;
                                bool testPaymentStatus = IsPaidCheckBox.IsChecked ?? false;
                                bool testIsActive = IsActiveCheckBox.IsChecked ?? false;
                                int testPatientId = _selectedUser.id;
                                int? testTimeSlotId = _selectedSlotId;

                                Visit newVisit = visitsManager.CreateVisit(db, testCost, testPaymentStatus,
                                                       testIsActive, testPatientId, testTimeSlotId);
                                if (newVisit != null && newVisit.id > 0)
                                {
                                    int visitId = newVisit.id;
                                    // Add TestHistory records for each selected test
                                    foreach (ListBoxItem item in TestsListBox.Items)
                                    {
                                        int testId = (int)item.Tag;
                                        TestHistory testHistory = new TestHistory
                                        {
                                            VisitId = visitId,
                                            TestId = testId,
                                            PatientId = testPatientId,
                                            Status = 1, // Default status
                                            AnalystId = null // Default null
                                        };
                                        db.TestHistories.Add(testHistory);
                                    }
                                    db.SaveChanges();
                                    transaction.Commit();
                                    MessageBox.Show("Wizyta zarejestrowana!", "Sukces",
                                                   MessageBoxButton.OK, MessageBoxImage.Information);

                                    // Refresh the time slots after successful registration
                                    RefreshTimeSlots();
                                }
                                else
                                {
                                    MessageBox.Show("Nie udało się zarejestrować wizyty", "Błąd",
                                                   MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                            catch (Exception ex)
                            {
                                // Rollback transaction if anything fails
                                transaction.Rollback();
                                throw; // Re-throw for outer exception handling
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd: {ex.Message}", "Błąd",
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Add this helper method to refresh the time slots
        private void RefreshTimeSlots()
        {
            // Setting _isInitialLoad to true to prevent showing message box during refresh
            bool originalInitialLoadState = _isInitialLoad;
            _isInitialLoad = true;

            // Clear and repopulate the time combo box
            TimeComboBox.Items.Clear();
            _selectedSlotId = null;

            if (VisitCalendar.SelectedDate.HasValue)
            {
                DateOnly selectedDate = DateOnly.FromDateTime(VisitCalendar.SelectedDate.Value);
                _AvaibleSlots = DbManager.GetAvailableSlotsForDate(selectedDate);

                if (_AvaibleSlots != null && _AvaibleSlots.Count > 0)
                {
                    foreach (var slot in _AvaibleSlots)
                    {
                        string nurseInfo = $"{slot.Nurse.Name} {slot.Nurse.Surname}";
                        string timeInfo = slot.Time.ToString("HH:mm");
                        ListBoxItem item = new ListBoxItem
                        {
                            Content = $"{timeInfo} - {nurseInfo}",
                            Tag = slot.id
                        };
                        TimeComboBox.Items.Add(item);
                    }
                }
            }

            // Restore original initial load state
            _isInitialLoad = originalInitialLoadState;
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
                //visitTime += 15; // zakładamy że każde badanie trwa 15 minut
                visitTime = 15; // łączny czas wizyty to 15 minut (założeneie)

                UpdateValues();

                TestsComboBox.SelectedIndex = 0;
            }
        }

        private void VisitCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            // Czyścimy combobox z poprzednich wartościi
            TimeComboBox.Items.Clear();
            _selectedSlotId = null;
            if (VisitCalendar.SelectedDate.HasValue)
            {
                DateOnly selectedDate = DateOnly.FromDateTime(VisitCalendar.SelectedDate.Value);
                _AvaibleSlots = DbManager.GetAvailableSlotsForDate(selectedDate);

                if (_AvaibleSlots != null && _AvaibleSlots.Count > 0)
                {
                    foreach (var slot in _AvaibleSlots)
                    {
                        string timeInfo = slot.Time.ToString("HH:mm");
                        ListBoxItem item = new ListBoxItem
                        {
                            Content = $"{timeInfo}",

                            Tag = slot.id
                        };
                        TimeComboBox.Items.Add(item);
                    }
                }
                else if (!_isInitialLoad) // Only show message box if it's not the initial load
                {
                    MessageBox.Show("Brak dostępnych terminów na wybrany dzień.",
                        "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            // After first execution, set flag to false
            _isInitialLoad = false;
        }

        private void PatientComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PatientComboBox.SelectedItem != null)
            {
                ComboBoxItem selectedItem = PatientComboBox.SelectedItem as ComboBoxItem;
                if (selectedItem != null)
                {
                    string pesel = selectedItem.Tag as string;
                    if (!string.IsNullOrEmpty(pesel))
                    {
                        User user = DbManager.GetUser(pesel);
                        if (user != null)
                        {
                            _selectedUser = user; 
                        }
                    }
                }
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
                    if (TestsListBox.Items.Count == 1)
                    { //łączny czas wizyty to 15 minut (założeneie)
                        visitTime = 0;
                    }

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
            if (!TestsListBox.HasItems)
            {
                MessageBox.Show("Musisz wybrać co najmniej jedno badanie.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!VisitCalendar.SelectedDate.HasValue)
            {
                MessageBox.Show("Musisz wybrać datę wizyty.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!_selectedSlotId.HasValue)
            {
                MessageBox.Show("Musisz wybrać godzinę wizyty.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void BtnAllVisits_Click(object sender, RoutedEventArgs e)
        {
            AllVisitsAdmin allVisits = new AllVisitsAdmin(_currentUser);
            allVisits.Show();
            this.Hide();
        }

        private void BtnNewVisit_Click(object sender, RoutedEventArgs e)
        {
            NewVisitAdmin newVisitAdmin = new NewVisitAdmin(_currentUser, this);
            newVisitAdmin.Show();
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

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            _parentWindow?.Show();
        }

        private void TimeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Pobieramy wybrany item z comboboxa
            ListBoxItem selectedItem = TimeComboBox.SelectedItem as ListBoxItem;

            // Jeśli coś zostało wybrane, pobieramy id slotu
            if (selectedItem != null)
            {
                // Zapisujemy id wybranego slotu do zmiennej _selectedSlotId
                _selectedSlotId = (int)selectedItem.Tag;

            }
            else
            {
                // Jeśli nic nie wybrano, resetujemy _selectedSlotId
                _selectedSlotId = null;
            }
        }
    }
}
