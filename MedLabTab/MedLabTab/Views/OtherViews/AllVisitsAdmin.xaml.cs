using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using static System.Net.Mime.MediaTypeNames;

namespace MedLabTab.Views.OtherViews
{
    public partial class AllVisitsAdmin : Window
    {
        private Window _parentWindow;
        public AllVisitsAdmin(Window parentWindow)
        {
            InitializeComponent();
            LoadVisits(); // Załaduj dane po inicjalizacji okna
            _parentWindow = parentWindow;
        }
        public void LoadVisits()
        {
            var visits = DbManager.GetAllVisits();

            if (visits != null)
            {
                var visitsWithNames = visits.Select(v => new
                {
                    Id = v.id,
                    v.Cost,
                    v.PaymentStatus,
                    v.IsActive,
                    DateTime = DbManager.GetSchedule(v.TimeSlotId.Value)?.Date + " " +
                          DbManager.GetSchedule(v.TimeSlotId.Value)?.Time,
                    Patient = DbManager.GetUserById(v.PatientId)?.Name + " " + DbManager.GetUserById(v.PatientId)?.Surname, 
                    Nurse = DbManager.GetUserById(DbManager.GetSchedule(v.TimeSlotId.Value).NurseId)?.Name + " " + 
                        DbManager.GetUserById(DbManager.GetSchedule(v.TimeSlotId.Value).NurseId)?.Surname,
                    OriginalVisit = v,
                }).ToList();

                VisitsDataGrid.ItemsSource = visitsWithNames;
            }
            else
            {
                MessageBox.Show("Błąd podczas ładowania danych.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            Visit selectedVisit = (sender as Button)?.CommandParameter as Visit;

            if (selectedVisit != null)
            {
                var editVisitWindow = new EditVisitAdmin(selectedVisit, this);
                editVisitWindow.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Nie udało się wczytać danych wizyty.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Deactivate_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Visit selectedVisit = (sender as Button)?.CommandParameter as Visit;

            if (selectedVisit != null)
            {
                var result = MessageBox.Show(
                    $"Czy na pewno chcesz anulować wizytę \"{selectedVisit.id}\"?",
                    "Potwierdzenie anulowania",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    bool deleted = DbManager.DeactivateVisit(selectedVisit);

                    if (deleted)
                    {
                        MessageBox.Show("Wizyta została anulowana.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadVisits(); // Odświeżenie tabeli
                    }
                    else
                    {
                        MessageBox.Show("Wystąpił błąd podczas anulowania wizyty.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Nie udało się pobrać wybranej wizyty.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            _parentWindow?.Show();
        }
    }
}
