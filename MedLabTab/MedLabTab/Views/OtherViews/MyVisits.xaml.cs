using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MedLabTab.DatabaseModels;
using BCrypt.Net;
using MedLabTab.DatabaseManager;
using MedLabTab.ViewModels;
using System.Collections.Generic;
using System.Windows.Data;
using System.Globalization;

namespace MedLabTab.Views.OtherViews
{
    /// <summary>
    /// Interaction logic for AllVisits.xaml
    /// </summary>
    /// 

    public partial class MyVisits : Window
    {
        private Window _parentWindow;
        private User _currentUser;

        public MyVisits(User currentUser, Window parentWindow)
        {
            if (currentUser == null)
                throw new ArgumentNullException(nameof(currentUser));

            InitializeComponent();
            _currentUser = currentUser; 
            _parentWindow = parentWindow;
            LoadVisits();
        }

        public void LoadVisits()
        {
            var visits = DbManager.GetMyVisits(_currentUser.id);

            // Tymczasowe debugowanie
            foreach (var visit in visits)
            {
                Console.WriteLine($"ID: {visit.id}");
                Console.WriteLine($"TimeSlot: {visit.TimeSlot != null}");
                Console.WriteLine($"Nurse: {visit.TimeSlot?.Nurse != null}");
                Console.WriteLine($"TestHistories: {visit.TestHistories?.Count ?? 0}");
            }

            VisitsDataGrid.ItemsSource = visits;
        }


        public static List<Visit> GetMyVisits(int userId)
        {
            try
            {
                using (var db = new MedLabContext())
                {
                    return db.Visits
                            .Where(v => v.PatientId == userId)
                            .ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting visits: {ex}");
                return new List<Visit>();
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            //Button button = sender as Button;

            //// odnalezienie kontekstu wiersza - czyli obiektu testu
            //Visit selectedVisit = (sender as Button)?.CommandParameter as Visit;

            //if (selectedVisit != null)
            //{
            //    var editTestWindow = new EditVisit(selectedVisit, this);
            //    editTestWindow.Show();
            //    this.Hide();
            //}
            //else
            //{
            //    MessageBox.Show("Nie udało się wczytać danych wizyty.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }

        private void Deactivate_Click(object sender, RoutedEventArgs e)
        {
            //Button button = sender as Button;
            //Visit selectedVisit = (sender as Button)?.CommandParameter as Visit;

            //if (selectedVisit != null)
            //{
            //    var result = MessageBox.Show(
            //        $"Czy na pewno chcesz anulować wizytę \"{selectedVisit.id}\"?",
            //        "Potwierdzenie anulowania",
            //        MessageBoxButton.YesNo,
            //        MessageBoxImage.Warning);

            //    if (result == MessageBoxResult.Yes)
            //    {
            //        bool deleted = DbManager.DeactivateVisit(selectedVisit);

            //        if (deleted)
            //        {
            //            MessageBox.Show("Wizyta została anulowana.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            //            LoadVisits(); // Odświeżenie tabeli
            //        }
            //        else
            //        {
            //            MessageBox.Show("Wystąpił błąd podczas anulowania wizyty.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            //        }
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("Nie udało się pobrać wybranej wizyty.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }

        private void BtnEditVisit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnCancelVisit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnExams_Click(object sender, RoutedEventArgs e)
        {
            AllTests allTests = new AllTests(this);
            allTests.Show();
            this.Hide();
        }

        private void BtnVisits_Click(object sender, RoutedEventArgs e)
        {
            MyVisits allVisits = new MyVisits(_currentUser, this);
            allVisits.Show();
            this.Hide();
        }


        private void BtnNewVisit_Click(object sender, RoutedEventArgs e)
        {
            NewVisit newVisit = new NewVisit(this);
            newVisit.Show();
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

        private void NewVisit2_Click(object sender, RoutedEventArgs e)
        {
            NewVisit newVisit = new NewVisit(this);
            newVisit.Show();
            this.Hide();
        }
    }
}
