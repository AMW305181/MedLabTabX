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
        private SignedInUser _currentUser;
        private User _editedUser;
        private Window _parentWindow;
        public Profile(User editedUser, Window parentWindow)
        {
            InitializeComponent();
            //_currentUser = currentUser;
            _editedUser = editedUser;
            _parentWindow = parentWindow;
            FillForm_Admin();
            txtPhone.PreviewTextInput += NumberValidationTextBox;

            switch (_editedUser.UserType)
            {
                case 1:
                    ReceptionMenu.Visibility = Visibility.Visible;
                    break;
                case 2:
                    ReceptionMenu.Visibility = Visibility.Visible;
                    break;
                case 3:
                    ReceptionMenu.Visibility = Visibility.Visible;
                    break;
                case 4:
                    ReceptionMenu.Visibility = Visibility.Visible;
                    break;
            }
        }

        public Profile(SignedInUser currentUser, Window parentWindow)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _parentWindow = parentWindow;
            FillForm();
            txtPhone.PreviewTextInput += NumberValidationTextBox;

            txtName.IsEnabled = false;
            txtPesel.IsEnabled = false;
            txtRole.IsEnabled = false;

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

        private void FillForm_Admin()
        {
            if (_editedUser != null)
            {
                txtName.Text = _editedUser.Name + " " + _editedUser.Surname;
                txtPesel.Text = _editedUser.PESEL;
                txtPhone.Text = _editedUser.PhoneNumber;
                txtLogin.Text = _editedUser.Login;


                int typeId = _editedUser.UserType;
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
                int UserId = 1;

                if (ValidateInputs())
                {
                    var nameParts = txtName.Text.Trim().Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    string newName = nameParts.Length > 0 ? nameParts[0] : "";
                    string newSurname = nameParts.Length > 1 ? nameParts[1] : "";
                    string newPesel = txtPesel.Text.Trim();
                    string newLogin = txtLogin.Text.Trim();
                    string newPassword = txtPassword.Password.Trim();
                    string repeatPassword = txtRepeatPassword.Password.Trim();
                    string newPhone = txtPhone.Text.Trim();
                int newRole = 1;
                switch (txtRole.Text.Trim())
                {
                    case "Recepcja":
                        newRole = 1;
                        break;
                    case "Pielęgniarka":
                        newRole = 2;
                        break;
                    case "Analityk":
                        newRole = 3;
                        break;
                    case "Pacjent":
                        newRole = 4;
                        break;
                }

                        if (_currentUser != null)
                    {
                        UserId = _currentUser.id;
                    }
                    else if (_editedUser != null)
                    {
                        UserId = _editedUser.id;
                    }

                    // Sprawdzenie czy hasła się zgadzają
                    if (newPassword != repeatPassword)
                    {
                        MessageBox.Show("Hasła się nie zgadzają.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Sprawdź, czy login nie jest zajęty przez innego użytkownika
                    if (DbManager.IsLoginTakenByAnotherUser(newLogin, UserId))
                    {
                        MessageBox.Show("Login jest już zajęty.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                bool updated;
                if (_editedUser != null) // Tryb admina
                {
                    var oldUser = DbManager.GetUserById(UserId);
                    if (oldUser == null)
                    {
                        MessageBox.Show("Nie znaleziono użytkownika do aktualizacji.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (string.IsNullOrEmpty(newPassword))
                    {
                        newPassword = oldUser.Password;
                    }

                    var newUser = new User
                    {
                        id = UserId,
                        Name = newName,
                        Surname = newSurname,
                        PESEL = newPesel,
                        PhoneNumber = newPhone,
                        Login = newLogin,
                        Password = newPassword,
                        UserType = newRole,
                    };

                    updated = DbManager.EditUserAdmin(oldUser, newUser);
                }
                else
                {
                    updated = DbManager.EditUserCommon(newLogin, newPassword, newPhone, UserId);
                }
                if (updated)
                {
                    MessageBox.Show("Profil został zaktualizowany!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);

                    if (_currentUser != null)
                    {
                        _currentUser.Name = newName;
                        _currentUser.Surname = newSurname;
                        _currentUser.PESEL = newPesel;
                        _currentUser.PhoneNumber = newPhone;
                        _currentUser.Login = newLogin;
                        _currentUser.UserType = newRole;
                        if (!string.IsNullOrEmpty(txtPassword.Password))
                            _currentUser.Password = txtPassword.Password.Trim();
                    }
                    if (_editedUser!=null)
                    {
                        AllUsers allUsers = new AllUsers(_currentUser);
                        allUsers.LoadUsers();
                        allUsers.Show();
                        this.Close();
                    }
                    else
                    {
                        _parentWindow?.Show();
                        this.Close();
                    }
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

            if (txtPhone.Text.Length != 9)
            {
                MessageBox.Show("Numer telefonu musi zawierać 9 cyfr.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
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
            AllVisitsAdmin allVisits = new AllVisitsAdmin(_currentUser);
            allVisits.Show();
            this.Hide();
        }

        private void BtnNewVisit_Click(object sender, RoutedEventArgs e)
        {
            NewVisit newVisit = new NewVisit(_currentUser, this);
            newVisit.Show();
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
        private void BtnExams_Click(object sender, RoutedEventArgs e)
        {
            AllTests allTests = new AllTests(_currentUser, this);
            allTests.Show();
            this.Hide();
        }

        private void BtnSamplesNurse_Click(object sender, RoutedEventArgs e)
        {
            SamplesNurse samples = new SamplesNurse(_currentUser);
            samples.Show();
            this.Hide();
        }

        private void BtnVisits_Click(object sender, RoutedEventArgs e)
        {
            MyVisits allVisits = new MyVisits(_currentUser, this);
            allVisits.Show();
            this.Hide();
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            var profile = new Profile(_currentUser, this);
            profile.Show();
            this.Hide();
        }

        private void BtnReport_Click(object sender, RoutedEventArgs e)
        {
            AllReports newReport = new AllReports(_currentUser, this);
            newReport.Show();
            this.Hide();
        }

        private void BtnSamplesAnalyst_Click(object sender, RoutedEventArgs e)
        {
            SamplesAnalyst samples = new SamplesAnalyst(_currentUser);
            samples.Show();
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