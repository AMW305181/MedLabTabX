﻿using MedLabTab.DatabaseManager;
using MedLabTab.DatabaseModels;
using MedLabTab.ViewModels;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MedLabTab.Views.OtherViews
{
    /// <summary>
    /// Interaction logic for Samples.xaml
    /// </summary>
    public partial class SamplesAnalyst : Window
    {
        private SignedInUser _currentUser;
        public SamplesAnalyst(SignedInUser currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            LoadSamples();
        }
        public void LoadSamples()
        {
            var tests = DbManager.GetAnalystTests(_currentUser.id);

            if (tests != null)
            {
                var samples = tests.Select(t => new
                {
                    TestName = t.Test?.TestName,
                    Date = t.Visit?.TimeSlot?.Date.ToString("dd.MM.yyyy"),
                    Time = t.Visit?.TimeSlot?.Time.ToString(@"HH\:mm"),
                    Patient = $"{t.Patient.Name} {t.Patient.Surname}",
                    TestCategory = t.Test?.CategoryNavigation?.CategoryName,
                    Status = t.Status == 3? "Do analizy" : t.Status==4?"Do uzupełnienia wyniki":"Analiza zakończona",
                    OriginalTest = t
                }).ToList();

                SamplesDataGrid.ItemsSource = samples;
            }
            else
            {
                MessageBox.Show("Błąd podczas ładowania danych.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void BeingAnalysed_Click(object sender, RoutedEventArgs e)
        {
            dynamic selectedItem = SamplesDataGrid.SelectedItem;
            TestHistory oldTest = selectedItem.OriginalTest;

            if (oldTest.Status == 3)
            {
                bool updated = true;

                var newTest = new TestHistory
                {
                    id = oldTest.id, // klucz główny musi być ten sam, żeby zaktualizować
                    VisitId = oldTest.VisitId,
                    TestId = oldTest.TestId,
                    PatientId = oldTest.PatientId,
                    AnalystId = _currentUser.id,
                    Status = 4 // do raportu
                };

                updated = DbManager.EditTestHistory(oldTest, newTest);


                if (updated)
                {
                    MessageBox.Show("Status badania został zaktualizowany.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Nie udało się zaktualizować badania.", "Uwaga", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                LoadSamples(); // odśwież widok
            }
            else
            {
                MessageBox.Show("Próbka została już poddana analizie.", "Uwaga", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AddResults_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ResultInputDialog
            {
                Owner = this // ważne dla prawidłowego ustawienia modala
            };

            if (dialog.ShowDialog() == true)
            {
                string resultText = dialog.ResultText;

                dynamic selectedItem = SamplesDataGrid.SelectedItem;
                TestHistory test = selectedItem.OriginalTest;

                var now = DateTime.Now;

                var newReport = new Report
                {
                    SampleId = test.id,
                    LastUpdateDate = DateOnly.FromDateTime(now),
                    LastUpdateTime = TimeOnly.FromDateTime(now),
                    Results = resultText,
                };

                bool added = false;

                var oldReport = DbManager.GetReport(test.id); //do sprawdzenia

                if (oldReport == null)
                {
                    added = DbManager.AddReport(newReport);
                }
                else
                {
                    added = DbManager.EditReport(oldReport, newReport);
                }

                bool updated = true;

                var newTest = new TestHistory
                {
                    id = test.id,
                    VisitId = test.VisitId,
                    TestId = test.TestId,
                    PatientId = test.PatientId,
                    AnalystId = _currentUser.id,
                    Status = 5 // gotowy
                };

                updated = DbManager.EditTestHistory(test, newTest);

                if (added && updated)
                {
                    MessageBox.Show("Wyniki zostały zapisane.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Nie udało się zapisać wyników.", "Uwaga", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

            LoadSamples();
        }

        private void BtnExams_Click(object sender, RoutedEventArgs e)
        {
            AllTests allTests = new AllTests(_currentUser, this);
            allTests.Show();
            this.Hide();
        }

        private void BtnSamples_Click(object sender, RoutedEventArgs e)
        {
            SamplesAnalyst samples = new SamplesAnalyst(_currentUser);
            samples.Show();
            this.Hide();
        }

        private void BtnReport_Click(object sender, RoutedEventArgs e)
        {
            AllReports newReport = new AllReports(_currentUser, this);
            newReport.Show();
            this.Hide();
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            Profile profile = new Profile(_currentUser, this);
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
    }

}
