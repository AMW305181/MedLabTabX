using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using MedLabTab.DatabaseModels;
using MedLabTab.DatabaseManager;
using System.Globalization;
using MedLabTab.ViewModels;

namespace MedLabTab.Views.OtherViews
{
    public partial class NewTest : Window
    {
        private Window _parentWindow;
        private SignedInUser _currentUser;
        public NewTest(SignedInUser currentUser, Window parentWindow)
        {
            InitializeComponent();
            InitializeCategories();
            ClearForm();
            PriceTextBox.PreviewTextInput += NumberValidationTextBox;
            _parentWindow = parentWindow;
            _currentUser = currentUser;
            CategoryComboBox.SelectionChanged += CategoryComboBox_SelectionChanged;
        }

        private void InitializeCategories()
        {
            CategoryComboBox.Items.Clear();
            var categories = DbManager.GetCategories();
            foreach (var cat in categories)
            {
                CategoryComboBox.Items.Add(new ComboBoxItem
                {
                    Content = cat.CategoryName,
                    Tag = cat.id
                });
            }
            CategoryComboBox.Items.Add(new ComboBoxItem
            {
                Content = "Nowa kategoria...",
                Tag = -1
            });
            if (CategoryComboBox.Items.Count > 1)
                CategoryComboBox.SelectedIndex = 0;
        }

        private void ClearForm()
        {
            TestNameTextBox.Text = string.Empty;
            DescriptionTextBox.Text = string.Empty;
            PriceTextBox.Text = string.Empty;
            IsActiveCheckBox.IsChecked = true;
        }

