
using System.Windows;
using System.Windows.Controls;
using MedLabTab.Views;
using MedLabTab.Views.OtherViews;
using System.Linq;
using MedLabTab.DatabaseModels;
using System.Collections.ObjectModel;
using MedLabTab.ViewModels;

namespace MedLabTab.Views.MainViews
{
    public partial class MainViewNurse : Window
    {
        private readonly MedLabContext _context;
        public ObservableCollection<Test> UpcomingTests { get; set; }

        private SignedInUser currentUser;
        public MainViewNurse(SignedInUser user)
        {
            InitializeComponent();
            _context = new MedLabContext();
            LoadUpcomingTests();
            ShowUpcomingTests();
            DataContext = this;
            currentUser = user;
        }

        private void LoadUpcomingTests()
        {
            UpcomingTests = new ObservableCollection<Test>(
                _context.Tests
                    .Where(t => t.IsActive)
                    .OrderBy(t => t.TestName)
                    .ToList());

            TestsDataGrid.ItemsSource = UpcomingTests;
        }

        private void ShowUpcomingTests()
        {
            ContentArea.Children.Clear();

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition());

            var header = new TextBlock
            {
                Text = "NADCHODZĄCE BADANIA",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 15)
            };

            var dataGrid = new DataGrid
            {
                ItemsSource = UpcomingTests,
                AutoGenerateColumns = false,
                IsReadOnly = true,
                SelectionMode = DataGridSelectionMode.Single,
                HeadersVisibility = DataGridHeadersVisibility.Column,
                BorderThickness = new Thickness(0),
                Background = System.Windows.Media.Brushes.Transparent
            };

            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Nazwa badania",
                Binding = new System.Windows.Data.Binding("TestName"),
                Width = new DataGridLength(1, DataGridLengthUnitType.Star)
            });

            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Kategoria",
                Binding = new System.Windows.Data.Binding("CategoryNavigation.CategoryName"),
                Width = new DataGridLength(1, DataGridLengthUnitType.Star)
            });

            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Opis",
                Binding = new System.Windows.Data.Binding("Description"),
                Width = new DataGridLength(2, DataGridLengthUnitType.Star)
            });

            dataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Cena",
                Binding = new System.Windows.Data.Binding("Price") { StringFormat = "{0:C}" },
                Width = new DataGridLength(1, DataGridLengthUnitType.Star)
            });

            Grid.SetRow(header, 0);
            Grid.SetRow(dataGrid, 1);

            grid.Children.Add(header);
            grid.Children.Add(dataGrid);

            ContentArea.Children.Add(grid);
        }
        //jak coś się spierniczy, to tutaj ----->
        private void BtnExams_Click(object sender, RoutedEventArgs e)
        {
            AllVisitsAdmin allVisits = new AllVisitsAdmin(this);
            allVisits.Show();
            this.Hide();
        }

        private void BtnSamples_Click(object sender, RoutedEventArgs e)
        {
            Samples samples = new Samples();
            samples.Show();
            this.Close();
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            Profile profile = new Profile(currentUser, this);
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