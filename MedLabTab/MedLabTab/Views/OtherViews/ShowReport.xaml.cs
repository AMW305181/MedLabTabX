using MedLabTab.DatabaseManager;
using MedLabTab.DatabaseModels;
using MedLabTab.ViewModels;
using Microsoft.Win32;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
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

namespace MedLabTab.Views.OtherViews
{
    /// <summary>
    /// Interaction logic for Report.xaml
    /// </summary>
    public partial class ShowReport : Window
    {
        private Window _parentWindow;
        private SignedInUser _currentUser;
        private readonly TestHistory _testHistory;
        public ShowReport(TestHistory testHistory, SignedInUser currentUser, Window parentWindow)
        {
            InitializeComponent();
            _parentWindow = parentWindow;
            _currentUser = currentUser;
            _testHistory = testHistory;
            FillReportWithData();

            switch (_currentUser.UserType)
            {
                case 1:
                    ReceptionMenu.Visibility = Visibility.Visible;
                    BtnPdf.Visibility = Visibility.Visible;
                    break;
                case 3:
                    AnalystMenu.Visibility = Visibility.Visible;
                    break;
                case 4:
                    PatientMenu.Visibility = Visibility.Visible;
                    BtnPdf.Visibility = Visibility.Visible;
                    LoadPatientResults(_currentUser.id);
                    break;
            }

        }
        private void FillReportWithData()
        {
            TestTextBlock.Text = _testHistory.Test?.TestName;
            PatientTextBlock.Text = _testHistory.Patient?.Name + " " + _testHistory.Patient?.Surname;
            NurseTextBlock.Text = _testHistory.Visit?.TimeSlot?.Nurse?.Name + " " + _testHistory.Visit?.TimeSlot?.Nurse?.Surname;
            AnalystTextBlock.Text = _testHistory.Analyst != null ? $"{_testHistory.Analyst.Name} {_testHistory.Analyst.Surname}" : "Brak analityka";
            DateTextBlock.Text = $"{_testHistory.Visit?.TimeSlot?.Date.ToString("dd.MM.yyyy") ?? "Brak daty"} {_testHistory.Visit?.TimeSlot?.Time.ToString(@"HH\:mm") ?? "Brak godziny"}";
            ResultTextBox.Text = DbManager.GetReport(_testHistory.id).Results;

        }

        private void LoadPatientResults(int patientId)
        {
            try
            {
                var results = DbManager.GetPatientResults(patientId);

                if (results != null && results.Any())
                {
                    var patientResults = results.Select(r => new
                    {
                        TestName = r.Test?.TestName,
                        Date = r.Visit?.TimeSlot?.Date,
                        Time = r.Visit?.TimeSlot?.Time,
                        Category = r.Test?.CategoryNavigation?.CategoryName,
                        Result = r.Reports.FirstOrDefault()?.Results,
                        Status = r.StatusNavigation?.StatusName,
                        Analyst = r.Analyst != null ? $"{r.Analyst.Name} {r.Analyst.Surname}" : "Nie przypisano",
                        OriginalTest = r
                    }).ToList();

                    //ResultsDataGrid.ItemsSource = patientResults;
                }
                else
                {
                    MessageBox.Show("Brak wyników dla wybranego pacjenta.", "Informacja",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas ładowania wyników: {ex.Message}", "Błąd",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void ExportToPdf_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string patientName = $"{_testHistory.Patient?.Name}_{_testHistory.Patient?.Surname}_{DateTime.Now:yyyyMMdd_HHmm}".Replace(" ", "");
                string fileName = $"Wyniki_{patientName}.pdf";

                var saveDialog = new SaveFileDialog
                {
                    FileName = fileName,
                    DefaultExt = ".pdf",
                    Filter = "Pliki PDF (*.pdf)|*.pdf",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                };

                if (saveDialog.ShowDialog() == true)
                {
                    var document = new PdfDocument();
                    var page = document.AddPage();

                    using (var gfx = XGraphics.FromPdfPage(page))
                    {
                        var fontTitle = new XFont("Arial", 16);
                        var fontHeader = new XFont("Arial", 12);
                        var fontNormal = new XFont("Arial", 10);
                        var fontResults = new XFont("Arial", 10);

                        int leftMargin = 40;
                        int topMargin = 40;
                        int yPos = topMargin;

                        gfx.DrawString("Raport z badania laboratoryjnego", fontTitle,
                            XBrushes.Black, new XRect(0, yPos, page.Width, 30),
                            XStringFormats.TopCenter);
                        yPos += 40;

                        gfx.DrawString("Podstawowe informacje:", fontHeader, XBrushes.Black, leftMargin, yPos);
                        yPos += 20;

                        gfx.DrawString($"• Badanie: {TestTextBlock.Text}", fontNormal, XBrushes.Black, leftMargin, yPos);
                        yPos += 15;

                        gfx.DrawString($"• Pacjent: {PatientTextBlock.Text}", fontNormal, XBrushes.Black, leftMargin, yPos);
                        yPos += 15;

                        gfx.DrawString($"• Data wykonania: {DateTextBlock.Text}", fontNormal, XBrushes.Black, leftMargin, yPos);
                        yPos += 15;

                        gfx.DrawString($"• Pielęgniarka: {NurseTextBlock.Text}", fontNormal, XBrushes.Black, leftMargin, yPos);
                        yPos += 15;

                        gfx.DrawString($"• Analityk: {AnalystTextBlock.Text}", fontNormal, XBrushes.Black, leftMargin, yPos);
                        yPos += 25;

                        gfx.DrawString("Wyniki badania:", fontHeader, XBrushes.Black, leftMargin, yPos);
                        yPos += 20;

                        var resultsText = ResultTextBox.Text;
                        var formattedResults = FormatResultsForPdf(resultsText);

                        foreach (var line in formattedResults)
                        {

                            gfx.DrawString(line, fontResults, XBrushes.Black, leftMargin, yPos);
                            yPos += 15;
                        }

                        yPos += 20;
                        gfx.DrawString($"Wygenerowano: {DateTime.Now:dd.MM.yyyy HH:mm}", fontNormal,
                            XBrushes.Black, new XRect(0, yPos, page.Width, 20),
                            XStringFormats.TopCenter);
                    }

                    document.Save(saveDialog.FileName);

                    MessageBox.Show($"Raport PDF został zapisany jako:\n{saveDialog.FileName}", "Sukces",
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

        private List<string> FormatResultsForPdf(string resultsText)
        {
            var lines = new List<string>();
            var paragraphs = resultsText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            foreach (var paragraph in paragraphs)
            {
                if (string.IsNullOrWhiteSpace(paragraph))
                {
                    lines.Add(" "); 
                    continue;
                }

                if (paragraph.Length > 100)
                {
                    var words = paragraph.Split(' ');
                    var currentLine = "";

                    foreach (var word in words)
                    {
                        if ((currentLine + word).Length > 100)
                        {
                            lines.Add(currentLine.Trim());
                            currentLine = "";
                        }
                        currentLine += word + " ";
                    }

                    if (!string.IsNullOrWhiteSpace(currentLine))
                        lines.Add(currentLine.Trim());
                }
                else
                {
                    lines.Add(paragraph);
                }
            }

            return lines;
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

        private void BtnSamplesAnalyst_Click(object sender, RoutedEventArgs e)
        {
            SamplesAnalyst samples = new SamplesAnalyst(_currentUser);
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
            Profile profile = new Profile(_currentUser, this);
            profile.Show();
            this.Hide();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            _parentWindow?.Show();
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
