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

namespace MedLabTab.Views.OtherViews
{
    /// <summary>
    /// Interaction logic for Statistics.xaml
    /// </summary>
    public partial class Statistics : Window
    {
        private Window _parentWindow;
        public Statistics(Window parentWindow)
        {
            InitializeComponent();
            FillData();
            _parentWindow = parentWindow;
        }

        private void FillData()
        {
            var tests = DbManager.GetCompletedTests();

            if (tests == null || !tests.Any())
            {
                Income.Text = "";
                Test.Text = "";
                Nurse.Text = "";
                Analyst.Text = "";
                Patient.Text = "";
                return;
            }

            // Income
            float income = tests.Sum(t => DbManager.GetTest(t.TestId)?.Price ?? 0);
            Income.Text = income.ToString();

            // Most common test
            var mostCommonTest = tests
                .GroupBy(t => t.TestId)
                .Select(g => new { TestId = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .FirstOrDefault();

            var testInfo = mostCommonTest != null ? DbManager.GetTest(mostCommonTest.TestId) : null;
            Test.Text = testInfo?.TestName ?? "Brak danych";

            // Most common nurse
            var mostCommonNurse = tests
                .GroupBy(t => DbManager.GetNurseIdFromTestHistory(t))
                .Select(g => new { NurseId = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .FirstOrDefault();

            var nurseInfo = mostCommonNurse != null ? DbManager.GetUserById(mostCommonNurse.NurseId) : null;
            Nurse.Text = nurseInfo != null ? $"{nurseInfo.Name} {nurseInfo.Surname}" : "Brak danych";

            // Most common analyst
            var mostCommonAnalyst = tests
                .GroupBy(t => t.AnalystId)
                .Select(g => new { AnalystId = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .FirstOrDefault();

            var analystInfo = mostCommonAnalyst?.AnalystId != null ? DbManager.GetUserById(mostCommonAnalyst.AnalystId.Value) : null;
            Analyst.Text = analystInfo != null ? $"{analystInfo.Name} {analystInfo.Surname}" : "Brak danych";

            // Most common patient
            var mostCommonPatient = tests
                .GroupBy(t => t.PatientId)
                .Select(g => new { PatientId = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .FirstOrDefault();

            var patientInfo = mostCommonPatient != null ? DbManager.GetUserById(mostCommonPatient.PatientId) : null;
            Patient.Text = patientInfo != null ? $"{patientInfo.Name} {patientInfo.Surname}" : "Brak danych";
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            _parentWindow?.Show();
        }
    }
}