        private void NumberValidationTextBox(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.,]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInputs())
            {
                var selectedCategory = (ComboBoxItem)CategoryComboBox.SelectedItem;
                int categoryId = (int)selectedCategory.Tag;

                var newTest = new Test
                {
                    TestName = TestNameTextBox.Text.Trim(),
                    Description = DescriptionTextBox.Text.Trim(),
                    Price = float.Parse(PriceTextBox.Text.Replace(',', '.'), CultureInfo.InvariantCulture),
                    Category = categoryId,
                    IsActive = IsActiveCheckBox.IsChecked == true
                };

                bool exists = DbManager.IsTestNameTaken(newTest.TestName);
                if (exists) { MessageBox.Show("Badanie o podanej nazwie już istnieje.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                bool added = DbManager.AddTest(newTest); // zakładamy że zwraca bool

                if (added)
                {
                    if (_parentWindow is AllTestsAdmin allTestsAdmin)
                        allTestsAdmin.LoadTests();
                    MessageBox.Show("Badanie zostało dodane pomyślnie!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                    _parentWindow?.Show();
                }
                else
                {
                    MessageBox.Show("Wystąpił błąd podczas dodawania badania.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(TestNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ||
                string.IsNullOrWhiteSpace(PriceTextBox.Text))
            {
                MessageBox.Show("Wszystkie pola muszą być wypełnione.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (CategoryComboBox.SelectedItem == null)
            {
                MessageBox.Show("Wybierz kategorię badania.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            string input = PriceTextBox.Text.Replace(',', '.');

            if (!float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out float price))
            {
                MessageBox.Show("Cena musi być liczbą.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (price < 0)
            {
                MessageBox.Show("Cena musi być liczbą dodatnią.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void ShowNewCategoryDialog()
        {
            var dialog = new Window
            {
                Title = "Nowa kategoria",
                Width = 300,
                Height = 150,
                WindowState = WindowState.Normal,
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this
            };

            var stackPanel = new StackPanel { Margin = new Thickness(10) };
            var textBoxStyle = this.FindResource("TextBox") as Style;
            var textBox = new TextBox { Margin = new Thickness(0, 0, 0, 10),
                                        Style = textBoxStyle};
            var buttonStyle = this.FindResource("NavButtonStyle2") as Style;
            var deletebuttonStyle = this.FindResource("DeleteButtonStyle") as Style;
            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 0)
            };

            var Okbutton = new Button
            {
                Content = "Zapisz",
                Margin = new Thickness(5, 0, 0, 0),
                Width = 100,
                Style = buttonStyle
            };

            var Cancelbutton = new Button
            {
                Content = "Anuluj",
                Margin = new Thickness(5, 0, 0, 0),
                Width = 100,
                Style = deletebuttonStyle
            };
            buttonPanel.Children.Add(Okbutton);
            buttonPanel.Children.Add(Cancelbutton);

            Okbutton.Click += (s, e) =>
            {
                string newCategoryName = textBox.Text.Trim();

                if (string.IsNullOrWhiteSpace(newCategoryName))
                {
                    MessageBox.Show("Nazwa kategorii nie może być pusta.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                bool exists = DbManager.GetCategories().Any(c =>
                    string.Equals(c.CategoryName, newCategoryName, StringComparison.OrdinalIgnoreCase));

                if (exists)
                {
                    MessageBox.Show("Kategoria o podanej nazwie już istnieje.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                bool added = DbManager.AddCategory(newCategoryName);

                if (added)
                {
                    MessageBox.Show("Nowa kategoria została dodana.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                    InitializeCategories(); 
                    dialog.Close();
                }
                else
                {
                    MessageBox.Show("Wystąpił błąd podczas dodawania kategorii.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            Cancelbutton.Click += (s, e) => dialog.Close();

            stackPanel.Children.Add(new TextBlock { Text = "Wprowadź nazwę nowej kategorii:" });
            stackPanel.Children.Add(textBox);
            stackPanel.Children.Add(buttonPanel);
            dialog.Content = stackPanel;

            dialog.ShowDialog();
        }

        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryComboBox.SelectedItem is ComboBoxItem selectedItem && (int)selectedItem.Tag == -1)
            {
                ShowNewCategoryDialog();
                CategoryComboBox.SelectionChanged -= CategoryComboBox_SelectionChanged;
                CategoryComboBox.SelectedIndex = 0;
                CategoryComboBox.SelectionChanged += CategoryComboBox_SelectionChanged;
            }
        }


        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            _parentWindow?.Show();
        }

     private void BtnAllVisits_Click(object sender, RoutedEventArgs e)
        {
            AllVisitsAdmin allVisits = new AllVisitsAdmin(_currentUser);
            allVisits.Show();
            this.Hide();
        }

        private void BtnNewVisit_Click(object sender, RoutedEventArgs e)
        {
            NewVisitAdmin newVisitAdmin = new NewVisitAdmin(_currentUser, this);
            newVisitAdmin.Show();
            this.Hide();
        }

        private void BtnSchedule_Click(object sender, RoutedEventArgs e)
        {
            EditSchedule editschedule = new EditSchedule(_currentUser);
            editschedule.Show();
            this.Hide();
        }

        private void BtnSamples_Click(object sender, RoutedEventArgs e)
        {
            Samples samples = new Samples(_currentUser);
            samples.Show();
            this.Hide();
        }

        private void BtnAllExams_Click(object sender, RoutedEventArgs e)
        {
            AllTestsAdmin allTests = new AllTestsAdmin(_currentUser, this);
            allTests.Show();
            this.Hide();
        }

        private void BtnNewExam_Click(object sender, RoutedEventArgs e)
        {
            NewTest newTest = new NewTest(_currentUser, this);
            newTest.Show();
            this.Hide();
        }

        private void BtnAllUsers_Click(object sender, RoutedEventArgs e)
        {
            AllUsers allUsers = new AllUsers(_currentUser);
            allUsers.Show();
            this.Close();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            Registration registration = new Registration(_currentUser);
            registration.Show();
            this.Close();
        }

        private void BtnReports_Click(object sender, RoutedEventArgs e)
        {
            AllReports allReports = new AllReports(_currentUser, this);
            allReports.Show();
            this.Hide();
        }

        private void BtnStats_Click(object sender, RoutedEventArgs e)
        {
            Statistics statistics = new Statistics(_currentUser);
            statistics.Show();
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
