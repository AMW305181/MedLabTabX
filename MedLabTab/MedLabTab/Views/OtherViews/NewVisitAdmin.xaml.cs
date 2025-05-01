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
            visitTime = 0;

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

            //załadowanie listy dostepnych terminow
            DateComboBox.Items.Clear();
            var dates = DbManager.GetAllDates(); // tutaj metoda do poprawki - zalezy jak bedzie wygladal harmonogram
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
                //            Status = 1, // to chyba oznacza ze jest pierwszy etap jakby
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
