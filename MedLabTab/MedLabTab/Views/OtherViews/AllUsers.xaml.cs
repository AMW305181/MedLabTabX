using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MedLabTab.DatabaseManager;
using MedLabTab.DatabaseModels;

namespace MedLabTab.Views.OtherViews
{
    /// <summary>
    /// Logika interakcji dla klasy AllUsers.xaml
    /// </summary>
    public partial class AllUsers : Window
    {
        private User _selectedUser;
        private List<User> _allUsers;
        private List<User> _filteredUsers;
        public AllUsers()
        {
            InitializeComponent();
            LoadUsers();
            txtSearch.TextChanged += TxtSearch_TextChanged;
        }

        private void LoadUsers()
        {
            var users = DbManager.LoadUsers();
            if (users != null)
            {
                dgUsers.ItemsSource = users;
            }
            else
            {
                MessageBox.Show("Błąd podczas ładowania aktywnych testów.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            /*
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                dgUsers.ItemsSource = _allUsers;
                _filteredUsers = new List<User>(_allUsers);
            }
            else
            {
                var searchText = txtSearch.Text.ToLower();
                _filteredUsers = _allUsers.Where(u =>
                    u.Login.ToLower().Contains(searchText) ||
                    u.Name.ToLower().Contains(searchText) ||
                    u.Surname.ToLower().Contains(searchText)).ToList();

                dgUsers.ItemsSource = _filteredUsers;
            }
            */
        }

        private void BtnAllVisits_Click(object sender, RoutedEventArgs e)
        {
            AllVisits allVisits = new AllVisits();
            allVisits.Show();
            this.Close();  
        }

        private void BtnNewVisit_Click(object sender, RoutedEventArgs e)
        {
            NewVisit newVisit = new NewVisit();
            newVisit.Show();
            this.Close();
        }

        private void BtnAllExams_Click(object sender, RoutedEventArgs e)
        {
            AllTests allTests = new AllTests(this);
            allTests.Show();
            this.Close();
        }

        private void BtnNewExam_Click(object sender, RoutedEventArgs e)
        {
            NewTest newTest = new NewTest(this);
            newTest.Show();
            this.Close();
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
            this.Close();
        }

        private void BtnStats_Click(object sender, RoutedEventArgs e)
        {
            Statistics statistics = new Statistics();
            statistics.Show();
            this.Close();
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

        private void CmbRoleFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbRoleFilter.SelectedItem == null || _allUsers == null) return;

            var selectedRole = (cmbRoleFilter.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (selectedRole == "Wszystkie role")
            {
                dgUsers.ItemsSource = _allUsers;
                _filteredUsers = new List<User>(_allUsers);
            }
            else
            {
                using (var db = new MedLabContext())
                {
                    var role = db.UserTypes.FirstOrDefault(ut => ut.TypeName == selectedRole);

                    if (role != null)
                    {
                        _filteredUsers = _allUsers.Where(u => u.UserTypeNavigation?.id == role.id).ToList();
                        dgUsers.ItemsSource = _filteredUsers;
                    }
                    else
                    {
                        _filteredUsers = new List<User>(_allUsers);
                        dgUsers.ItemsSource = _filteredUsers;
                    }
                }
            }
        }

        private void BtnEditUser_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            User selectedUser = button?.DataContext as User;

            if (selectedUser != null)
            {

            }
            else
            {
                MessageBox.Show("Nie udało się wczytać danych użytkownika.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void BtnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            User selectedUser = button?.DataContext as User;

            if (selectedUser != null)
            {
                var result = MessageBox.Show(
                    $"Czy na pewno chcesz usunąć użytkownika \"{selectedUser.Name} { selectedUser.Surname}\"?",
                    "Potwierdzenie usunięcia",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    bool deleted = DbManager.ChangeUserStatus(selectedUser.id);

                    if (deleted)
                    {
                        MessageBox.Show("Użytkownik został usunięty.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadUsers();
                    }
                    else
                    {
                        MessageBox.Show("Wystąpił błąd podczas usuwania użytkownika.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Nie udało się pobrać wybranego użytkownika.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void BtnCancelUser_Click(object sender, RoutedEventArgs e)
        {
            panelEdit.Visibility = Visibility.Collapsed;
            ClearEditFields();
        }
        private void BtnSaveUser_Click(object sender, RoutedEventArgs e)
        {

        }
        private void BtnCancelEdit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DgUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedUser = dgUsers.SelectedItem as User;
        }

        private void ClearEditFields()
        {
            txtLogin.Text = string.Empty;
            txtFirstName.Text = string.Empty;
            txtLastName.Text = string.Empty;
            txtPesel.Text = string.Empty;
            txtPhone.Text = string.Empty;
            txtPassword.Password = string.Empty;
            cmbRole.SelectedIndex = -1;
            _selectedUser = null;
        }

        private void CmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
