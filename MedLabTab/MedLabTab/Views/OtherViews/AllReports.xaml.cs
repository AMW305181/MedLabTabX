using MedLabTab.DatabaseManager;
using MedLabTab.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// Logika interakcji dla klasy AllReports.xaml
    /// </summary>
    public partial class AllReports : Window
    {
        private Window _parentWindow;
        public AllReports(Window parentWindow)
        {
            InitializeComponent();
            LoadDoneTests();
            _parentWindow = parentWindow;
        }

        private void LoadDoneTests()
        {
            var doneTests = DbManager.GetActiveTests(); // do zmiany na GetDoneTests()
            if (doneTests != null)
            {
                RaportyDataGrid.ItemsSource = doneTests;
            }
            else
            {
                MessageBox.Show("Błąd podczas ładowania gotowych testów.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowReport_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            // odnalezienie kontekstu wiersza - czyli obiektu testu
            Test selectedTest = button?.DataContext as Test;

            if (selectedTest != null)
            {
                var viewReportWindow = new ShowReport(/*selectedTest,*/ this);
                viewReportWindow.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Nie udało się wczytać danych raportu.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            _parentWindow?.Show();
        }
    }
}
