using MedLabTab.DatabaseManager;
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
using MedLabTab.ViewModels;
using LiveCharts;
using LiveCharts.Wpf;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Diagnostics;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.Win32;
using System.Xml.Linq;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace MedLabTab.Views.OtherViews
{
    /// <summary>
    /// Interaction logic for Statistics.xaml
    /// </summary>
    public partial class Statistics : Window
    {
        private SignedInUser _currentUser;
        private List<TestHistory> _allTests;
        private List<Test> _availableTests;
        private List<TestHistory> tests;

        public Statistics(SignedInUser currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;

            _allTests = DbManager.GetCompletedTests();
            _availableTests = DbManager.GetAllTests()?.ToList() ?? new List<Test>();

            var allTestsOption = new Test { id = 0, TestName = "Wszystkie testy" };
            _availableTests.Insert(0, allTestsOption);

            TestNameComboBox.ItemsSource = _availableTests;
            TestNameComboBox.DisplayMemberPath = "TestName";
            TestNameComboBox.SelectedValuePath = "id";
            TestNameComboBox.SelectedIndex = 0;

            DateFrom.SelectedDate = DateTime.Today.AddMonths(-1);
            DateTo.SelectedDate = DateTime.Today;
            UpdateData();
        }
        private void FillData_OfAllTimes()
        {
            var tests = DbManager.GetCompletedTests();

            if (tests == null || !tests.Any())
            {
                Nurse.Text = "";
                Analyst.Text = "";
                Patient.Text = "";
                return;
            }

            float income = tests.Sum(t => DbManager.GetTest(t.TestId)?.Price ?? 0);
            Income.Text = income.ToString();
            Income.Text = string.Format("{0:N2} zł", income);

            var mostCommonTest = tests
                .GroupBy(t => t.TestId)
                .Select(g => new { TestId = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .FirstOrDefault();

            var testInfo = mostCommonTest != null ? DbManager.GetTest(mostCommonTest.TestId) : null;
            Test.Text = testInfo?.TestName ?? "Brak danych";

            var mostCommonNurse = tests
                .GroupBy(t => DbManager.GetNurseIdFromTestHistory(t))
                .Select(g => new { NurseId = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .FirstOrDefault();

            var nurseInfo = mostCommonNurse != null ? DbManager.GetUserById(mostCommonNurse.NurseId) : null;
            Nurse.Text = nurseInfo != null ? $"{nurseInfo.Name} {nurseInfo.Surname}" : "Brak danych";

            var mostCommonAnalyst = tests
                .GroupBy(t => t.AnalystId)
                .Select(g => new { AnalystId = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .FirstOrDefault();

            var analystInfo = mostCommonAnalyst?.AnalystId != null ? DbManager.GetUserById(mostCommonAnalyst.AnalystId.Value) : null;
            Analyst.Text = analystInfo != null ? $"{analystInfo.Name} {analystInfo.Surname}" : "Brak danych";

            var mostCommonPatient = tests
                .GroupBy(t => t.PatientId)
                .Select(g => new { PatientId = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .FirstOrDefault();

            var patientInfo = mostCommonPatient != null ? DbManager.GetUserById(mostCommonPatient.PatientId) : null;
            Patient.Text = patientInfo != null ? $"{patientInfo.Name} {patientInfo.Surname}" : "Brak danych";
        }

        private void UpdateData()
        {
            var filteredTests = ApplyFilters(_allTests);
            DisplayStatistics(filteredTests);
            UpdateChart(filteredTests);
        }

        private List<TestHistory> ApplyFilters(List<TestHistory> tests)
        {
            var filtered = tests.AsEnumerable();

            if (DateFrom.SelectedDate != null)
            {
                filtered = filtered.Where(t => t.Visit?.TimeSlot?.Date >= DateOnly.FromDateTime(DateFrom.SelectedDate.Value));
            }

            if (DateTo.SelectedDate != null)
            {
                filtered = filtered.Where(t => t.Visit?.TimeSlot?.Date <= DateOnly.FromDateTime(DateTo.SelectedDate.Value));
            }

            if (TestNameComboBox.SelectedValue != null)
            {
                int selectedTestId = Convert.ToInt32(TestNameComboBox.SelectedValue);
                if (selectedTestId != 0) 
                {
                    filtered = filtered.Where(t => t.TestId == selectedTestId);
                }
            }

            return filtered.ToList();
        }

        private void DisplayStatistics(List<TestHistory> tests)
        {
            if (!tests.Any())
            {
                Income.Text = "0 zł";
                Test.Text = "Brak danych";
                Nurse.Text = "Brak danych";
                Analyst.Text = "Brak danych";
                Patient.Text = "Brak danych";
                return;
            }

            int? selectedTestId = TestNameComboBox.SelectedValue as int?;
            if (selectedTestId == 0) selectedTestId = null; 

            var filteredTests = selectedTestId.HasValue
                ? tests.Where(t => t.TestId == selectedTestId.Value).ToList()
                : tests;

            float income = filteredTests.Sum(t => _availableTests.FirstOrDefault(test => test.id == t.TestId)?.Price ?? 0);
            Income.Text = $"{income:N2} zł";

            if (selectedTestId.HasValue)
            {
                var testInfo = _availableTests.FirstOrDefault(t => t.id == selectedTestId.Value);
                Test.Text = testInfo != null
                    ? $"{testInfo.TestName} ({filteredTests.Count}x)"
                    : "Brak danych";
            }
            else
            {
                var mostCommonTest = tests
                    .GroupBy(t => t.TestId)
                    .Select(g => new { TestId = g.Key, Count = g.Count() })
                    .OrderByDescending(g => g.Count)
                    .FirstOrDefault();

                Test.Text = mostCommonTest != null
                    ? $"{_availableTests.FirstOrDefault(t => t.id == mostCommonTest.TestId)?.TestName} ({mostCommonTest.Count}x)"
                    : "Brak danych";
            }

            var mostCommonNurse = filteredTests
                .GroupBy(t => DbManager.GetNurseIdFromTestHistory(t))
                .Select(g => new { NurseId = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .FirstOrDefault();

            var nurseInfo = mostCommonNurse != null ? DbManager.GetUserById(mostCommonNurse.NurseId) : null;
            Nurse.Text = nurseInfo != null
                ? $"{nurseInfo.Name} {nurseInfo.Surname} ({mostCommonNurse.Count}x)"
                : "Brak danych";

            var mostCommonAnalyst = filteredTests
                .GroupBy(t => t.AnalystId)
                .Select(g => new { AnalystId = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .FirstOrDefault();

            var analystInfo = mostCommonAnalyst?.AnalystId != null ? DbManager.GetUserById(mostCommonAnalyst.AnalystId.Value) : null;
            Analyst.Text = analystInfo != null
                ? $"{analystInfo.Name} {analystInfo.Surname} ({mostCommonAnalyst.Count}x)"
                : "Brak danych";

            var mostCommonPatient = filteredTests
                .GroupBy(t => t.PatientId)
                .Select(g => new { PatientId = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .FirstOrDefault();

            var patientInfo = mostCommonPatient != null ? DbManager.GetUserById(mostCommonPatient.PatientId) : null;
            Patient.Text = patientInfo != null
                ? $"{patientInfo.Name} {patientInfo.Surname} ({mostCommonPatient.Count}x)"
                : "Brak danych";
        }

        private void UpdateChart(List<TestHistory> tests)
        {
            var selectedTestId = TestNameComboBox.SelectedValue as int?;
            if (selectedTestId == 0) selectedTestId = null;

            var monthlyData = tests
                .Where(t => t.Visit?.TimeSlot != null && (!selectedTestId.HasValue || t.TestId == selectedTestId.Value))
                .GroupBy(t => new { t.Visit.TimeSlot.Date.Year, t.Visit.TimeSlot.Date.Month })
                .Select(g => new
                {
                    Miesiąc = new DateOnly(g.Key.Year, g.Key.Month, 1),
                    Liczba_badań = g.Count(),
                    Przychód = g.Sum(t => _availableTests.FirstOrDefault(test => test.id == t.TestId)?.Price ?? 0)
                })
                .OrderBy(x => x.Miesiąc)
                .ToList();

            ChartDataGrid.AutoGenerateColumns = false;
            ChartDataGrid.ItemsSource = monthlyData;

            ChartDataGrid.Columns.Clear();
            ChartDataGrid.Columns.Add(new DataGridTextColumn()
            {
                Header = "Miesiąc",
                Binding = new Binding("Miesiąc") { StringFormat = "dd.MM.yyyy" }
            });
            ChartDataGrid.Columns.Add(new DataGridTextColumn()
            {
                Header = "Liczba badań",
                Binding = new Binding("Liczba_badań")
            });
            ChartDataGrid.Columns.Add(new DataGridTextColumn()
            {
                Header = "Przychód (zł)",
                Binding = new Binding("Przychód") { StringFormat = "N2" }
            });
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateData();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            DateFrom.SelectedDate = DateTime.Today.AddMonths(-1);
            DateTo.SelectedDate = DateTime.Today;
            TestNameComboBox.SelectedIndex = 0;
            UpdateData();
        }

        private void BtnExportPdf_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var document = new PdfDocument();
                var page = document.AddPage();

                using (var gfx = XGraphics.FromPdfPage(page))
                {
                    var fontTitle = new XFont("Arial", 16);
                    var fontHeader = new XFont("Arial", 12);
                    var fontNormal = new XFont("Arial", 10);
                    var fontTableHeader = new XFont("Arial", 10);
                    var fontTableContent = new XFont("Arial", 9);

                    int leftMargin = 40;
                    int rightMargin = 40;
                    int topMargin = 40;
                    int yPos = topMargin;

                    gfx.DrawString("Raport statystyk MedLabTab", fontTitle,
                        XBrushes.Black, new XRect(0, yPos, page.Width, 30),
                        XStringFormats.TopCenter);
                    yPos += 40;

                    string periodInfo = $"Okres: {DateFrom.SelectedDate?.ToString("dd.MM.yyyy") ?? "brak"} - {DateTo.SelectedDate?.ToString("dd.MM.yyyy") ?? "brak"}";
                    gfx.DrawString(periodInfo, fontHeader, XBrushes.Black, leftMargin, yPos);
                    yPos += 25;

                    gfx.DrawString($"Data generacji: {DateTime.Now:dd.MM.yyyy}", fontNormal, XBrushes.Black, leftMargin, yPos);
                    yPos += 30;

                    gfx.DrawString("Podsumowanie:", fontHeader, XBrushes.Black, leftMargin, yPos);
                    yPos += 20;

                    gfx.DrawString($"• Przychód: {Income.Text}", fontNormal, XBrushes.Black, leftMargin, yPos);
                    yPos += 15;

                    gfx.DrawString($"• Najpopularniejsze badanie: {Test.Text}", fontNormal, XBrushes.Black, leftMargin, yPos);
                    yPos += 15;

                    gfx.DrawString($"• Najlepsza pielęgniarka: {Nurse.Text}", fontNormal, XBrushes.Black, leftMargin, yPos);
                    yPos += 15;

                    gfx.DrawString($"• Najlepszy analityk: {Analyst.Text}", fontNormal, XBrushes.Black, leftMargin, yPos);
                    yPos += 15;

                    gfx.DrawString($"• Najlepszy pacjent: {Patient.Text}", fontNormal, XBrushes.Black, leftMargin, yPos);
                    yPos += 25;

                    var tableData = (ChartDataGrid.ItemsSource as IEnumerable<dynamic>)?.ToList();
                    if (tableData != null && tableData.Any())
                    {
                        gfx.DrawString("Statystyki miesięczne:", fontHeader, XBrushes.Black, leftMargin, yPos);
                        yPos += 20;

                        int[] columnWidths = { 100, 80, 100 };
                        string[] headers = { "Miesiąc", "Liczba badań", "Przychód (zł)" };

                        int xPos = leftMargin;
                        for (int i = 0; i < headers.Length; i++)
                        {
                            gfx.DrawRectangle(XBrushes.LightGray, xPos, yPos, columnWidths[i], 20);
                            gfx.DrawString(headers[i], fontTableHeader, XBrushes.Black,
                                new XRect(xPos, yPos, columnWidths[i], 20),
                                XStringFormats.Center);
                            xPos += columnWidths[i];
                        }
                        yPos += 20;

                        foreach (var item in tableData)
                        {
                            xPos = leftMargin;
                            gfx.DrawRectangle(XBrushes.White, xPos, yPos, columnWidths.Sum(), 20);

                            gfx.DrawString(item.Miesiąc.ToString(), fontTableContent, XBrushes.Black,
                                new XRect(xPos, yPos, columnWidths[0], 20),
                                XStringFormats.CenterLeft);
                            xPos += columnWidths[0];

                            gfx.DrawString(item.Liczba_badań.ToString(), fontTableContent, XBrushes.Black,
                                new XRect(xPos, yPos, columnWidths[1], 20),
                                XStringFormats.Center);
                            xPos += columnWidths[1];

                            gfx.DrawString(((decimal)item.Przychód).ToString("N2"), fontTableContent, XBrushes.Black,
                                new XRect(xPos, yPos, columnWidths[2], 20),
                                XStringFormats.CenterRight);

                            yPos += 20;
                        }
                        yPos += 15;
                    }
                    else
                    {
                        gfx.DrawString("Brak danych do wyświetlenia", fontNormal, XBrushes.Black, leftMargin, yPos);
                        yPos += 20;
                    }
                }

                var saveDialog = new SaveFileDialog
                {
                    FileName = $"Raport_statystyk_{DateTime.Now:yyyyMMdd_HHmm}",
                    DefaultExt = ".pdf",
                    Filter = "Pliki PDF (*.pdf)|*.pdf"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    document.Save(saveDialog.FileName);
                    MessageBox.Show("Raport PDF został pomyślnie wygenerowany.", "Sukces",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    Process.Start(new ProcessStartInfo(saveDialog.FileName) { UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas generowania raportu PDF:\n{ex.Message}",
                    "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OfAllTimes_Click(object sender, RoutedEventArgs e)
        {
            FillData_OfAllTimes();
        }

        private void BtnAllVisits_Click(object sender, RoutedEventArgs e)
        {
            AllVisitsAdmin allVisits = new AllVisitsAdmin(_currentUser);
            allVisits.Show();
            this.Hide();
        }

        private void BtnNewVisit_Click(object sender, RoutedEventArgs e)
        {
            NewVisitAdmin newVisit = new NewVisitAdmin(_currentUser,this);
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
