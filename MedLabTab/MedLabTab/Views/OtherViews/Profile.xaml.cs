using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MedLabTab.DatabaseModels;
using BCrypt.Net;
using System.Text.RegularExpressions;

namespace MedLabTab.Views.OtherViews
{
    public partial class Profile : Window
    {
        private User _currentUser; //tu powinien być zalogowany użytkownik

        public Profile(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            FillForm();
            txtPhone.PreviewTextInput += NumberValidationTextBox;
        }

        private void FillForm()
        {
            if (_currentUser != null)
            {
                txtPhone.Text = _currentUser.PhoneNumber;
                txtLogin.Text = _currentUser.Login;
                txtPassword.Text = _currentUser.Password;
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
                try
                {
                    using (var db = new MedLabContext())
                    {
                        var user = db.Users.FirstOrDefault(u => u.id == _currentUser.id);
                        if (user != null)
                        {
                            // Sprawdź, czy login się nie powiela
                            if (db.Users.Any(u => u.Login == txtLogin.Text.Trim() && u.id != user.id))
                            {
                                MessageBox.Show("Login jest już zajęty.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }

                            user.PhoneNumber = txtPhone.Text.Trim();
                            user.Login = txtLogin.Text.Trim();
                            user.Password = BCrypt.Net.BCrypt.HashPassword(txtPassword.Text);

                            db.SaveChanges();

                            MessageBox.Show("Profil został zaktualizowany!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Nie znaleziono użytkownika.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Wystąpił błąd: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtPhone.Text) ||
                string.IsNullOrWhiteSpace(txtLogin.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Wszystkie pola muszą być wypełnione.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
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