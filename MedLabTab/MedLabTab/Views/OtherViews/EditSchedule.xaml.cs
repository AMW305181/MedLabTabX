using MedLabTab.DatabaseManager;
using MedLabTab.DatabaseModels;
using MedLabTab.ViewModels;
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
    /// Logika interakcji dla klasy EditeSchedule.xaml
    /// </summary>
    public partial class EditSchedule : Window
    {
        TimeOnly StartTime, EndTime;
        private SignedInUser currentUser;
        private User _selectedNurse;
        private bool _isInitialLoad = true;
        private List<Schedule> _availableSlots;
        public EditSchedule(SignedInUser user)
        {
            InitializeComponent();
            InitializeTimeComboBoxes();
            LoadData();
            currentUser = user;
        }
        private void UpdateTimeSummary()
        {
            // Sprawdź, czy obie godziny są wybrane
            if (StartTime != default && EndTime != default)
            {
                // Aktualizuj podsumowanie
                SummaryTimeTextBlock.Text = $"{StartTime.ToString("HH:mm")} - {EndTime.ToString("HH:mm")}";

                // Opcjonalnie: możesz dodać sprawdzenie poprawności zakresu czasu
                if (StartTime >= EndTime)
                {
                    // Wyświetl ostrzeżenie
                    SummaryTimeTextBlock.Foreground = Brushes.Red;
                }
                else
                {
                    // Normalny tekst
                    SummaryTimeTextBlock.Foreground = Brushes.Black;
                }
            }
            else
            {
                SummaryTimeTextBlock.Text = "-";
            }
        }
        private void LoadData()
        {
            NurseComboBox.Items.Clear();
            var users = DbManager.GetActiveNurses();
            foreach (var user in users)
            {
                NurseComboBox.Items.Add(new ComboBoxItem
                {
                    Content = user.Name + " " + user.Surname,
                    Tag = user.PESEL
                });
            }

            if (NurseComboBox.Items.Count > 0)
                NurseComboBox.SelectedIndex = 0;
        }
        private void InitializeTimeComboBoxes()
        {
            // Czyszczenie istniejących pozycji
            FromTimeComboBox.Items.Clear();
            ToTimeComboBox.Items.Clear();

            // Dodawanie godzin w odstępach 15-minutowych
            for (int hour = 7; hour <= 20; hour++)
            {
                FromTimeComboBox.Items.Add($"{hour:D2}:00");
                FromTimeComboBox.Items.Add($"{hour:D2}:15");
                FromTimeComboBox.Items.Add($"{hour:D2}:30");
                FromTimeComboBox.Items.Add($"{hour:D2}:45");

                ToTimeComboBox.Items.Add($"{hour:D2}:00");
                ToTimeComboBox.Items.Add($"{hour:D2}:15");
                ToTimeComboBox.Items.Add($"{hour:D2}:30");
                ToTimeComboBox.Items.Add($"{hour:D2}:45");
            }

            // Dodaj ostatnie godziny
            ToTimeComboBox.Items.Add("21:00");

            // Ustaw domyślne wartości
            FromTimeComboBox.SelectedItem = "08:00";
            ToTimeComboBox.SelectedItem = "16:00";

            // Inicjalizuj zmienne StartTime i EndTime
            if (FromTimeComboBox.SelectedItem != null)
            {
                StartTime = TimeOnly.Parse(FromTimeComboBox.SelectedItem.ToString());
            }

            if (ToTimeComboBox.SelectedItem != null)
            {
                EndTime = TimeOnly.Parse(ToTimeComboBox.SelectedItem.ToString());
            }

            UpdateTimeSummary();
        }
        private void BtnAllVisits_Click(object sender, RoutedEventArgs e)
        {
            AllVisitsAdmin allVisits = new AllVisitsAdmin(currentUser);
            allVisits.Show();
            this.Hide();
        }

        private void BtnNewVisit_Click(object sender, RoutedEventArgs e)
        {
            NewVisitAdmin newVisit = new NewVisitAdmin(currentUser, this);
            newVisit.Show();
            this.Hide();
        }

        private void BtnSchedule_Click(object sender, RoutedEventArgs e)
        {
            EditSchedule editschedule = new EditSchedule(currentUser);
            editschedule.Show();
            this.Hide();
        }

        private void BtnSamples_Click(object sender, RoutedEventArgs e)
        {
            Samples samples = new Samples(currentUser);
            samples.Show();
            this.Hide();
        }

        private void BtnAllExams_Click(object sender, RoutedEventArgs e)
        {
            AllTestsAdmin allTests = new AllTestsAdmin(currentUser, this);
            allTests.Show();
            this.Hide();
        }

        private void BtnNewExam_Click(object sender, RoutedEventArgs e)
        {
            NewTest newTest = new NewTest(currentUser, this);
            newTest.Show();
            this.Hide();
        }

        private void BtnAllUsers_Click(object sender, RoutedEventArgs e)
        {
            AllUsers allUsers = new AllUsers(currentUser);
            allUsers.Show();
            this.Close();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            Registration registration = new Registration(currentUser);
            registration.Show();
            this.Close();
        }

        private void BtnReports_Click(object sender, RoutedEventArgs e)
        {
            AllReports allReports = new AllReports(currentUser, this);
            allReports.Show();
            this.Hide();
        }

        private void BtnStats_Click(object sender, RoutedEventArgs e)
        {
            Statistics statistics = new Statistics(currentUser);
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

        private string GetNurseName(int nurseId)
        {
            return DbManager.GetUserById(nurseId).Name;
        }

        private void ScheduleCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            ExistingSchedulesListBox.Items.Clear();

            if (ScheduleCalendar.SelectedDate.HasValue)
            {
                DateOnly selectedDate = DateOnly.FromDateTime(ScheduleCalendar.SelectedDate.Value);
                _availableSlots = DbManager.GetAllSlotsForDate(selectedDate);

                if (_availableSlots != null && _availableSlots.Count > 0)
                {
                    var slotsByNurse = _availableSlots.GroupBy(s => s.NurseId);

                    foreach (var nurseGroup in slotsByNurse)
                    {
                        string nurseName = GetNurseName(nurseGroup.Key);
                        var sortedSlots = nurseGroup.OrderBy(s => s.Time).ToList();
                        List<(TimeOnly Start, TimeOnly End)> timeRanges = new List<(TimeOnly Start, TimeOnly End)>();

                        if (sortedSlots.Count > 0)
                        {
                            TimeOnly rangeStart = sortedSlots[0].Time;
                            TimeOnly expectedNext = rangeStart.AddMinutes(15);

                            for (int i = 1; i < sortedSlots.Count; i++)
                            {
                                if (sortedSlots[i].Time != expectedNext)
                                {
                                    timeRanges.Add((rangeStart, expectedNext));
                                    rangeStart = sortedSlots[i].Time;
                                }

                                expectedNext = sortedSlots[i].Time.AddMinutes(15);
                            }

                            timeRanges.Add((rangeStart, expectedNext));
                            string scheduleSummary = nurseName + ": ";
                            for (int i = 0; i < timeRanges.Count; i++)
                            {
                                if (i > 0)
                                    scheduleSummary += ", ";

                                scheduleSummary += $"{timeRanges[i].Start.ToString("HH:mm")}-{timeRanges[i].End.ToString("HH:mm")}";
                            }

                            ListBoxItem item = new ListBoxItem
                            {
                                Content = scheduleSummary,
                                Tag = nurseGroup.Key
                            };

                            ExistingSchedulesListBox.Items.Add(item);
                        }
                    }
                }
                /*else if (!_isInitialLoad) // Only show message box if it's not the initial load
                {
                    MessageBox.Show("Brak harmonogramów na wybrany dzień.",
                        "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
                }*/

                // Aktualizacja wybranej daty w podsumowaniu
                SummaryDateTextBlock.Text = selectedDate.ToString("dd.MM.yyyy");
            }

            // After first execution, set flag to false
            _isInitialLoad = false;
        }

        private void NurseComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NurseComboBox.SelectedItem != null)
            {
                ComboBoxItem selectedItem = NurseComboBox.SelectedItem as ComboBoxItem;
                if (selectedItem != null)
                {
                    string pesel = selectedItem.Tag as string;
                    if (!string.IsNullOrEmpty(pesel))
                    {
                        User user = DbManager.GetUser(pesel);
                        if (user != null)
                        {
                            _selectedNurse = user;
                            SummaryNurseTextBlock.Text = user.Name + " " + user.Surname;
                        }
                    }
                }
            }
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            //if (ValidateInputs())
            //{
                // Convert the time range into 15-minute slots
                List<Schedule> newSlots = new List<Schedule>();
                DateOnly selectedDate = DateOnly.FromDateTime(ScheduleCalendar.SelectedDate.Value);

                // Create slots from StartTime to EndTime in 15-minute increments
                TimeOnly currentTime = StartTime;
                while (currentTime < EndTime)
                {
                    Schedule slot = new Schedule
                    {
                        NurseId = _selectedNurse.id,
                        Date = selectedDate,
                        Time = currentTime
                    };
                    newSlots.Add(slot);
                    currentTime = currentTime.AddMinutes(15);
                }

                // Add the slots to the database
                if (newSlots.Count > 0)
                {
                    bool success = AddSchedule(newSlots);
                    if (success)
                    {
                        MessageBox.Show("Dodano do harmonogramu",
                               "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Refresh the displayed schedule
                        ScheduleCalendar_SelectedDatesChanged(null, null);
                    }
                    else
                    {
                        MessageBox.Show("Nie udało się dodać do harmonogramu",
                               "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Nie wybrano żadnych slotów czasowych",
                           "Ostrzeżenie", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            //}
        }

        private bool AddSchedule(List<Schedule> slots)
        {
            try
            {
                // Check for overlapping slots before adding
                foreach (var slot in slots)
                {
                    // Check if this slot already exists for this nurse on this date
                    bool slotExists = _availableSlots?.Any(s =>
                        s.NurseId == slot.NurseId &&
                        s.Date == slot.Date &&
                        s.Time == slot.Time) ?? false;

                    if (slotExists)
                    {
                        MessageBox.Show($"Slot {slot.Time.ToString("HH:mm")} już istnieje w harmonogramie",
                               "Konflikt", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                }

                // All slots are new, so add them to the database
                DbManager.AddScheduleSlots(slots);

                // Update the available slots list with the newly added slots
                if (_availableSlots == null)
                    _availableSlots = new List<Schedule>();

                _availableSlots.AddRange(slots);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas dodawania harmonogramu: {ex.Message}",
                       "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private void FromTimeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FromTimeComboBox.SelectedItem != null)
            {
                StartTime = TimeOnly.Parse(FromTimeComboBox.SelectedItem.ToString());
                UpdateTimeSummary();
            }
        }

        private void ToTimeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ToTimeComboBox.SelectedItem != null)
            {
                EndTime = TimeOnly.Parse(ToTimeComboBox.SelectedItem.ToString());
                UpdateTimeSummary();
            }
        }
        /*private bool ValidateInputs()
        {
            bool valid = true;

            return valid;
        }*/
    }
}