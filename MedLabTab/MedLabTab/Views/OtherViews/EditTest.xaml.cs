using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using MedLabTab.DatabaseModels;
using MedLabTab.DatabaseManager;

namespace MedLabTab.Views.OtherViews
{
    public partial class EditTest : Window
    {
        private Test _originalTest;
        private Window _parentWindow;

        public EditTest(Test testToEdit, Window parentWindow)
        {
            InitializeComponent();
            _originalTest = testToEdit;
            _parentWindow = parentWindow;
            InitializeCategories();
            FillFormWithData();
            PriceTextBox.PreviewTextInput += NumberValidationTextBox;
        }

        private void InitializeCategories()
        {
            CategoryComboBox.Items.Clear();
            var categories = DbManager.GetCategories();
            foreach (var cat in categories)
            {
                var item = new ComboBoxItem
                {
                    Content = cat.CategoryName,
                    Tag = cat.id
                };

                CategoryComboBox.Items.Add(item);

                // zaznacz kategorię testu
                if (cat.id == _originalTest.Category)
                    CategoryComboBox.SelectedItem = item;
            }

            if (CategoryComboBox.SelectedItem == null && CategoryComboBox.Items.Count > 0)
                CategoryComboBox.SelectedIndex = 0;
        }

        private void FillFormWithData()
        {
            TestNameTextBox.Text = _originalTest.TestName;
            DescriptionTextBox.Text = _originalTest.Description;
            PriceTextBox.Text = _originalTest.Price.ToString(CultureInfo.InvariantCulture);
            IsActiveCheckBox.IsChecked = _originalTest.IsActive;
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

                var updatedTest = new Test
                {
                    TestName = TestNameTextBox.Text.Trim(),
                    Description = DescriptionTextBox.Text.Trim(),
                    Price = float.Parse(PriceTextBox.Text.Replace(',', '.'), CultureInfo.InvariantCulture),
                    Category = categoryId,
                    IsActive = IsActiveCheckBox.IsChecked == true
                };

                bool success = DbManager.EditTest(_originalTest, updatedTest);

                if (success)
                {
                    MessageBox.Show("Badanie zostało zaktualizowane!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                    _parentWindow?.Show();
                }
                else
                {
                    MessageBox.Show("Wystąpił błąd podczas edycji badania.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            _parentWindow?.Show();
        }
    }
}
