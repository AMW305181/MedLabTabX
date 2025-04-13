using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MedLabTab.DatabaseModels;
using BCrypt.Net;
using System.Text.RegularExpressions;
using MedLabTab.DatabaseManager;

namespace MedLabTab.Views.OtherViews
{
    public partial class Registration : Window
    {
        public Registration()
        {
            InitializeComponent();
            InitializeUserRoles();
            ClearForm();

            txtPesel.PreviewTextInput += NumberValidationTextBox;
            txtPhone.PreviewTextInput += NumberValidationTextBox;
        }

        private void InitializeUserRoles()
        {
            cmbUserRole.Items.Clear();
            cmbUserRole.Items.Add(new ComboBoxItem { Content = "Pacjent", Tag = 3 });
            cmbUserRole.Items.Add(new ComboBoxItem { Content = "Recepcja", Tag = 2 });
            cmbUserRole.Items.Add(new ComboBoxItem { Content = "Pielęgniarka", Tag = 4 });
            cmbUserRole.Items.Add(new ComboBoxItem { Content = "Analityk", Tag = 5 });
            cmbUserRole.SelectedIndex = 0;
        }

        private void ClearForm()
        {
            txtName.Text = string.Empty;
            txtSurname.Text = string.Empty;
            txtPesel.Text = string.Empty;
            txtPhone.Text = string.Empty;
            txtLogin.Text = string.Empty;
            txtPassword.Text = string.Empty;
            cmbUserRole.SelectedIndex = 0;
        }

        private void NumberValidationTextBox(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInputs())
            {
                    var selectedRole = (ComboBoxItem)cmbUserRole.SelectedItem;
                    int userType = (int)selectedRole.Tag;

                var newUser = new User
                {
                    Name = txtName.Text.Trim(),
                    Surname = txtSurname.Text.Trim(),
                    PESEL = txtPesel.Text.Trim(),
                    PhoneNumber = txtPhone.Text.Trim(),
                    Login = txtLogin.Text.Trim(),
                    Password = txtPassword.Text,//BCrypt.Net.BCrypt.HashPassword(txtPassword.Text),
                        UserType = userType,
                    IsActive = true
                };

                        if (DbManager.IsLoginTaken(newUser.Login))
                        {
                            MessageBox.Show("Login jest już zajęty.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        if (DbManager.IsPESELTaken(newUser.PESEL))
                        {
                            MessageBox.Show("Użytkownik z tym PESEL już istnieje.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                       bool userAdded= DbManager.AddUser(newUser);
                if (userAdded) { MessageBox.Show("Rejestracja zakończona pomyślnie!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information); }
                else { MessageBox.Show("Wystąpił błąd.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning); }

                    this.Close();
               
               
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtSurname.Text) ||
                string.IsNullOrWhiteSpace(txtPesel.Text) ||
                string.IsNullOrWhiteSpace(txtPhone.Text) ||
                string.IsNullOrWhiteSpace(txtLogin.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Wszystkie pola muszą być wypełnione.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (txtPesel.Text.Length != 11)
            {
                MessageBox.Show("PESEL musi składać się z 11 cyfr.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (txtPhone.Text.Length < 9)
            {
                MessageBox.Show("Numer telefonu musi zawierać co najmniej 9 cyfr.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (txtPassword.Text.Length < 6)
            {
                MessageBox.Show("Hasło musi zawierać co najmniej 6 znaków.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}