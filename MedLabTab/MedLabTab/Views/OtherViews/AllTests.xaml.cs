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

namespace MedLabTab.Views.OtherViews
{
    /// <summary>
    /// Interaction logic for Tests.xaml
    /// </summary>
    public partial class AllTests : Window
    {
        public AllTests()
        {
            InitializeComponent();
            LoadActiveTests(); // Załaduj dane po inicjalizacji okna
        }

        private void LoadActiveTests()
        {
            var activeTests = DbManager.GetActiveTests(); // <-- Zmienna klasa, np. TestRepository
            if (activeTests != null)
            {
                BadaniaDataGrid.ItemsSource = activeTests;
            }
            else
            {
                MessageBox.Show("Błąd podczas ładowania aktywnych testów.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddTest_Click(object sender, RoutedEventArgs e)
        {
            var newTestWindow = new NewTest();
            newTestWindow.Show();
            this.Close();
        }
    }
}
