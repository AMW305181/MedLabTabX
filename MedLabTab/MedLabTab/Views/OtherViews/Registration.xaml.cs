using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MedLabTab.DatabaseModels;
using BCrypt.Net;
using System.Text.RegularExpressions;
using MedLabTab.Views.MainViews;
using MedLabTab.DatabaseManager;
using MedLabTab.ViewModels;

namespace MedLabTab.Views.OtherViews
{
    public partial class Registration : Window
    {
        private User _currentUser;
        public Registration(User currentUser)
        {
            InitializeComponent();
            InitializeUserRoles();
            _currentUser = currentUser;
            ClearForm();

            txtPesel.PreviewTextInput += NumberValidationTextBox;
            txtPhone.PreviewTextInput += NumberValidationTextBox;
        }

        private void BtnAllVisits_Click(object sender, RoutedEventArgs e)
        {
            AllVisitsAdmin allVisits = new AllVisitsAdmin(_currentUser);
            allVisits.Show();
            this.Hide();
        }

        private void BtnNewVisit_Click(object sender, RoutedEventArgs e)
        {
            NewVisitAdmin newVisit = new NewVisitAdmin(_currentUser, this);
            newVisit.Show();
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
            this.Close();
        }

        private void BtnNewExam_Click(object sender, RoutedEventArgs e)
        {
            NewTest newTest = new NewTest(_currentUser, this);
            newTest.Show();
            this.Close();
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

        private void InitializeUserRoles()
        {
            cmbUserRole.Items.Clear();
            cmbUserRole.Items.Add(new ComboBoxItem { Content = "Pacjent", Tag = 4 });
            cmbUserRole.Items.Add(new ComboBoxItem { Content = "Recepcja", Tag = 1 });
            cmbUserRole.Items.Add(new ComboBoxItem { Content = "Pielęgniarka", Tag = 2 });
            cmbUserRole.Items.Add(new ComboBoxItem { Content = "Analityk", Tag = 3 });
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
                    Password = txtPassword.Text,
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
                if (userAdded) { MessageBox.Show("Rejestracja zakończona pomyślnie!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                else { MessageBox.Show("Wystąpił błąd.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning); }

                var signedInUser = new SignedInUser
                {
                    id = _currentUser.id,
                    Login = _currentUser.Login,
                    UserType = _currentUser.UserType,
                    Password = _currentUser.Password,
                    IsActive = _currentUser.IsActive,
                    Name = _currentUser.Name,
                    Surname = _currentUser.Surname,
                    PESEL = _currentUser.PESEL,
                    PhoneNumber = _currentUser.PhoneNumber,
                };
                MainViewReception newMain = new MainViewReception(signedInUser);
                newMain.Show();
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
            MessageBox.Show("Czy na pewno chcesz wyjść bez zapisanych zmian.?", "Wyjście bez zapisu", MessageBoxButton.OK, MessageBoxImage.Warning);

            var signedInUser = new SignedInUser
            {
                id = _currentUser.id,
                Login = _currentUser.Login,
                UserType = _currentUser.UserType,
                Password = _currentUser.Password,
                IsActive = _currentUser.IsActive,
                Name = _currentUser.Name,
                Surname = _currentUser.Surname,
                PESEL = _currentUser.PESEL,
                PhoneNumber = _currentUser.PhoneNumber,
            };
            MainViewReception reception = new MainViewReception(signedInUser);
            reception.Show();
            this.Close();
        }
    }
}