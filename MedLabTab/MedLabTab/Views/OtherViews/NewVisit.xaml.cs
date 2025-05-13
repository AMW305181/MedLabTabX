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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MedLabTab.Views.OtherViews
{
    /// <summary>
    /// Interaction logic for NewVisit.xaml
    /// </summary>
    public partial class NewVisit : Window
    {
        private Window _parentWindow;
        private float visitCost;
        private int visitTime;
        private int? _selectedSlotId;
        private List<Schedule> _AvaibleSlots;
        private User _currentUser;
        private bool _isInitialLoad = true;
        public NewVisit(User currentUser, Window parentWindow)
        {
            InitializeComponent();
            ClearForm();
            this.DataContext = this;
            LoadData();
            _parentWindow = parentWindow;
            _currentUser = currentUser;

            PatientTextBox.Text = currentUser != null
    ? $"{currentUser.Name} {currentUser.Surname}"
    : "Nie zalogowano";
        }

        private void LoadData()
        {
            visitCost = 0;
            visitTime = 0;

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
            //IsPaidCheckBox.IsChecked = false;
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
                                bool testPaymentStatus = false;
                                bool testIsActive = IsActiveCheckBox.IsChecked ?? false;
                                int testPatientId = _currentUser.id;
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

                                    MessageBox.Show("Visit and tests registered successfully!", "Success",
                                                   MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                else
                                {
                                    MessageBox.Show("Failed to register visit.", "Error",
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
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error",
                                   MessageBoxButton.OK, MessageBoxImage.Error);
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
                //visitTime += 15; // zakładamy że każde badanie trwa 15 minut
                visitTime = 15; // łączny czas wizyty to 15 minut (założeneie)

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
                    if (TestsListBox.Items.Count == 1){ //łączny czas wizyty to 15 minut (założeneie)
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

        /*private void PatientComboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (TestsListBox.SelectedItem is ListBoxItem selectedItem)
            {

            }

        }*/

        private void VisitCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            // Czyścimy combobox z poprzednich wartości
            TimeComboBox.Items.Clear();
            _selectedSlotId = null;
            if (VisitCalendar.SelectedDate.HasValue)
            {
                DateOnly selectedDate = DateOnly.FromDateTime(VisitCalendar.SelectedDate.Value);
                _AvaibleSlots = DbManager.GetAvailableSlotsForDate(selectedDate);
                // Sprawdzamy czy są dostępne sloty
                if (_AvaibleSlots != null && _AvaibleSlots.Count > 0)
                {
                    // Wypełniamy combobox dostępnymi slotami
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
                else if (!_isInitialLoad) // Only show message box if it's not the initial load
                {
                    // Jeśli nie ma dostępnych slotów, informujemy o tym użytkownika
                    MessageBox.Show("Brak dostępnych terminów na wybrany dzień.",
                        "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

            // After first execution, set flag to false
            _isInitialLoad = false;
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

        private void BtnEditVisit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnCancelVisit_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button)) return;

            if (!(button.DataContext is Visit selectedVisit))
            {
                MessageBox.Show("Nie udało się pobrać wybranej wizyty.",
                              "Błąd",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show(
                $"Czy na pewno chcesz anulować wizytę z dnia {selectedVisit.TimeSlot?.Date.ToString("dd.MM.yyyy")} o {selectedVisit.TimeSlot?.Time.ToString("hh\\:mm")}?",
                "Potwierdzenie anulowania",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                bool success = DbManager.DeactivateVisit(selectedVisit);

                if (success)
                {
                    MessageBox.Show("Wizyta została anulowana.",
                                  "Sukces",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Nie udało się anulować wizyty.",
                                  "Błąd",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd: {ex.Message}",
                              "Błąd",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void BtnExams_Click(object sender, RoutedEventArgs e)
        {
            AllTests allTests = new AllTests(_currentUser, this);
            allTests.Show();
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
            NewVisit newVisit = new NewVisit(_currentUser, this);
            newVisit.Show();
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

        private void NewVisit2_Click(object sender, RoutedEventArgs e)
        {
            NewVisit newVisit = new NewVisit(_currentUser, this);
            newVisit.Show();
            this.Hide();
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
