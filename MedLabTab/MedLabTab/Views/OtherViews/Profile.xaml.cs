using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MedLabTab.DatabaseModels;
using BCrypt.Net;
using System.Text.RegularExpressions;
using MedLabTab.DatabaseManager;
using MedLabTab.ViewModels;

namespace MedLabTab.Views.OtherViews
{
    public partial class Profile : Window
    {
        private User _currentUser; //tu powinien być zalogowany użytkownik
        private Window _parentWindow;
        public Profile(User currentUser, Window parentWindow)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _parentWindow = parentWindow;
            FillForm();
            txtPhone.PreviewTextInput += NumberValidationTextBox;
        }

        private void FillForm()
        {
            if (_currentUser != null)
            {
                txtName.Text = _currentUser.Name + " " + _currentUser.Surname;
                txtPesel.Text = _currentUser.PESEL;
                txtPhone.Text = _currentUser.PhoneNumber;
                txtLogin.Text = _currentUser.Login;
                txtPassword.Password = _currentUser.Password;
                txtRepeatPassword.Password = _currentUser.Password;

                int typeId= _currentUser.UserType;
                switch (typeId) 
                {
                    case 1:
                        txtRole.Text = "Recepcja";
                        break;
                    case 2:
                        txtRole.Text = "Pielęgniarka";
                        break;
                    case 3:
                        txtRole.Text = "Analityk";
                        break;
                    case 4:
                        txtRole.Text = "Pacjent";
                        break;
                } 
            }
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
                string newLogin = txtLogin.Text.Trim();
                string newPassword = txtPassword.Password.Trim();
                string repeatPassword = txtRepeatPassword.Password.Trim();
                string newPhone = txtPhone.Text.Trim();

                // Sprawdzenie czy hasła się zgadzają
                if (newPassword != repeatPassword)
                {
                    MessageBox.Show("Hasła się nie zgadzają.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Sprawdź, czy login nie jest zajęty przez innego użytkownika
                if (DbManager.IsLoginTakenByAnotherUser(newLogin, _currentUser.id))
                {
                    MessageBox.Show("Login jest już zajęty.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Aktualizacja profilu użytkownika
                bool updated = DbManager.EditUserCommon(newLogin, newPassword, newPhone, _currentUser.id);
                if (updated)
                {
                    MessageBox.Show("Profil został zaktualizowany!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                    _parentWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Wystąpił błąd podczas aktualizacji.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtPhone.Text) ||
                string.IsNullOrWhiteSpace(txtLogin.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                MessageBox.Show("Wszystkie pola muszą być wypełnione.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (txtPhone.Text.Length < 9)
            {
                MessageBox.Show("Numer telefonu musi zawierać co najmniej 9 cyfr.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (txtPassword.Password.Length < 6)
            {
                MessageBox.Show("Hasło musi zawierać co najmniej 6 znaków.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _parentWindow.Show();
            this.Close();
        }
    }
}