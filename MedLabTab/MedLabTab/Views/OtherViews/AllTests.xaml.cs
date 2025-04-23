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
    /// <summary>
    /// Interaction logic for Tests.xaml
    /// </summary>
    public partial class AllTests : Window
    {
        private Window _parentWindow;
        public AllTests(Window parentWindow)
        {
            InitializeComponent();
            LoadTests(); // Załaduj dane po inicjalizacji okna
            _parentWindow = parentWindow;
        }

        private void LoadTests()
        {
            var tests = DbManager.GetAllTests(); // <-- Zmienna klasa, np. TestRepository
            if (tests != null)
            {
                BadaniaDataGrid.ItemsSource = tests;
            }
            else
            {
                MessageBox.Show("Błąd podczas ładowania aktywnych testów.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddTest_Click(object sender, RoutedEventArgs e)
        {
            var newTestWindow = new NewTest(this);
            newTestWindow.Show();
            this.Hide();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            // odnalezienie kontekstu wiersza - czyli obiektu testu
            Test selectedTest = button?.DataContext as Test;

            if (selectedTest != null)
            {
                var editTestWindow = new EditTest(selectedTest, this);
                editTestWindow.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Nie udało się wczytać danych badania.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Deactivate_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Test selectedTest = button?.DataContext as Test;

            if (selectedTest != null)
            {
                var result = MessageBox.Show(
                    $"Czy na pewno chcesz usunąć badanie \"{selectedTest.TestName}\"?",
                    "Potwierdzenie usunięcia",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    bool deleted = DbManager.DeleteTest(selectedTest);

                    if (deleted)
                    {
                        MessageBox.Show("Badanie zostało usunięte.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadTests(); // Odświeżenie tabeli
                    }
                    else
                    {
                        MessageBox.Show("Wystąpił błąd podczas usuwania badania.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Nie udało się pobrać wybranego badania.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            _parentWindow?.Show();
        }
    }
}
