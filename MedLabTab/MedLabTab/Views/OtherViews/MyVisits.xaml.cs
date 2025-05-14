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
        private SignedInUser _currentUser;

        public MyVisits(SignedInUser currentUser, Window parentWindow)
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

            if (visits != null)
            {
                var visitsWithNames = visits.Select(v => new
                {
                    Date = DbManager.GetSchedule(v.TimeSlotId.Value)?.Date.ToString("dd.MM.yyyy"),
                    Time = DbManager.GetSchedule(v.TimeSlotId.Value)?.Time.ToString(@"HH\:mm") ?? "",
                    Tests = string.Join(", ", DbManager.GetTestsInVisit(v.id)
                        .Select(th => DbManager.GetTest(th.TestId))
                        .Where(test => test != null && !string.IsNullOrEmpty(test.TestName))
                        .Select(test => test.TestName)),
                    Cost = v.Cost + " zł",
                    Nurse = DbManager.GetUserById(DbManager.GetSchedule(v.TimeSlotId.Value).NurseId)?.Name + " " +
                        DbManager.GetUserById(DbManager.GetSchedule(v.TimeSlotId.Value).NurseId)?.Surname,
                    PaymentStatus = v.PaymentStatus,
                    v.IsActive,
                    OriginalVisit = v,
                }).ToList();

                VisitsDataGrid.ItemsSource = visitsWithNames;
            }
        }

        private void BtnEditVisit_Click(object sender, RoutedEventArgs e)
        
        {
            if (VisitsDataGrid.SelectedItem != null)
            {
                dynamic selectedItem = VisitsDataGrid.SelectedItem;
                Visit selectedVisit = selectedItem.OriginalVisit;

                EditVisitAdmin editVisitAdmin = new EditVisitAdmin(selectedVisit, _currentUser, this);
                editVisitAdmin.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Proszę wybrać wizytę do edycji.", "Brak zaznaczenia",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        
        }

        private void BtnCancelVisit_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button)) return;

            dynamic selectedItem = button.DataContext;

            if (!(selectedItem.OriginalVisit is Visit selectedVisit))
            {
                MessageBox.Show("Nie udało się pobrać wybranej wizyty.",
                              "Błąd",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show(
                $"Czy na pewno chcesz anulować wizytę z dnia {DbManager.GetSchedule(selectedVisit.TimeSlotId.Value)?.Date.ToString("dd.MM.yyyy")} o {DbManager.GetSchedule(selectedVisit.TimeSlotId.Value)?.Time.ToString(@"HH\:mm")}?",
                "Potwierdzenie anulowania",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                bool success = DbManager.DeactivateVisit(selectedVisit);

                if (success)
                {
                    MessageBox.Show("Wizyta została anulowana.",
                                  "Sukces",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);
                    LoadVisits();
                }
                else
                {
                    MessageBox.Show("Nie udało się anulować wizyty.",
                                  "Błąd",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd: {ex.Message}",
                              "Błąd",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
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


        private void BtnNewVisit_Click(object sender, RoutedEventArgs e)
        {
            NewVisit newVisit = new NewVisit(_currentUser,this);
            newVisit.Show();
            this.Hide();
        }

        private void BtnResults_Click(object sender, RoutedEventArgs e)
        {
            AllReports reports = new AllReports(_currentUser, this);
            reports.Show();
            this.Hide();
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
            NewVisit newVisit = new NewVisit(_currentUser, this);
            newVisit.Show();
            this.Hide();
        }
    }
}
