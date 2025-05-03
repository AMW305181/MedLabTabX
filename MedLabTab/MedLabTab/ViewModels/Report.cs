using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedLabTab.ViewModels
{
    public class Report
    {
        public string TestName { get; set; }
        public string PatientName { get; set; }
        public string AnalystName { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }

        public string Results { get; set; }
    }
}
