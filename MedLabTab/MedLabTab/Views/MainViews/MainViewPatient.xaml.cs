using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using MedLabTab.DatabaseModels;
using MedLabTab.ViewModels;
using MedLabTab.Views.OtherViews;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MedLabTab.Views.MainViews
{
    /// <summary>
    /// Interaction logic for MainViewPatient.xaml
    /// </summary>
    public partial class MainViewPatient : Window
    {
        private SignedInUser currentUser;
        private readonly MedLabContext _context;
        private ObservableCollection<Visit> UpcomingVisits { get; set; }

        public MainViewPatient(SignedInUser user)
        {
            InitializeComponent();

            // Ensure user is not null before proceeding
            if (user == null)
            {
                MessageBox.Show("Błąd: Nie można załadować danych użytkownika.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            currentUser = user;

            try
            {
                _context = new MedLabContext();
                UpcomingVisits = new ObservableCollection<Visit>();
                DataContext = this;

                // Initialize the DataGrid first
                InitializeVisitsDataGrid();

                // Then load the data
                LoadUpcomingVisits();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas inicjalizacji: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitializeVisitsDataGrid()
        {
            // Set up the DataGrid without data first
            VisitsDataGrid.AutoGenerateColumns = false;
            VisitsDataGrid.IsReadOnly = true;
            VisitsDataGrid.SelectionMode = DataGridSelectionMode.Single;
            VisitsDataGrid.HeadersVisibility = DataGridHeadersVisibility.Column;
            VisitsDataGrid.BorderThickness = new Thickness(0);
            VisitsDataGrid.Background = Brushes.Transparent;

            // Add columns
            VisitsDataGrid.Columns.Clear();
            VisitsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Data",
                Binding = new Binding("DisplayDate"),
                Width = new DataGridLength(1, DataGridLengthUnitType.Star)
            });

            VisitsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Godzina",
                Binding = new Binding("DisplayTime"),
                Width = new DataGridLength(1, DataGridLengthUnitType.Star)
            });

            VisitsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Pielęgniarka",
                Binding = new Binding("DisplayNurse"),
                Width = new DataGridLength(1, DataGridLengthUnitType.Star)
            });

            VisitsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Badania",
                Binding = new Binding("DisplayTests"),
                Width = new DataGridLength(2, DataGridLengthUnitType.Star)
            });

            VisitsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Koszt",
                Binding = new Binding("DisplayCost"),
                Width = new DataGridLength(1, DataGridLengthUnitType.Star)
            });
        }

        private void LoadUpcomingVisits()
        {
            try
            {
                // Clear existing items
                UpcomingVisits.Clear();

                if (_context == null)
                {
                    MessageBox.Show("Błąd: Kontekst bazy danych jest pusty.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (currentUser == null || currentUser.id <= 0)
                {
                    MessageBox.Show("Błąd: Nieprawidłowy identyfikator użytkownika.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Load visits using safe approach
                var visits = _context.Visits
                    .AsNoTracking() // Improves performance for read-only operations
                    .Include(v => v.TimeSlot)
                        .ThenInclude(t => t.Nurse)
                    .Include(v => v.TestHistories)
                        .ThenInclude(th => th.Test)
                    .Where(v => v.PatientId == currentUser.id && v.IsActive)
                    .OrderBy(v => v.TimeSlot.Date)
                    .ThenBy(v => v.TimeSlot.Time)
                    .ToList();

                foreach (var visit in visits)
                {
                    UpcomingVisits.Add(visit);
                }

                // Set the DataGrid's ItemsSource
                VisitsDataGrid.ItemsSource = UpcomingVisits;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas ładowania wizyt: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowUpcomingVisits()
        {
            // Since we're now using the XAML-defined DataGrid (VisitsDataGrid), 
            // we don't need to programmatically create it again
            LoadUpcomingVisits();
        }

        private void BtnExams_Click(object sender, RoutedEventArgs e)
        {
            AllTests allTests = new AllTests(currentUser, this);
            allTests.Show();
            this.Hide();
        }

        private void BtnVisits_Click(object sender, RoutedEventArgs e)
        {
            MyVisits allVisits = new MyVisits(currentUser, this);
            allVisits.Show();
            this.Hide();
        }

        private void BtnNewVisit_Click(object sender, RoutedEventArgs e)
        {
            NewVisit newVisit = new NewVisit(currentUser, this);
            newVisit.Show();
            this.Hide();
        }

        private void BtnResults_Click(object sender, RoutedEventArgs e)
        {
            // Implementation pending
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            var profile = new Profile(currentUser, this);
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
    }
}