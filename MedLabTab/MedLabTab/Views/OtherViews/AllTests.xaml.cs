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
using MedLabTab.DatabaseManager;
using MedLabTab.DatabaseModels;

namespace MedLabTab.Views.OtherViews
{
    public partial class AllTests : Window
    {
        private Window _parentWindow;
        public AllTests(Window parentWindow)
        {
            InitializeComponent();
            LoadTests(); // Załaduj dane po inicjalizacji okna
            _parentWindow = parentWindow;
        }
        public void LoadTests()
        {
            var tests = DbManager.GetActiveTests();
            var categoryDict = DbManager.GetCategoriesDictionary();

            if (tests != null && categoryDict != null)
            {
                var mappedTests = tests.Select(t => new
                {
                    t.TestName,
                    t.Description,
                    t.Price,
                    Category = categoryDict.TryGetValue(t.Category, out var catName) ? catName : "Nieznana",
                    OriginalTest = t // jeśli chcesz móc edytować dalej test - tego trochę nie czaje
                }).ToList();

                BadaniaDataGrid.ItemsSource = mappedTests;
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
