using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            _allUsers = DbManager.LoadUsers();
            if (_allUsers != null)
            {
                _filteredUsers = new List<User>(_allUsers);
                dgUsers.ItemsSource = _filteredUsers;
            }
            else
            {
                MessageBox.Show("Błąd podczas ładowania użytkowników.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_allUsers == null || _filteredUsers == null) return;

            var searchText = txtSearch.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                dgUsers.ItemsSource = _filteredUsers;
                return;
            }

            var searchParts = searchText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            dgUsers.ItemsSource = _filteredUsers.Where(u =>
            {
                string name = u.Name?.ToLower() ?? string.Empty;
                string surname = u.Surname?.ToLower() ?? string.Empty;
                string login = u.Login?.ToLower() ?? string.Empty;

                bool matchesSingleField =
                    login.Contains(searchText) ||
                    name.Contains(searchText) ||
                    surname.Contains(searchText);

                bool matchesFullName = false;

                if (searchParts.Length > 1)
                {
                    matchesFullName =
                        (name.Contains(searchParts[0]) && surname.Contains(searchParts[1])) ||
                        (name.Contains(searchParts[1]) && surname.Contains(searchParts[0]));
                }

                return matchesSingleField || matchesFullName;
            }).ToList();
        }

private void BtnAllVisits_Click(object sender, RoutedEventArgs e)
        {
            AllVisitsAdmin allVisits = new AllVisitsAdmin(this);
            allVisits.Show();
            this.Hide();  
        }

        private void BtnNewVisit_Click(object sender, RoutedEventArgs e)
        {
            NewVisit newVisit = new NewVisit(this);
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

            var selectedDisplayRole = (cmbRoleFilter.SelectedItem as ComboBoxItem)?.Content?.ToString();

            if (selectedDisplayRole == "Wszystkie role")
            {
                dgUsers.ItemsSource = new List<User>(_allUsers);
                return;
            }

            if (_roleMapping.TryGetValue(selectedDisplayRole, out var dbRoleValue))
            {
                using (var db = new MedLabContext())
                {
                    var role = db.UserTypes.FirstOrDefault(ut => ut.TypeName == dbRoleValue);

                    if (role != null)
                    {
                        dgUsers.ItemsSource = _allUsers
                            .Where(u => u.UserTypeNavigation?.id == role.id)
                            .ToList();
                        return;
                    }
                }
            }

            dgUsers.ItemsSource = new List<User>(_allUsers);
        }

        private readonly Dictionary<string, string> _roleMapping = new Dictionary<string, string>
    {
        {"Analityk", "analyst"},
        {"Pielęgniarka", "nurse"},
        {"Recepcja", "receptionist"},
        {"Pacjent", "patient"}
        };

        private void BtnEditUser_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            User selectedUser = button?.DataContext as User;

            if (selectedUser != null)
            {
                Profile profile = new Profile(selectedUser, this);
                profile.Show();
                this.Hide();
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
                if (selectedUser.IsActive == false)
                {
                    var result2 = MessageBox.Show(
                        $"Czy na pewno chcesz przywrócić użytkownika \"{selectedUser.Name} {selectedUser.Surname}\"?",
                        "Potwierdzenie przywrócenia",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result2 == MessageBoxResult.Yes)
                    {
                        bool deleted = DbManager.ChangeUserStatus(selectedUser.id);

                        if (deleted)
                        {
                            MessageBox.Show("Użytkownik został przywrócony.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadUsers();
                        }
                        else
                        {
                            MessageBox.Show("Wystąpił błąd podczas przywracania użytkownika.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    var result = MessageBox.Show(
                        $"Czy na pewno chcesz usunąć użytkownika \"{selectedUser.Name} {selectedUser.Surname}\"?",
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
            }
            else
            {
                MessageBox.Show("Nie udało się pobrać wybranego użytkownika.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedUser = dgUsers.SelectedItem as User;
        }
    }
}
