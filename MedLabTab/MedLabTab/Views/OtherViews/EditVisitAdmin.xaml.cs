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
        private Visit _originalVisit;
        private AllVisitsAdmin _parentWindow;
        private float visitCost;
        private int visitTime;
        public EditVisitAdmin(Visit visitToEdit, AllVisitsAdmin parentWindow)
        {
            InitializeComponent();
            _originalVisit = visitToEdit;
            _parentWindow = parentWindow;
            LoadData();
        }

        private void LoadData()
        {
            visitCost = _originalVisit.Cost;
            visitTime = (DbManager.GetTestsInVisit(_originalVisit.id).Count) * 15;

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
            visitCost = 0;
            visitTime = 0;
            foreach (ListBoxItem item in TestsListBox.Items)
            {
                if (item.Tag is int testId)
                {
                    var test = DbManager.GetTest(testId);
                    visitCost += test.Price;
                    visitTime += 15;
                }
            }
            UpdateValues();

            //załadowanie listy dostepnych terminow
            DateComboBox.Items.Clear();
            var dates = DbManager.GetAllDates(); // tutaj metoda do poprawki - zalezy jak bedzie wygladal harmonogram
            int selectedSchedule = -1;
            if (_originalVisit.TimeSlotId.HasValue)
            {
                var schedule = DbManager.GetSchedule(_originalVisit.TimeSlotId.Value);
                if (schedule != null)
                {
                    selectedSchedule = schedule.id;
                }
            }
            foreach (var date in dates)
            {
                var item = new ComboBoxItem
                {
                    Content = date.Date + " " + date.Time,
                    Tag = date.id
                };

                DateComboBox.Items.Add(item);

                if (date.id == selectedSchedule)
                {
                    DateComboBox.SelectedItem = item;
                }
            }

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
                int timeSlotId = (int)selectedTimeSlot.Tag;

                var newVisit = new Visit
                {
                    //id = _originalVisit.id,
                    Cost = visitCost,
                    PaymentStatus = IsPaidCheckBox.IsChecked == true,
                    IsActive = IsActiveCheckBox.IsChecked == true,
                    PatientId = (DbManager.GetUser(patientPESEL)).id,
                    TimeSlotId = timeSlotId,
                };

                bool editedVisit = DbManager.EditVisit(_originalVisit, newVisit);
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
                    _parentWindow?.LoadVisits();
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
                visitTime += 15; // zakładamy że każde badanie trwa 15 minut

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
                    visitTime -= 15;

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
    }
}