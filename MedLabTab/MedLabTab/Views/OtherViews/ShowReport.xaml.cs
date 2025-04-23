using MedLabTab.DatabaseModels;
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
    /// Interaction logic for Report.xaml
    /// </summary>
    public partial class ShowReport : Window
    {
        private Window _parentWindow;
        //private Report _testReport;
        public ShowReport(/*Report report,*/ Window parentWindow)
        {
            InitializeComponent();
            _parentWindow = parentWindow;
            //_testReport = report;
            FillReportWithData();
        }
        private void FillReportWithData()
        {
            //TestTextBlock.Text = _testReport.Test;
            //PatientTextBlock.Text = _testReport.Patient;
            //NurseTextBlock.Text = _testReport.Nurse;
            //AnalystTextBlock.Text = _testReport.Analyst;
            //DateTextBlock.Text = _testReport.Date;
            //ResultTextBox.Text = _testReport.Result;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            _parentWindow?.Show();
        }
    }
}
