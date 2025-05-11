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
    public partial class Samples : Window
    {
        private Window _parentWindow;
        private User _currentUser;
        public Samples(Window parentWindow, User currentUser)
        {
            InitializeComponent();
            _parentWindow = parentWindow;
            _currentUser = currentUser;
            LoadSamples();
        }
        public void LoadSamples()
        {
            var tests = DbManager.GetAllTestHistories();

            if (tests != null)
            {
                var samples = tests.Select(t => new
                {
                    TestName = t.Test?.TestName,
                    Date = t.Visit?.TimeSlot?.Date,
                    Time = t.Visit?.TimeSlot?.Time,
                    Patient = $"{t.Patient.Name} {t.Patient.Surname}",
                    TestCategory = t.Test?.CategoryNavigation?.CategoryName,
                    Status = t.Status == 3 ? "Do analizy" : "Do uzupełnienia wyniki", // tu jakiś enum żeby pokazywało wszystkie opcje
                    OriginalTest = t
                }).ToList();

                SamplesDataGrid.ItemsSource = samples;
            }
            else
            {
                MessageBox.Show("Błąd podczas ładowania danych.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            _parentWindow?.Show();
        }
    }
}
