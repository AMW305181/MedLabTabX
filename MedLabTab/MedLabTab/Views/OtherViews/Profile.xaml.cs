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
        private User _currentUser;
        private Window _parentWindow;
        public Profile(User currentUser, Window parentWindow)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _parentWindow = parentWindow;
            FillForm();
            txtPhone.PreviewTextInput += NumberValidationTextBox;

            switch (_currentUser.UserType)
            {
                case 1:
                    ReceptionMenu.Visibility = Visibility.Visible;
                    break;
                case 2: 
                    NurseMenu.Visibility = Visibility.Visible;
                    break;
                case 3: 
                    AnalystMenu.Visibility = Visibility.Visible;
                    break;
                case 4: 
                    PatientMenu.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void FillForm()
        {
            if (_currentUser != null)
            {
                txtName.Text = _currentUser.Name + " " + _currentUser.Surname;
                txtPesel.Text = _currentUser.PESEL;
                txtPhone.Text = _currentUser.PhoneNumber;
                txtLogin.Text = _currentUser.Login;
                //zakomentowane, poniewaz hasla hashowane wygladaja inaczej
                //txtPassword.Password = _currentUser.Password;
                //txtRepeatPassword.Password = _currentUser.Password;

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
                string.IsNullOrWhiteSpace(txtLogin.Text))
                
            {
                MessageBox.Show("Wszystkie pola muszą być wypełnione.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (txtPhone.Text.Length < 9)
            {
                MessageBox.Show("Numer telefonu musi zawierać co najmniej 9 cyfr.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtPassword.Password) && txtPassword.Password.Length < 6)
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

        private void BtnAllVisits_Click(object sender, RoutedEventArgs e)
        {
            AllVisitsAdmin allVisits = new AllVisitsAdmin(this);
            allVisits.Show();
            this.Hide();
        }

        private void BtnNewVisit_Click(object sender, RoutedEventArgs e)
        {
            NewVisit newVisit = new NewVisit(_currentUser, this);
            newVisit.Show();
            this.Hide();
        }

        private void BtnAllExams_Click(object sender, RoutedEventArgs e)
        {
            AllTestsAdmin allTests = new AllTestsAdmin(this);
            allTests.Show();
            this.Hide();
        }

        private void BtnNewExam_Click(object sender, RoutedEventArgs e)
        {
            NewTest newTest = new NewTest(this);
            newTest.Show();
            this.Hide();
        }

        private void BtnAllUsers_Click(object sender, RoutedEventArgs e)
        {
            AllUsers allUsers = new AllUsers();
            allUsers.Show();
            this.Close();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            Registration registration = new Registration();
            registration.Show();
            this.Close();
        }

        private void BtnReports_Click(object sender, RoutedEventArgs e)
        {
            AllReports allReports = new AllReports(this);
            allReports.Show();
            this.Hide();
        }

        private void BtnStats_Click(object sender, RoutedEventArgs e)
        {
            Statistics statistics = new Statistics(this);
            statistics.Show();
            this.Hide();
        }
        private void BtnExams_Click(object sender, RoutedEventArgs e)
        {
            AllTests allTests = new AllTests(_currentUser, this);
            allTests.Show();
            this.Hide();
        }

        private void BtnVisits_Click(object sender, RoutedEventArgs e)
        {
            MyVisits allVisits = new MyVisits(_currentUser, this);
            allVisits.Show();
            this.Hide();
        }



        private void BtnResults_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            var profile = new Profile(_currentUser, this);
            profile.Show();
            this.Hide();
        }

        private void BtnSamples_Click(object sender, RoutedEventArgs e)
        {
            Samples samples = new Samples(this, _currentUser);
            samples.Show();
            this.Close();
        }

        private void BtnReport_Click(object sender, RoutedEventArgs e)
        {
            AllReports newReport = new AllReports(this);
            newReport.Show();
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