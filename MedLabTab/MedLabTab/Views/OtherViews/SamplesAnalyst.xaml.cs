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

namespace MedLabTab.Views.OtherViews
{
    /// <summary>
    /// Interaction logic for Samples.xaml
    /// </summary>
    public partial class SamplesAnalyst : Window
    {
        private Window _parentWindow;
        private User _currentUser;
        public SamplesAnalyst(Window parentWindow, User currentUser)
        {
            InitializeComponent();
            _parentWindow = parentWindow;
            _currentUser = currentUser;
            LoadSamples();
        }
        public void LoadSamples()
        {
            var tests = DbManager.GetAnalystTests();

            if (tests != null)
            {
                var samples = tests.Select(t => new
                {
                    TestName = t.Test?.TestName,
                    Date = t.Visit?.TimeSlot?.Date,
                    Time = t.Visit?.TimeSlot?.Time,
                    Patient = $"{t.Patient.Name} {t.Patient.Surname}",
                    TestCategory = t.Test?.CategoryNavigation?.CategoryName,
                    OriginalTest = t
                }).ToList();

                SamplesDataGrid.ItemsSource = samples;
            }
            else
            {
                MessageBox.Show("Błąd podczas ładowania danych.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void BeingAnalysed_Click(object sender, RoutedEventArgs e)
        {
            dynamic selectedItem = SamplesDataGrid.SelectedItem;
            TestHistory oldTest = selectedItem.OriginalTest;

            bool updated = true;

            var newTest = new TestHistory
            {
                id = oldTest.id, // klucz główny musi być ten sam, żeby zaktualizować
                VisitId = oldTest.VisitId,
                TestId = oldTest.TestId,
                PatientId = oldTest.PatientId,
                AnalystId = oldTest.AnalystId,
                Status = 4 // do raportu
            };

            updated = DbManager.EditTestHistory(oldTest, newTest);
            

            if (updated)
            {
                MessageBox.Show("Status badania został zaktualizowany.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Nie udało się zaktualizować badania.", "Uwaga", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            LoadSamples(); // odśwież widok
        }

        public void AddResults_Click (object sender, RoutedEventArgs e)
        {

        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            _parentWindow?.Show();
        }
    }

}
