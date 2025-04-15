using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using MedLabTab.DatabaseModels;
using MedLabTab.DatabaseManager;

namespace MedLabTab.Views.OtherViews
{
    public partial class NewTest : Window
    {
        public NewTest()
        {
            InitializeComponent();
            InitializeCategories();
            ClearForm();
            PriceTextBox.PreviewTextInput += NumberValidationTextBox;
        }

        private void InitializeCategories()
        {
            //CategoryComboBox.Items.Clear();
            //var categories = DbManager.GetCategories(); 
            //foreach (var cat in categories)
            //{
            //    CategoryComboBox.Items.Add(new ComboBoxItem
            //    {
            //        Content = cat.Name,
            //        Tag = cat.Id
            //    });
            //}

            //if (CategoryComboBox.Items.Count > 0)
            //    CategoryComboBox.SelectedIndex = 0;
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
                    Price = float.Parse(PriceTextBox.Text.Replace(',', '.')), // Ujednolicenie separatora
                    Category = categoryId,
                    IsActive = IsActiveCheckBox.IsChecked == true
                };

                //bool added = DbManager.AddTest(newTest); // zakładamy że zwraca bool

                //if (added)
                //{
                //    MessageBox.Show("Badanie zostało dodane pomyślnie!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                //    this.Close();
                //}
                //else
                //{
                //    MessageBox.Show("Wystąpił błąd podczas dodawania badania.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                //}
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

            if (!float.TryParse(PriceTextBox.Text.Replace(',', '.'), out float price) || price < 0)
            {
                MessageBox.Show("Cena musi być liczbą dodatnią.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }


        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
